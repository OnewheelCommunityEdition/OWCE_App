using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;

namespace OWCE
{
	public class MockOWBoardCharacteristic : ICharacteristic
    {
        public MockOWBoardCharacteristic()
        {
        }

        public Guid Id => throw new NotImplementedException();

        public string Uuid => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public byte[] Value => throw new NotImplementedException();

        public string StringValue => throw new NotImplementedException();

        public CharacteristicPropertyType Properties => throw new NotImplementedException();

        public CharacteristicWriteType WriteType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool CanRead => throw new NotImplementedException();

        public bool CanWrite => throw new NotImplementedException();

        public bool CanUpdate => throw new NotImplementedException();

        public IService Service => throw new NotImplementedException();

        public event EventHandler<CharacteristicUpdatedEventArgs> ValueUpdated;

        public Task<IDescriptor> GetDescriptorAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<IList<IDescriptor>> GetDescriptorsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ReadAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public Task StartUpdatesAsync()
        {
            throw new NotImplementedException();
        }

        public Task StopUpdatesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> WriteAsync(byte[] data, CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }
    }
}
