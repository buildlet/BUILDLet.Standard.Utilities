/***************************************************************************************************
The MIT License (MIT)

Copyright 2015 Daiki Sakamoto

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and 
associated documentation files (the "Software"), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, 
sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or 
substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT 
NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND 
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, 
DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
***************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using BUILDLet.Standard.Utilities.Properties;
using BUILDLet.Standard.Diagnostics;

namespace BUILDLet.Standard.Utilities.Network
{
    /// <summary>
    /// マジックパケット (AMD Magic Packet Format) を実装します。
    /// </summary>
    public static class MagicPacket
    {
        // ----------------------------------------------------------------------------------------------------
        // Constructor(s)
        // ----------------------------------------------------------------------------------------------------
        // (None)


        // ----------------------------------------------------------------------------------------------------
        // Static Properties
        // ----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 許容されている MAC アドレスの既定の区切り文字を取得します。
        /// </summary>
        public static readonly char[] Delimiters = { ':', '-' };


        // ----------------------------------------------------------------------------------------------------
        // Static Method(s)
        // ----------------------------------------------------------------------------------------------------

        // Get Datagram as Byte Array
        private static byte[] GetDatagram(string macAddress)
        {
#if DEBUG
            Debug.WriteLine($"MAC Address=\"{macAddress}\"", DebugInfo.ShortName);
#endif

            // Validation
            bool match = false;
            foreach (var separator in MagicPacket.Delimiters)
            {
                if (Regex.IsMatch($"{macAddress}{separator}", $"^((([0-9A-Za-z]{{2}}){separator}){{6}})$")) { match = true; }
            }
            if (!match)
            {
                // ERROR
                throw new FormatException(Resources.InvalidArgumentErrorMessage);
            }


            // for UDP Datagram (Byte Array)
            byte[] dgram = new byte[6 * (1 + 16)];

            // SET Header (0xFF * 6)
            (new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF }).CopyTo(dgram, 0);

            // Convert MAC Address from String to Byte Array
            var mac = macAddress.Split(MagicPacket.Delimiters).ToList().ConvertAll(hex => Convert.ToByte(hex, 16));

            // SET MAC Address * 16
            for (int i = 0; i < 16; i++) { mac.ToArray().CopyTo(dgram, 6 + (i * 6)); }

#if DEBUG
            for (int i = 0; i < (1 + 16); i++)
            {
                Debug.Write($"Magic Packet[{i:D2}]=0x", DebugInfo.ShortName);
                for (int j = 0; j < 6; j++) { Debug.Write($"{dgram[(i * 6) + j]:X2}"); }
                Debug.WriteLine("");
            }
#endif

            // RETURN
            return dgram;
        }


        // Send Datagram
        private static async Task<string[]> SendDatagramAsync(string macAddress, int port, int count, int interval, bool async)
        {
            // GET UDP Datagram (Byte Array)
            var dgram = MagicPacket.GetDatagram(macAddress);

            // List of IP Address sent
            List<string> addresses = new List<string>();

            // for NetworkInterface(s)
            foreach (var address in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                // only for IPv4
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    var lep = new IPEndPoint(address, 0);
                    var udp = new UdpClient(lep);
                    var ep = new IPEndPoint(IPAddress.Broadcast, port);

                    // Sent Flag
                    bool sent = false;

                    for (int i = 0; i < count; i++)
                    {
                        // Send Datagram (Async or Sync)
                        var bytes = async ? await udp.SendAsync(dgram, dgram.Length, ep) : udp.Send(dgram, dgram.Length, ep);

                        if (bytes > 0)
                        {
                            // Set Sent Flag ON
                            sent = true;

#if DEBUG
                            Debug.Write($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] Send Magic Packet{(async ? " [Async]" : "")} (", DebugInfo.ShortName);
                            Debug.Write($"{i + 1}: MAC Address=\"{macAddress}\", {bytes} Bytes");
                            Debug.WriteLine($", Destination Port={port}, Source IP Address={lep.Address})");
#endif

                            if (i < count - 1)
                            {
                                // Wait for Interval
                                if (async)
                                {
                                    // Async
                                    await Task.Delay(interval);
                                }
                                else
                                {
                                    // NOT Async
                                    Thread.Sleep(interval);
                                }
                            }
                        }
                        else
                        {
                            // ERROR
                            throw new InvalidOperationException();
                        }
                    }

                    // Add IP Address sent
                    if (sent)
                    {
                        addresses.Add(address.ToString());
                    }
                }
            }

            // RETURN
            return addresses.ToArray();
        }


        /// <summary>
        /// マジックパケット (AMD Magic Packet Format) を送信します。
        /// </summary>
        /// <param name="macAddress">
        /// MAC アドレスの文字列を指定します。
        /// </param>
        /// <param name="port">
        /// リモートマシンのポート番号を指定します。
        /// 既定のポート番号は <c>2304</c> 番です。
        /// </param>
        /// <param name="count">
        /// マジックパケットを送信する回数を指定します。
        /// 既定の回数は <c>1</c> 回です。
        /// </param>
        /// <param name="interval">
        /// マジックパケットを送信する間隔を、ミリ秒単位で指定します。
        /// 既定の回数は <c>0</c> ミリ秒です。
        /// </param>
        /// <returns>
        /// マジックパケットを送信した IP アドレスの文字列配列を返します。
        /// </returns>
        public static string[] Send(string macAddress, int port = 2304, int count = 1, int interval = 0)
            => MagicPacket.SendDatagramAsync(macAddress, port, count, interval, false).Result;


        /// <summary>
        /// マジックパケット (AMD Magic Packet Format) を送信します。
        /// </summary>
        /// <param name="macAddress">
        /// MAC アドレスの文字列を指定します。
        /// </param>
        /// <param name="port">
        /// リモートマシンのポート番号を指定します。
        /// 既定のポート番号は <c>2304</c> 番です。
        /// </param>
        /// <param name="count">
        /// マジックパケットを送信する回数を指定します。
        /// 既定の回数は <c>1</c> 回です。
        /// </param>
        /// <param name="interval">
        /// マジックパケットを送信する間隔を、ミリ秒単位で指定します。
        /// 既定の回数は <c>0</c> ミリ秒です。
        /// </param>
        /// <returns>
        /// マジックパケットを送信した IP アドレスの文字列配列を返します。
        /// </returns>
        public static Task<string[]> SendAsync(string macAddress, int port = 2304, int count = 1, int interval = 0)
            => MagicPacket.SendDatagramAsync(macAddress, port, count, interval, true);
    }
}
