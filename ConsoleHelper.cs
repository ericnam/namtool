using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamTool
{
    public static class ConsoleHelper
    {
        /// <summary>
        ///     Writes a header for the application
        /// </summary>
        public static void InstantiateHeader()
        {
            Console.WriteLine("NamTool");
        }

        /// <summary>
        ///     Lists all currently accepted actions
        /// </summary>
        public static void ActionInfo()
        {
            Console.WriteLine();
            Console.WriteLine("List of currently accepted actions:");
            Console.WriteLine(" -- Format Conversions");
        }

        /// <summary>
        ///     Gets user input for which action user wants to implement
        /// </summary>
        /// <returns></returns>
        public static string GetUserInput()
        {
            Console.WriteLine();
            Console.Write("What would you like to do? ");
            return Console.ReadLine();
        }

        /// <summary>
        ///     Write description of chosen action, and ask for confirmation
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public static bool ConfirmAction(string description)
        {
            Console.WriteLine();
            Console.WriteLine("Desc:");
            Console.WriteLine(description);

            while (true)
            {
                Console.Write("Y/N: ");
                var result = Console.ReadLine();

                switch (result)
                {
                    case "Y":
                        return true;
                    case "N":
                        Console.WriteLine("Action cancelled.");
                        return false;
                    default:
                        Console.WriteLine("Please select 'Y' or 'N'.");
                        break;
                }
            }
        }

    }
}
