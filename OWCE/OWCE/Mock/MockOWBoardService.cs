using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace OWCE
{
	public class MockOWBoardService : IService
    {
        public MockOWBoardService()
        {
        }

        public Guid Id => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public bool IsPrimary => throw new NotImplementedException();

        public IDevice Device => throw new NotImplementedException();

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<ICharacteristic> GetCharacteristicAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ICharacteristic>> GetCharacteristicsAsync()
        {
            throw new NotImplementedException();
        }
    }
}
