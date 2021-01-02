namespace OWCE.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class ListExtension
    {
        public static T BinarySearchClosestBind<T, TKey>(this IList<T> list, Func<T, TKey> keySelector, TKey key)
            where TKey : IComparable<TKey>
        {
            if (list.Count == 0)
            {
                throw new InvalidOperationException("Empty list; Item not found");
            }

            int min = 0;
            int max = list.Count;

            while (min < max)
            {
                int mid = min + ((max - min) / 2);
                T midItem = list[mid];
                TKey midKey = keySelector(midItem);
                int comp = midKey.CompareTo(key);

                if (comp < 0)
                {
                    min = mid + 1;
                }
                else if (comp > 0)
                {
                    max = mid - 1;
                }
                else
                {
                    return midItem;
                }
            }

            if (min < list.Count)
            {
                return list[min];
            }
            else
            {
                return list[max - 1];
            }
        }
    }
}
