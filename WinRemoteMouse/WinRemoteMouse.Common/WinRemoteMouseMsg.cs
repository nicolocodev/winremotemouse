using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WinRemoteMouse.Common
{
    public class WinRemoteMouseMsg
    {
        public static IEnumerable<Tuple<CommandType, int, int>> DecodeMessageStream(Stream stream)
        {
            var messages = GetMessage(stream).Select(DecodeMesage);
            foreach (var message in messages)
                foreach (var command in message)
                {
                    CommandType type;
                    if (!Enum.TryParse(command[0].Split(':')[1], out type)) throw new InvalidOperationException();
                    if (type != CommandType.Move) yield return new Tuple<CommandType, int, int>(type, 0, 0);
                    else
                    {
                        var x = int.Parse(command[1].Split(':')[1]) * 2;
                        var y = int.Parse(command[2].Split(':')[1]) * 2;
                        yield return new Tuple<CommandType, int, int>(type, x, y);
                    }
                }
        }


        private static IEnumerable<string> GetMessage(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                var buffer = new char[512];
                int received;
                while ((received = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    var msg = new string(buffer, 0, received);
                    yield return msg;
                }
            }
        }

        static string[][] DecodeMesage(string message)
        {
            var split = message.Split(new[] { ';' });
            var i = 0;
            var query = (from s in split
                         let num = i++
                         where !string.IsNullOrEmpty(s)
                         group s by num / 3
                into g
                         select g.ToArray()).ToArray();
            return query;
        }
    }

    public enum CommandType
    {
        Move =1,
        Lclick = 2,
        RClick = 3,
        DoubleClick = 4
    }
}
