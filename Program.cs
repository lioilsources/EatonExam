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

    class MainClass
    {
        public static char GetRandomDevice(string deviceIds, Random rng)
        {
            int index = rng.Next(deviceIds.Length);
            return deviceIds[index];
        }

        public static int GetRandomMeasurement(Random rng) => rng.Next(0, 999);

        public static void Main(string[] args)
        {
            var rng = new Random();
            var messagesCount = 1000;
            var devices = "ABCDEFGHIJKLM";

            ToolEx.Range(0, messagesCount)
            .Select(i => new
            {
                Device = GetRandomDevice(devices, rng),
                Measurement = GetRandomMeasurement(rng)
            })
            .GroupBy(m => m.Device)
            .Select(g => new
            {
                Device = g.Key,
                MessageCount = g.Count()
            })
            .Each(result => Console.WriteLine($"Device {result.Device}: {result.MessageCount}x measurements"));
        }
    }
}
