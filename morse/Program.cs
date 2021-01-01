using System;

namespace morse
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(string.Join(" ", "crème brûlée".ToMorse()));
        }
    }
}
