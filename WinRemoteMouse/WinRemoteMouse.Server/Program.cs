using System;
using System.IO;
using System.Linq;
using System.Threading;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace WinRemoteMouse.Server
{
    class Program
    {
        static void Main()
        {
            var rfcommServiceUuid = Guid.Parse("87240836-54AF-41F8-A881-09F06364EEFC");
            var listener = new BluetoothListener(rfcommServiceUuid)
            {
                ServiceName = "Win 8 remote mouse"
            };
            BluetoothRadio.PrimaryRadio.Mode = RadioMode.Discoverable;
            listener.Start();
            var client = listener.AcceptBluetoothClient();
            Console.WriteLine("Conectado");
            while (client.Connected)
            {
                var stream = client.GetStream();
                if (stream.DataAvailable)
                {
                    Console.WriteLine("Data avaliable");
                    using (var reader = new StreamReader(stream))
                    {
                        var buffer = new char[512];
                        int received;
                        while ((received = reader.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            var msg = new string(buffer, 0, received);
                            var split = msg.Split(new[] { ';' });
                            int i = 0;
                            var query = (from s in split
                                         let num = i++
                                         where !string.IsNullOrEmpty(s)
                                         group s by num / 3 into g
                                         select g.ToArray()).ToArray();

                            foreach (var s in query)
                            {
                                var type = split[0].Split(':')[1];
                                MousePointer p;
                                if (!MouseInterop.GetCursorPos(out p))
                                {
                                    return;
                                }
                                switch (type)
                                {
                                    case "Move":
                                        var x = int.Parse(s[1].Split(':')[1]); //p.X + int.Parse(s[1].Split(':')[1]);
                                        var y = int.Parse(s[2].Split(':')[1]); //p.Y + int.Parse(s[2].Split(':')[1]);
                                        MouseInterop.SetCursorPos(x, y);
                                        break;
                                    case "LClick":
                                        MouseInterop.MouseEvent(
                                                    MouseInterop.MouseeventLeftDown | MouseInterop.MouseeventLeftUp,
                                                    (uint)p.X, (uint)p.Y, 0, (UIntPtr)0);

                                        break;
                                    case "DoubleClick":
                                        MouseInterop.MouseEvent(
                                            MouseInterop.MouseeventLeftDown | MouseInterop.MouseeventLeftUp,
                                            (uint)p.X, (uint)p.Y, 0, (UIntPtr)0);
                                        Thread.Sleep(150);
                                        MouseInterop.MouseEvent(
                                            MouseInterop.MouseeventLeftDown | MouseInterop.MouseeventLeftUp,
                                            (uint)p.X, (uint)p.Y, 0, (UIntPtr)0);
                                        break;

                                    case "RClick":
                                        MouseInterop.MouseEvent(
                                            MouseInterop.MouseEventRightDown | MouseInterop.MouseEventRightUp,
                                            (uint)p.X, (uint)p.Y, 0, (UIntPtr)0);
                                        break;
                                }
                            }
                            Console.WriteLine(msg);
                        }
                    }
                }
                Thread.Sleep(150);
            }
            Console.ReadLine();
        }
    }
}