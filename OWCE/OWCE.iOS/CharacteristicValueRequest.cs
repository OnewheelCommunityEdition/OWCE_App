using System;
using System.Threading.Tasks;
using CoreBluetooth;
using Foundation;

namespace OWCE.iOS
{
    public class CharacteristicValueRequest : IEquatable<CharacteristicValueRequest>
    {
        public Guid ID { get; private set; } = Guid.NewGuid();
        public CBUUID CharacteristicId { get; private set; }
        public TaskCompletionSource<byte[]> CompletionSource { get; private set; }
        public NSData Data { get; private set; }

        public CharacteristicValueRequest(CBUUID characteristicId, TaskCompletionSource<byte[]> completionSource, NSData data)
        {
            CharacteristicId = characteristicId;
            CompletionSource = completionSource;
            Data = data;
        }

        public bool Equals(CharacteristicValueRequest other)
        {
            if (other == null)
                return false;

            return other.ID.Equals(ID);
        }
    }
}
