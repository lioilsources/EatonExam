using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EatonExam
{
    static class ToolEx
    {
        public static IEnumerable<int> Range(int start, int stop)
        {
            MinMaxSwap(ref start, ref stop);
            return Times(stop - start + 1).Select(i => i + start);
        }

        public static void MinMaxSwap<T>(ref T a, ref T b)
            where T : IComparable<T>
        {
            if (a.CompareTo(b) > 0)
                Swap(ref a, ref b);
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            var c = a;
            a = b;
            b = c;
        }

        public static IEnumerable<int> Times(this int number)
        {
            for (int i = 0; i < number; i++)
            {
                yield return i;
            }
        }
          
        public static void Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }
    }

    class DevicesEnvironment
    {
        readonly MonitorDevice monitor;
        readonly string deviceIds;

        static CountdownEvent countdownSynchronization;

        public class Message
        {
            public char DeviceId { get; set; }
            public int Measurement { get; set; }
        }

        public class MonitorDevice
        {
            List<Message> queue = new List<Message>();

            public void AddMessage(Message m)
            {
                lock (queue)
                {
                    queue.Add(m);
                    //Console.WriteLine($"[{m.DeviceId},{m.Measurement}]");
                }
            }

            public List<Message> ListMessages() => queue;
        }

        public class MeasuringDevice
        {
            readonly char deviceId;
            readonly int numOfMessages;
            readonly int frequency;

            Random rng;
            readonly MonitorDevice monitor;

            public MeasuringDevice(int randomSeed, char deviceId, MonitorDevice monitor)
            {
                this.deviceId = deviceId;
                this.monitor = monitor;

                this.rng = new Random(randomSeed);
                this.numOfMessages = rng.Next(0, 99);
                this.frequency = rng.Next(0, 10);
            }

            int GetRandomMeasurement() => rng.Next(0, 999);

            public void Run()
            {
                for (int i = 0; i < numOfMessages; i++)
                {
                    var message = new Message
                    {
                        DeviceId = deviceId,
                        Measurement = GetRandomMeasurement()
                    };
                    monitor.AddMessage(message);

                    Thread.Sleep(frequency);
                }

                DevicesEnvironment.SingnalCountdown();
            }
        }

        public DevicesEnvironment(string deviceIds)
        {
            this.deviceIds = deviceIds;
            this.monitor = new MonitorDevice();

            countdownSynchronization = new CountdownEvent(deviceIds.Length);
        }

        int GetNumOfDevices() => deviceIds.Length - 1;

        static void SingnalCountdown() => countdownSynchronization.Signal();

        public void RunMeasuringDevices()
        {
            ToolEx.Range(0, GetNumOfDevices())
            .Select(index => new MeasuringDevice(index, deviceIds[index], monitor))
            .Each(device => new Thread(device.Run).Start());
        }

        public void PrintMeasurements()
        {
            countdownSynchronization.Wait();

            monitor.ListMessages()
            .GroupBy(m => m.DeviceId)
            .Select(g => new
            {
                Device = g.Key,
                MessageCount = g.Count()
            })
            .OrderBy(r => r.Device)
            .Each(result => Console.WriteLine($"Device {result.Device}: {result.MessageCount}x measurements"));
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            var system = new DevicesEnvironment("ABC");
            system.RunMeasuringDevices();
            system.PrintMeasurements();
        }
    }
}
