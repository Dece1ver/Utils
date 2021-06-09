using System;

namespace KeyViewer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            while (true)
            {
                var pressed = Console.ReadKey();
                Console.WriteLine(" Нажата клавиша: " + pressed.Key);
            }
        }
    }
}
