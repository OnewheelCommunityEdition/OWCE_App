using System;
using System.Threading.Tasks;

namespace OWCE.Droid
{
    public class CharacteristicValueRequest : IEquatable<CharacteristicValueRequest>
    {
        public Guid ID { get; private set; } = Guid.NewGuid();
        public string CharacteristicId { get; private set; }
        public TaskCompletionSource<byte[]> CompletionSource { get; private set; }
        public byte[] Data { get; private set; }

        public CharacteristicValueRequest(string characteristicId, TaskCompletionSource<byte[]> completionSource, byte[] data)
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
