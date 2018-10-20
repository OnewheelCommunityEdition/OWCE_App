using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;

namespace OWCE
{
	public class MockOWBoardDevice : IDevice
    {
        public Guid Id => new Guid(OWBoard.ServiceUUID);

        public string Name => "FakeOWBoard";

        private int _rssi = -70;
        public int Rssi => _rssi;

        public object NativeDevice => null;

        public DeviceState State => DeviceState.Connected;

        public IList<AdvertisementRecord> AdvertisementRecords => new List<AdvertisementRecord>();

        private ConnectionInterval _connectionInterval;

        private List<MockOWBoardService> _services = new List<MockOWBoardService>();


        public MockOWBoardDevice()
        {
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        public Task<IService> GetServiceAsync(Guid id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var foundService = _services.First(service => service.Id == id);
            return Task.FromResult<IService>(foundService);
        }

        public Task<IList<IService>> GetServicesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            IList<IService> services = (System.Collections.Generic.IList<Plugin.BLE.Abstractions.Contracts.IService>)_services;
            return Task.FromResult<IList<IService>>(services);
        }

        public Task<int> RequestMtuAsync(int requestValue)
        {
            // What do?
            return Task.FromResult<int>(1);
        }

        public bool UpdateConnectionInterval(ConnectionInterval interval)
        {
            _connectionInterval = interval;
            return true;
        }

        public Task<bool> UpdateRssiAsync()
        {
            _rssi = new Random().Next(-80, -60);
            return Task.FromResult<bool>(true);
        }
    }
}
