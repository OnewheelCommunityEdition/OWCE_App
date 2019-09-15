using System;
using OWCE.DependencyInterfaces;
using Xamarin.Forms;
using CoreBluetooth;
using Foundation;
using CoreFoundation;
using System.Threading.Tasks;
using System.Linq;

#if __IOS__
using OWCE.iOS.Extensions;
[assembly: Dependency(typeof(OWCE.iOS.DependencyImplementations.OWBLE))]
namespace OWCE.iOS.DependencyImplementations
#elif __MACOS__
using OWCE.MacOS.Extensions;
[assembly: Dependency(typeof(OWCE.MacOS.DependencyImplementations.OWBLE))]
namespace OWCE.MacOS.DependencyImplementations
#endif
{
    public class OWBLE : NSObject, IOWBLE, ICBCentralManagerDelegate
    {
        public Action<string> BLEStateChanged { get; set; }
        public Action<OWBoard> BoardDiscovered { get; set; }
        public Action<OWBoard> BoardConnected { get; set; }

        private CBCentralManager _centralManager;
        private CBPeripheral _peripheral;
        private DispatchQueue _dispatchQueue;

        public OWBLE()
        {
            _dispatchQueue = new DispatchQueue("CBCentralManager_Queue");
            _centralManager = new CBCentralManager(this, _dispatchQueue);
            /*
            _centralManager.ConnectedPeripheral += CentralManager_ConnectedPeripheral;
            _centralManager.DisconnectedPeripheral += CentralManager_DisconnectedPeripheral;
            _centralManager.DiscoveredPeripheral += CentralManager_DiscoveredPeripheral;
            _centralManager.FailedToConnectPeripheral += CentralManager_FailedToConnectPeripheral;
            _centralManager.RetrievedConnectedPeripherals += CentralManager_RetrievedConnectedPeripherals;
            _centralManager.RetrievedPeripherals += CentralManager_RetrievedPeripherals;
            _centralManager.UpdatedState += CentralManager_UpdatedState;
            _centralManager.WillRestoreState += CentralManager_WillRestoreState;
            */
        }

        public async Task StartScanning(int timeout = 15)
        {
            Console.WriteLine("StartScanning");

            /*
            //B9859BB5-2CF8-4583-A396-9950656F2265 
            var serviceUuids = new Guid[]
            {
                //Guid.Parse("40DD9B0B-7D8D-FA29-B8D7-28F026F6C247"),
                //Guid.Parse("B9859BB5-2CF8-4583-A396-9950656F2265"),
                Guid.Parse("E659F300-EA98-11E3-AC10-0800200C9A66"),
            };
            //40DD9B0B-7D8D-FA29-B8D7-28F026F6C247
            //B9859BB5-2CF8-4583-A396-9950656F2265 
            CBUUID[] serviceCbuuids = null;
            if (serviceUuids != null && serviceUuids.Any())
            {
                serviceCbuuids = serviceUuids.Select(u => CBUUID.FromString(u.ToString())).ToArray();
            }
            */
            _centralManager.ScanForPeripherals(new CBUUID[] { OWBoard.ServiceUUID.ToCBUUID() }, new PeripheralScanningOptions { AllowDuplicatesKey = true });

            //CBUUID[] peripherals = null;
            //_centralManager.ScanForPeripherals(peripherals, new PeripheralScanningOptions { AllowDuplicatesKey = false });

            /*
            //Console.WriteLine("IsMainThread: " + Xamarin.Essentials.MainThread.IsMainThread);
            _centralManager.ScanForPeripherals((CBUUID[])null); // OWBoard.ServiceUUID.ToCBUUID());


            if (_centralManager.IsScanning)
            {
                _centralManager.StopScan();
            }
            */

            await Task.Delay(timeout);

            StopScanning();
        }

        public void StopScanning()
        {
            Console.WriteLine("StopScanning");
            _centralManager.StopScan();
        }

        public void Disconnect()
        {
            Console.WriteLine("Disconnect");
            _centralManager.CancelPeripheralConnection(_peripheral);
        }

#region CBCentralManager_Delegate
        [Export("centralManager:didConnectPeripheral:")]
        public void CentralManager_ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
        {
            Console.WriteLine("CentralManager_ConnectedPeripheral: " + peripheral.Name);
        }

        [Export("centralManager:didDiscoverPeripheral:advertisementData:RSSI:")]
        public void CentralManager_DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
        {
            Console.WriteLine("CentralManager_DiscoveredPeripheral: "+ peripheral.Name);

            OWBoard board = new OWBoard()
            {
                ID = peripheral.Identifier.ToString(),
                Name = peripheral.Name ?? "Onewheel",
                IsAvailable = true,
            };

            BoardDiscovered?.Invoke(board);
        }

        [Export("centralManager:didDisconnectPeripheral:error:")]
        public void CentralManager_DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            Console.WriteLine("CentralManager_DisconnectedPeripheral");
        }

        [Export("centralManager:didFailToConnectPeripheral:error:")]
        public void CentralManager_FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError error)
        {
            Console.WriteLine("CentralManager_FailedToConnectPeripheral");
        }

        [Export("centralManager:didRetrieveConnectedPeripherals:")]
        public void CentralManager_RetrievedConnectedPeripherals(CBCentralManager central, CBPeripheral[] peripherals)
        {
            Console.WriteLine("CentralManager_RetrievedConnectedPeripherals");

        }

        [Export("centralManager:didRetrievePeripherals:")]
        public void CentralManager_RetrievedPeripherals(CBCentralManager central, CBPeripheral[] peripherals)
        {
            Console.WriteLine("CentralManager_RetrievedPeripherals");
        }

        public void UpdatedState(CBCentralManager central)
        {
            Console.WriteLine("CentralManager_UpdatedState: " + central.State);
        }

        private void CentralManager_WillRestoreState(object sender, CBWillRestoreEventArgs e)
        {
            Console.WriteLine("CentralManager_WillRestoreState");

        }
#endregion
    }
}
