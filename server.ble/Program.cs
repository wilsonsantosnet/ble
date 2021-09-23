using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace server.ble
{
    class Program
    {

        static void Main(string[] args) {

            ExecuteAsync(args).Wait();
            Console.Read();
        }
        static async Task ExecuteAsync(string[] args)
        {

            var uuid = Guid.NewGuid();
            var uuid1 = Guid.NewGuid();
            var uuid2 = Guid.NewGuid();
            var uuid3 = Guid.NewGuid();

            GattServiceProvider serviceProvider = null;
            var result = await GattServiceProvider.CreateAsync(uuid);

            if (result.Error == BluetoothError.Success)
            {
                serviceProvider = result.ServiceProvider;
            }

            var value = new byte[] { 0x21 };
            var constantParametersRead = new GattLocalCharacteristicParameters
            {
                CharacteristicProperties = (GattCharacteristicProperties.Read),
                StaticValue = value.AsBuffer(),
                ReadProtectionLevel = GattProtectionLevel.Plain,
                UserDescription = "teste1"
            };

            var characteristicResult = await serviceProvider.Service.CreateCharacteristicAsync(uuid1, constantParametersRead);
            if (characteristicResult.Error != BluetoothError.Success)
            {
                // An error occurred.
                return;
            }

            var constantParametersWrite = new GattLocalCharacteristicParameters
            {
                CharacteristicProperties = (GattCharacteristicProperties.Write),
                StaticValue = value.AsBuffer(),
                ReadProtectionLevel = GattProtectionLevel.Plain,
                UserDescription = "teste2"
            };
            var _readCharacteristic = characteristicResult.Characteristic;
            _readCharacteristic.ReadRequested += ReadCharacteristic_ReadRequested;

            characteristicResult = await serviceProvider.Service.CreateCharacteristicAsync(uuid2, constantParametersWrite);
            if (characteristicResult.Error != BluetoothError.Success)
            {
                // An error occurred.
                return;
            }

            var constantParametersNotify = new GattLocalCharacteristicParameters
            {
                CharacteristicProperties = (GattCharacteristicProperties.Notify),
                StaticValue = value.AsBuffer(),
                ReadProtectionLevel = GattProtectionLevel.Plain,
                UserDescription = "teste3"
            };
            var _writeCharacteristic = characteristicResult.Characteristic;
            _writeCharacteristic.WriteRequested += WriteCharacteristic_WriteRequested;

            characteristicResult = await serviceProvider.Service.CreateCharacteristicAsync(uuid3, constantParametersNotify);
            if (characteristicResult.Error != BluetoothError.Success)
            {
                // An error occurred.
                return;
            }
            var _notifyCharacteristic = characteristicResult.Characteristic;
            _notifyCharacteristic.SubscribedClientsChanged += SubscribedClientsChanged;


            var advParameters = new GattServiceProviderAdvertisingParameters
            {
                IsDiscoverable = true,
                IsConnectable = true
            };
            serviceProvider.StartAdvertising(advParameters);

            Console.WriteLine($"{uuid}|{uuid1}|{uuid2}|{uuid3}");
        }

        private static void SubscribedClientsChanged(GattLocalCharacteristic sender, object args)
        {
            Console.WriteLine("SubscribedClientsChanged!");
        }

        private static void WriteCharacteristic_WriteRequested(GattLocalCharacteristic sender, GattWriteRequestedEventArgs args)
        {
            Console.WriteLine("WriteCharacteristic_WriteRequested!");
        }

        private static void ReadCharacteristic_ReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
        {
            Console.WriteLine("ReadCharacteristic_ReadRequested!");
        }
    }
}
