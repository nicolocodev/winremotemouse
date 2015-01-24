using System;
using System.Threading;
using WinRemoteMouse.Common;

namespace WinRemoteMouse.Server
{
    internal class Program
    {
        private static void Main()
        {
            var client = Server.Start();
            Console.WriteLine("Connected");
            while (client.Connected)
            {
                var stream = client.GetStream();
                if (stream.DataAvailable)
                {
                    Console.WriteLine("Data avaliable");
                    foreach (var command in WinRemoteMouseMsg.DecodeMessageStream(stream))
                    {
                        var type = command.Item1;
                        MousePointer p;
                        if (!MouseInterop.GetCursorPos(out p)) return; //TODO: Show message...
                        switch (type)
                        {
                            case CommandType.Move:
                                var x = p.X + command.Item2*2;
                                var y = p.Y + command.Item3*2;
                                MouseInterop.SetCursorPos(x, y);
                                break;
                            case CommandType.Lclick:
                                MouseInterop.MouseEvent(
                                    MouseInterop.MouseeventLeftDown | MouseInterop.MouseeventLeftUp, (uint) p.X,
                                    (uint) p.Y, 0, (UIntPtr) 0);
                                break;
                            case CommandType.DoubleClick:
                                MouseInterop.MouseEvent(
                                    MouseInterop.MouseeventLeftDown | MouseInterop.MouseeventLeftUp, (uint) p.X,
                                    (uint) p.Y, 0, (UIntPtr) 0);
                                Thread.Sleep(150);
                                MouseInterop.MouseEvent(
                                    MouseInterop.MouseeventLeftDown | MouseInterop.MouseeventLeftUp, (uint) p.X,
                                    (uint) p.Y, 0, (UIntPtr) 0);
                                break;
                            case CommandType.RClick:
                                MouseInterop.MouseEvent(
                                    MouseInterop.MouseEventRightDown | MouseInterop.MouseEventRightUp, (uint) p.X,
                                    (uint) p.Y, 0, (UIntPtr) 0);
                                break;
                        }
                    }
                }
                Thread.Sleep(150);
            }
        }
    }
}