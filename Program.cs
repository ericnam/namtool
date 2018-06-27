using System;

namespace NamTool
{
    /// <summary>
    ///     A tool developed to help with wide variety of issues/tasks regarding the MLP application.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHelper.InstantiateHeader();

            while (true)
            {
                var action = ConsoleHelper.GetUserInput();
                switch (action)
                {
                    case "Format Conversions":
                        if (ConsoleHelper.ConfirmAction("This action will format all conversions within the application to include CultureInfo.InvariantCulture.\nDo you wish to continue?"))
                            FormatConversions.Execute();
                        break;
                    case "Format EDMX":
                        if (ConsoleHelper.ConfirmAction("This action will format your Central EDMX file to remove all SysStartTime and SysEndTime schema changes. \nDo you wish to continue?"))
                            FormatEdmx.Execute();
                        break;
                    case "-i":
                        ConsoleHelper.ActionInfo();
                        break;
                    default:
                        Console.WriteLine("Unrecognized action. Please input a valid action.");
                        break;
                }
            }
        }
    }
}
