using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace question4
{
    class Program
    {
        public static string convertOperator(string line) {
            string result = "";
            if (line.Contains("AND"))
                result = line.Replace("AND", "&");
            else if (line.Contains("OR"))
                result = line.Replace("OR", "|");
            else if (line.Contains("LSHIFT"))
                result = line.Replace("LSHIFT", "<<");
            else if (line.Contains("RSHIFT"))
                result = line.Replace("RSHIFT", ">>");
            else if (line.Contains("NOT"))
                result = line.Replace("NOT", "~");
            else
                result = line;

            return result;
        }

        public static bool extractVarAndValue (string line, ref string variable, ref ushort value) {
            bool ret = false;
            string [] stringSeparators = new string[] {"->"};
            string [] strArray = line.Split(stringSeparators, StringSplitOptions.None);
            if (strArray.Length == 2) {
                variable = strArray[1].Trim();
                value = ushort.Parse(strArray[0]);
                ret = true;
            }

            return ret;
        }

        public static List<int> replaceVarWithValue(ref List<string> unresolvedLines, string variable, ushort value) {
            List<int> indexes = new List<int>();
            for (int i = 0; i < unresolvedLines.Count; i++) {
                string line = unresolvedLines[i];
                int start = line.IndexOf(variable, 0);
                if (start != -1) {
                    int pos = start + variable.Length;
                    if (pos < line.Length && !Char.IsLetterOrDigit(line[pos])) {
                        pos = start - 1;
                        if (pos < 0 || (pos > 0 && !Char.IsLetterOrDigit(line[pos]))) {
                            unresolvedLines[i] = line.Replace(variable, value.ToString());
                            indexes.Add(i);
                        }
                    }
                }
            }
            return indexes;
        }

        public static string getFirstPart(string line) {
            string part1 = string.Empty;

            string [] stringSeparators = new string[] {"->"};
            string [] strArray = line.Split(stringSeparators, StringSplitOptions.None);
            if (strArray.Length == 2) {
                part1 = strArray[0];
            }
            return part1;
        }

        public static bool anyVariable(string line) {
            bool ret = true;
            string part1 = getFirstPart(line);
            int pos = 0;
            while (pos < part1.Length && !Char.IsLetter(part1[pos])) {
                ++pos;
            }
            if (pos >= part1.Length)
                ret = false; // no variable
            return ret;
        }

        public static string evaluateLine(string line)
        {
            string result = line;
            string part1 = getFirstPart(line);
            if (part1.Contains("~")) {
                string numericString = Regex.Match(part1, @"\d+").Value;
                int value = int.Parse(numericString);
                UInt16 newValue = (UInt16)~value;

                result = newValue.ToString() + line.Substring(line.IndexOf(" ->"));
            } else if (part1.Contains("<<")) {
                string numericString1 = Regex.Match(part1, @"\d+").Value;
                string secondPart = part1.Substring(line.IndexOf("<<"));
                string numericString2 = Regex.Match(secondPart, @"\d+").Value;
                int value1 = int.Parse(numericString1);
                int value2 = int.Parse(numericString2);

                int newValue = value1 << value2;
                result = newValue.ToString() + line.Substring(line.IndexOf(" ->"));
            } else if (part1.Contains(">>")) {
                string numericString1 = Regex.Match(part1, @"\d+").Value;
                string secondPart = part1.Substring(line.IndexOf(">>"));
                string numericString2 = Regex.Match(secondPart, @"\d+").Value;
                int value1 = int.Parse(numericString1);
                int value2 = int.Parse(numericString2);

                int newValue = value1 >> value2;
                result = newValue.ToString() + line.Substring(line.IndexOf(" ->"));
            } else if (part1.Contains("|")) {
                string numericString1 = Regex.Match(part1, @"\d+").Value;
                string secondPart = part1.Substring(line.IndexOf("|"));
                string numericString2 = Regex.Match(secondPart, @"\d+").Value;
                int value1 = int.Parse(numericString1);
                int value2 = int.Parse(numericString2);

                int newValue = value1 | value2;
                result = newValue.ToString() + line.Substring(line.IndexOf(" ->"));
            } else if (part1.Contains("&")) {
                string numericString1 = Regex.Match(part1, @"\d+").Value;
                string secondPart = part1.Substring(line.IndexOf("&"));
                string numericString2 = Regex.Match(secondPart, @"\d+").Value;
                int value1 = int.Parse(numericString1);
                int value2 = int.Parse(numericString2);

                int newValue = value1 & value2;
                result = newValue.ToString() + line.Substring(line.IndexOf(" ->"));
            }

            return result;
        }

        public static void question4a() {
            string fileName = "question04_input.txt";
            if (File.Exists(fileName)) {
                using (StreamReader file = new StreamReader(fileName)) {
                    List<string> unresolvedLines = new List<string>();
                    List<string> resolvedLines = new List<string>();

                    string line;
                    while ((line = file.ReadLine()) != null) {
                        string numericString = Regex.Match(line, @"^\d+ ->").Value;
                        if (string.IsNullOrEmpty(numericString)) {
                            line = convertOperator(line);
                            unresolvedLines.Add(line);
                        } else
                            resolvedLines.Add(line);
                    }
                   
                    int current = 0;
                    while (resolvedLines.Count > 0 && current < resolvedLines.Count){
                        string varName = string.Empty;
                        ushort varValue = 0;

                        if (extractVarAndValue(resolvedLines[current], ref varName, ref varValue)) {
                            List<int> replaced = replaceVarWithValue(ref unresolvedLines, varName, varValue);
                            if (replaced.Count > 0) {
                                for (int i = replaced.Count - 1; i >= 0; i--) {
                                    if (!anyVariable(unresolvedLines[replaced[i]])) {
                                        string newLine = evaluateLine(unresolvedLines[replaced[i]]);
                                        resolvedLines.Add(newLine);
                                        unresolvedLines.RemoveAt(replaced[i]);
                                    }
                                }
                            }
                        }
                        current++;
                    }
                   
                    foreach(string x in resolvedLines) {
                        if (Regex.IsMatch(x, @"\ba\b")) 
                            Console.WriteLine(x);
                    }


                }
                
            }

            // Failed to open file
        }

        static void Main(string[] args) {
            question4a();
        }
    }
}
