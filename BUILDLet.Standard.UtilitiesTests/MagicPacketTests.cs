/***************************************************************************************************
The MIT License (MIT)

Copyright 2021 Daiki Sakamoto

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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.NetworkInformation;  // for NetworkInterface

using BUILDLet.Standard.Utilities.Network;
using BUILDLet.Standard.Diagnostics;

namespace BUILDLet.Standard.Utilities.Network.Tests
{
    [TestClass]
    public class MagicPacketTests
    {
        // Get Number of NetworkInterfaces of Local Machine (Excluding Loopback Address)
        private static int GetNumberOfNetworkInterfaces() =>
            NetworkInterface.GetAllNetworkInterfaces().Where(ni => ni.OperationalStatus == OperationalStatus.Up).Count() - 1;


        [TestMethod]
        [TestCategory("Manual")]
        public void WolTest() => MagicPacket.Send("E4:11:5B:AD:32:38", count: 3, interval: 1000);


        [DataTestMethod]
        [TestCategory("Exception")]
        // [ExpectedException(typeof(FormatException))]
        [ExpectedException(typeof(AggregateException))]
        [DataRow("00+00-00-00-00-00")]
        [DataRow("00+00+00+00+00+00")]
        [DataRow("00-00-00-00-00-0-")]
        public void SendMacAddressInvalidFormatTest(string macAddress)
        {
            // ACT & ASSERT
            MagicPacket.Send(macAddress);
        }


        [DataTestMethod]
        [TestCategory("Exception")]
        [ExpectedException(typeof(FormatException))]
        // [ExpectedException(typeof(AggregateException))]
        [DataRow("00+00-00-00-00-00")]
        [DataRow("00+00+00+00+00+00")]
        [DataRow("00-00-00-00-00-0-")]
        public async Task SendAsyncMacAddressInvalidFormatTest(string macAddress)
        {
            try
            {
                // ACT & ASSERT
                await MagicPacket.SendAsync(macAddress);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [DataTestMethod]
        [DataRow("00-00-00-FF-FF-FF", -1, -1, -1, DisplayName = "w/o Parameter")]
        [DataRow("FF:FF:FF:00:00:00", 9, 5, 0)]
        [DataRow("01:23:45:AB:CD:EF", 9, 3, 100)]
        [DataRow("AB:CD:EF:01:23:45", 9, 2, 1000)]
        [DataRow("01:23:45:AB:CD:EF", 9, 0, 1000, DisplayName = "Count = 0")]
        public void SendTest(string macAddress, int port, int count, int interval)
        {
            // ARRANGE
            var expected = count == 0 ? 0 : MagicPacketTests.GetNumberOfNetworkInterfaces();

            // ACT: Send Magic Packet
            var sent = (port < 0 && count < 0 && interval < 0) ?
                MagicPacket.Send(macAddress) :
                MagicPacket.Send(macAddress, port, count, interval);

            // OUTPUT
            for (int i = 0; i < sent?.Length; i++)
            {
                Debug.WriteLine($"Source IP Address[{i}] = {sent[i]}");
            }

            // ASSERT
            Assert.AreEqual(expected, sent.Length);
        }


        [DataTestMethod]
        [DataRow(false, DisplayName = "NOT Async")]
        [DataRow(true, DisplayName = "Async")]
        public void SendSyncTest(bool async)
        {
            // ARRANGE
            var this_MethodName = $"{nameof(MagicPacketTests)}.{nameof(MagicPacketTests.SendSyncTest)}";
            var act_MethodName = async ?
                $"{nameof(MagicPacketTests)}.{nameof(MagicPacketTests.SendSyncTestAsync)}" :
                $"{nameof(MagicPacket)}.{nameof(MagicPacket.Send)}";

            var macAddress = "01:23:45:AB:CD:EF";
            var port = 9;
            var count = 2;
            var interval = 1000;
            var error = 100;

            var expected_count = MagicPacketTests.GetNumberOfNetworkInterfaces();
            var expected_timespan = TimeSpan.FromMilliseconds(interval * (count - 1) * MagicPacketTests.GetNumberOfNetworkInterfaces());

            string[] sent = null;
            Task<string[]> task = null;


            // OUTPUT: START
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this_MethodName}: START");

            // OUTPUT: Before Call
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this_MethodName}() --> {act_MethodName}()");
            Console.WriteLine();


            // GET Start Time
            var start_time = DateTime.Now;

            // ACT
            if (!async)
            {
                // NOT Async
                sent = MagicPacket.Send(macAddress, port, count, interval);
            }
            else
            {
                // Async
                task = async ? SendSyncTestAsync(macAddress, port, count, interval) : null;
            }

            // GET Timespan
            var time = DateTime.Now - start_time;


            // OUTPUT: Returned
            Console.WriteLine();
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this_MethodName}() <-- {act_MethodName}()");


            // Wait for complete (Async)
            if (async)
            {
                // OUTPUT: Before Wait for Complete
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this_MethodName}: Wait for Complete {act_MethodName}()");
                Console.WriteLine("-----");
                Console.WriteLine();

                // Wait for complete (Async)
                task.Wait();

                // OUTPUT: After Wait for Complete
                Console.WriteLine();
                Console.WriteLine("-----");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this_MethodName}: {act_MethodName}() is completed.");
            }

            
            // OUTPUT: END (before Assertion)
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this_MethodName}: END (Count = {expected_count}, Timespan = {time.TotalMilliseconds})");


            // ASSERT (Count)
            Assert.AreEqual(expected_count, !async ? sent.Length : task.Result.Length);

            // ASSERT (Timespan)
            if (!async)
            {
                // NOT Async
                Assert.IsTrue((time > (expected_timespan - TimeSpan.FromMilliseconds(error))) && (time < (expected_timespan + TimeSpan.FromMilliseconds(error))));
            }
            else
            {
                // Async
                Assert.IsTrue(time < TimeSpan.FromMilliseconds(error));
            }
        }

        // SendSyncTestAsync
        public static async Task<string[]> SendSyncTestAsync(string macAddress, int port, int count, int interval)
        {
            // ARRANGE
            var this_MethodName = $"{nameof(MagicPacketTests)}.{nameof(MagicPacketTests.SendSyncTestAsync)}";
            var act_MethodName = $"{nameof(MagicPacket)}.{nameof(MagicPacket.SendAsync)}";


            // OUTPUT: START
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this_MethodName}: START");

            // OUTPUT: Before Call
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this_MethodName}() --> {act_MethodName}()");


            // ACT
            var task = await MagicPacket.SendAsync(macAddress, port, count, interval);


            // OUTPUT: Returned
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this_MethodName}() <-- {act_MethodName}()");

            // OUTPUT: END
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {this_MethodName}: END");


            // RETURN
            return task;
        }
    }
}