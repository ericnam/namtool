using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamTool
{
    public static class FormatEdmx
    {
        static string MLP_CENTRAL_PATH = "C:\\Dev\\MLP\\Data\\MLPCentral";
        static string[] SYS_VARIABLES = new string[] { "SysStartTime", "SysEndTime" };

        public static void Execute()
        {
            string[] edmxFiles = Directory.GetFiles(MLP_CENTRAL_PATH, "*edmx", SearchOption.TopDirectoryOnly);
            string filePath = edmxFiles.FirstOrDefault();

            string[] lines = File.ReadAllLines(filePath);
            List<int> sysVariableLine = new List<int>();

            int i = 0;
            foreach (string line in lines)
            {
                if (line.Contains(SYS_VARIABLES[0]) || line.Contains(SYS_VARIABLES[1]))
                {
                    sysVariableLine.Add(i);
                    Console.WriteLine(line);
                }
                i++;
            }

            i = 0;
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                sw.Flush();

                foreach (string line in lines)
                {
                    if (sysVariableLine.Contains(i))
                    {
                        i++;
                        continue;
                    }
                    sw.WriteLine(line);
                    i++;
                }

                sw.Close();
            }

        }
    }
}
