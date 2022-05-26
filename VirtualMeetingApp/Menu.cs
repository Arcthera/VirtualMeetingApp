using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualMeetingApp
{
    public static class Menu
    {
        public static void Intro()
        {
            Console.WriteLine("Welcome to the Visma virtual meeting app.");
        }

        public static string ReadUserInput(string prompt)
        {
            Console.WriteLine(prompt);
            return Console.ReadLine();
        }

        public static string ReadUserInput()
        {
            return Console.ReadLine();
        }

        public static void Output(string message)
        {
            Console.WriteLine(message);
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static void WaitForInput()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public static void DisplayOptions()
        {
            Clear();
            Console.WriteLine("Please select a command.");
            Console.WriteLine("\t 1 - Create a new meeting");
            Console.WriteLine("\t 2 - Delete an existing meeting.");
            Console.WriteLine("\t 3 - Add person to a meeting.");
            Console.WriteLine("\t 4 - Remove person from a meeting.");
            Console.WriteLine("\t 5 - List all meetings.");
            Console.WriteLine("\t 6 - Exit.");
        }
    }
}
