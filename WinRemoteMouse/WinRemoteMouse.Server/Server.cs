using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using WinRemoteMouse.Common;

namespace WinRemoteMouse.Server
{
    public class Server
    {
        internal static BluetoothClient Start()
        {
            var listener = new BluetoothListener(Constants.RfcommChatServiceUuid)
            {
                ServiceName = Constants.Servicename
            };
            BluetoothRadio.PrimaryRadio.Mode = RadioMode.Discoverable;
            listener.Start();
            return listener.AcceptBluetoothClient();
        }
    }
}
