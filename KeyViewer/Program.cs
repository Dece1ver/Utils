using System;

namespace KeyViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                ConsoleKeyInfo pressed;
                pressed = Console.ReadKey();

                Console.WriteLine(" Нажата клавиша: " + pressed.Key);
            }
        }
    }
}
