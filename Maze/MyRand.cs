using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    class MyRand : Random
    {
        public static Random rand = new Random();

        public static void GenerateRandomList(int length, int[] arr)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < length; i++)
                list.Add(i);
            for (int i = 0; i < length; i++)
            {
                int r = rand.Next(length - i);
                arr[i] = list[r];
                list.RemoveAt(r);
            }
        }

        public static void GenerateRandomList(int length, int range, int[] arr)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < range; i++)
                list.Add(i);
            for (int i = 0; i < length; i++)
            {
                int r = rand.Next(range - i);
                arr[i] = list[r];
                list.RemoveAt(r);
            }
        }

        public static int NextInt(int maxValue)
        {
            return rand.Next(maxValue);
        }
    }
}
