using System;
using System.Collections.Generic;
using System.Linq;

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
        Random rng = new Random();
        MonitorDevice monitor;

        public MeasuringDevice(char deviceId, MonitorDevice monitor)
        {
            this.deviceId = deviceId;
            this.monitor = monitor;
        }

        public int GetRandomMeasurement(Random rng) => rng.Next(0, 999);

        public void Run()
        {
            var message = new Message
            {
                DeviceId = deviceId,
                Measurement = GetRandomMeasurement(rng)
            };
            monitor.AddMessage(message);
        }
    }

    class MainClass
    {
        public static void Main(string[] args)
        {
            var rng = new Random();
            var messagesCount = 1000;
            var deviceIds = "ABCDEFGHIJKLM";

            var monitor = new MonitorDevice();

            var numOfDevices = deviceIds.Length - 1;
            var devices = ToolEx.Range(0, numOfDevices)
            .Select(index => new MeasuringDevice(deviceIds[index], monitor)).ToArray();
            
            ToolEx.Range(0, messagesCount)
            .Each(i =>
            {
                var device = devices[rng.Next(0, numOfDevices)];
                device.Run();
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
