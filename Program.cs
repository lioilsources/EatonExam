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

    class Message
    {
        public char DeviceId { get; set; }
        public int Measurement { get; set; }
    }

    class MonitorDevice
    {
        List<Message> queue = new List<Message>();

        public void AddMessage(Message m)
        {
            lock (queue)
            {
                queue.Add(m);
            }
        }

        public List<Message> ListMessages() => queue;
    }

    class MeasuringDevice
    {
        char deviceId;
        int numOfMessages;

        Random rng;
        MonitorDevice monitor;

        public MeasuringDevice(int index, char deviceId, MonitorDevice monitor)
        {
            this.deviceId = deviceId;
            this.monitor = monitor;

            this.rng = new Random(index);
            this.numOfMessages = rng.Next(0, 99);
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
                //Thread.Sleep(10);
                monitor.AddMessage(message);
            }
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            var rng = new Random();
            var deviceIds = "ABCD";

            var monitor = new MonitorDevice();

            var numOfDevices = deviceIds.Length - 1;
            ToolEx.Range(0, numOfDevices)
            .Select(index => new MeasuringDevice(index, deviceIds[index], monitor))
            .Each(device =>
            {
                var t = new Thread(device.Run);
                t.Start();
                t.Join();
            });
    
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
}
