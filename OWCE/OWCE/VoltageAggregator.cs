namespace OWCE
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    public class VoltageAggregator
    {
        private readonly TimeSpan TrimPeriod = TimeSpan.FromSeconds(60);

        private LinkedList<VoltageEntry> _contiguousList = new LinkedList<VoltageEntry>();
        private SortedList<float, VoltageEntry> _sortedList = new SortedList<float, VoltageEntry>(4096, new DuplicateKeyComparer<float>());
        private object entryLock = new object();

        public VoltageAggregator()
        {
        }

        public void AppendVoltageEntry(float voltage)
        {
            lock (entryLock)
            {
                var currentTime = DateTime.Now;
                var trimTime = currentTime - TrimPeriod;
                var newEntry = new VoltageEntry() { Voltage = voltage, Time = currentTime };

                _contiguousList.AddLast(newEntry);
                _sortedList.Add(voltage, newEntry);

                int countTrimmed = 0;
                while (_contiguousList.Count != 0 && _contiguousList.First.Value.Time < trimTime)
                {
                    var entryBeingRemoved = _contiguousList.First.Value;
                    _sortedList.Remove(entryBeingRemoved.Voltage);
                    _contiguousList.RemoveFirst();
                    ++countTrimmed;
                }

                Debug.WriteLine($"AppendVoltageEntry trimmed {countTrimmed} entries.");
            }
        }

        public float GetMedianVoltage()
        {
            lock (entryLock)
            {
                int count = _sortedList.Count;
                if (count == 0)
                {
                    return 0;
                }

                int i = count / 2;
                if (count % 2 == 0)
                {
                    var voltages = _sortedList.Keys;
                    return (voltages[i-1] + voltages[i]) * 0.5f;
                }
                else
                {
                    return _sortedList.Keys[i];
                }
            }
        }

        private class VoltageEntry
        {
            public float Voltage { get; set; }
            public DateTime Time { get; set; }
        }

        /// <summary>
        /// Comparer for comparing two keys, handling equality as beeing greater
        /// Use this Comparer e.g. with SortedLists or SortedDictionaries, that don't allow duplicate keys
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        private class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
        {
            #region IComparer<TKey> Members

            public int Compare(TKey x, TKey y)
            {
                int result = x.CompareTo(y);

                if (result == 0)
                {
                    return 1;   // Handle equality as beeing greater
                }
                else
                {
                    return result;
                }
            }

            #endregion
        }
    }
}
