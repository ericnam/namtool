using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamTool
{
    public static class FormatConversions
    {
        static string MLP_APP_PATH = "C:\\Dev\\MLP";
        static string[] affectedProjects = new string[] { "Business", "Common", "ElectronicFiling", "FederalK1Viewer", "UI", "Validation" };
        static string[] conversionsType1 = new string[] { "Convert.ToString(", "Convert.ToDateTime(", "Convert.ToInt32(", "Convert.ToInt64(", "Convert.ToDouble(", "Convert.ToBoolean(", "Convert.ToDecimal(", "int.Parse(", "double.Parse(", "decimal.Parse(", "DateTime.Parse(" };
        static string[] conversionsType2 = new string[] { "string.Format(", "String.Format(" };

        static string CULTUREINFO_INVARIANTCULTURE = "CultureInfo.InvariantCulture";
        static string CULTUREINFO_CURRENTCULTURE = "CultureInfo.CurrentCulture";
        static string SYSTEM_GLOBALIZATION = "using System.Globalization;";

        static Dictionary<int, string> lineNumberToLineDictionary = new Dictionary<int, string>();
        static Dictionary<int, string> lineNumberToNamespace = new Dictionary<int, string>();
        static bool includesGlobalizationNamespace = false;
        static int namespaceLineNumber = 0;

        /// <summary>
        ///     Execute FormatConversions Action
        /// </summary>
        public static void Execute()
        {
            string[] files = Directory.GetFiles(MLP_APP_PATH, "*cs", SearchOption.AllDirectories);
            
            foreach (string file in files)
            {
                //clear global dictionaries that mapes line to line number
                lineNumberToLineDictionary.Clear();
                lineNumberToNamespace.Clear();
                includesGlobalizationNamespace = false;

                //Make sure we're in the right project
                if (!ValidateProject(file))
                    continue;

                string[] lines = File.ReadAllLines(file);
                int i = 0;
                foreach (string line in lines)
                {
                    UpdateConversionWithCultureInfo(line, i);
                    i++;
                }

                if (lineNumberToLineDictionary.Count == 0)
                    continue;

                StreamWriter strmw = new StreamWriter(file);
                strmw.Flush();

                string newLine;
                i = 0;
                
                foreach (string line in lines)
                {
                    if (!includesGlobalizationNamespace && i == namespaceLineNumber)
                    {
                        strmw.WriteLine(SYSTEM_GLOBALIZATION);
                    }

                    if (lineNumberToLineDictionary.TryGetValue(i, out newLine))
                    {
                        strmw.WriteLine(newLine);
                    } else
                    {
                        strmw.WriteLine(line);
                    }
                    i++;
                }

                strmw.Close();
            }
        }

        /// <summary>
        ///     Check if the given line has any conversions included, add CultureInfo.Invariant culture to correct spot.
        /// </summary>
        /// <param name="line"></param>
        /// <param name="index"></param>
        public static Dictionary<int, string> UpdateConversionWithCultureInfo(string line, int lineIndex)
        {
            CalculateGlobalizationStatement(line, lineIndex);

            string convertedLine;
            foreach (string conversion in conversionsType1)
            {
                int i = 0;
                int l = 0;
                int r = 0;
                if (line.Contains(conversion))
                {
                    i = line.IndexOf(conversion);

                    foreach (Char c in line.Substring(i))
                    {
                        l = (c == '(') ? l + 1 : l;
                        r = (c == ')') ? r + 1 : r;

                        if (c == ')' && l == r)
                        {
                            if (!line.Substring(i - CULTUREINFO_INVARIANTCULTURE.Length).Contains(CULTUREINFO_INVARIANTCULTURE) && 
                                !line.Substring(i - CULTUREINFO_CURRENTCULTURE.Length).Contains(CULTUREINFO_CURRENTCULTURE))
                            {
                                if(!lineNumberToLineDictionary.TryGetValue(lineIndex, out convertedLine))
                                {
                                    lineNumberToLineDictionary.Add(lineIndex, line.Substring(0, i) + ", " + CULTUREINFO_INVARIANTCULTURE + line.Substring(i, line.Length - i));
                                }
                                else
                                {
                                    lineNumberToLineDictionary[lineIndex] = line.Substring(0, i) + ", " + CULTUREINFO_INVARIANTCULTURE + line.Substring(i, line.Length - i);
                                }
                            }
                            Console.WriteLine(line.Substring(0, i) + ", " + CULTUREINFO_INVARIANTCULTURE + line.Substring(i, line.Length - i));
                            break;
                        }
                        i++;
                    }
                }
            }

            foreach (string conversion in conversionsType2)
            {
                int i = 0;
                if (line.Contains(conversion))
                {
                    i = line.IndexOf(conversion);

                    foreach (Char c in line.Substring(i))
                    {
                        if (c == '(')
                        {
                            if (!line.Contains(CULTUREINFO_INVARIANTCULTURE))
                            {
                                if (!lineNumberToLineDictionary.TryGetValue(lineIndex, out convertedLine))
                                {
                                    lineNumberToLineDictionary.Add(lineIndex, line.Substring(0, i + 1) + CULTUREINFO_INVARIANTCULTURE + ", " + line.Substring(i + 1, line.Length - i - 1));
                                }
                                else
                                {
                                    lineNumberToLineDictionary[lineIndex] = line.Substring(0, i + 1) + CULTUREINFO_INVARIANTCULTURE + ", " + line.Substring(i + 1, line.Length - i - 1);
                                }
                            }
                            Console.WriteLine(line.Substring(0, i + 1) + CULTUREINFO_INVARIANTCULTURE + ", " + line.Substring(i + 1, line.Length - i - 1));
                            break;
                        }
                        i++;
                    }
                }
            }

            return lineNumberToLineDictionary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineIndex"></param>
        public static void CalculateGlobalizationStatement(string line, int lineIndex)
        {
            if (line.Contains("using"))
            {
                lineNumberToNamespace.Add(lineIndex, line);
            }

            if (line.Contains("namespace"))
            {
                foreach (string l in lineNumberToNamespace.Values)
                {
                    if (l.Contains(SYSTEM_GLOBALIZATION))
                    {
                        includesGlobalizationNamespace = true;
                    }
                }

                if (!includesGlobalizationNamespace)
                {
                    if (lineNumberToNamespace.Count == 0)
                    {
                        namespaceLineNumber = 0;
                    }
                    else
                    {
                        namespaceLineNumber = lineNumberToNamespace.Keys.ToArray().Max() + 1;
                    }

                }
            }
        }

        /// <summary>
        ///     Make sure that the file path only contains valid projects
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool ValidateProject(string file)
        {
            if (file.Contains("GlobalSuppressions"))
                return false;

            foreach (string project in affectedProjects)
            {
                if (file.Contains("Dev\\MLP\\" + project + "\\"))
                    return true;
            }
            return false;
        }

    }
}
