using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EllApp_server.Classes
{
    class Config_Manager
    {
        string pathFileConfig = "";
        string[] filecontent;
        public Config_Manager()
        {
            pathFileConfig = "constants.conf";
            filecontent = File.ReadAllLines(pathFileConfig);
        }

        public string getValue(string indexer)
        {
            string result = "";
            bool found = false;
            foreach (string line in filecontent)
            {
                if (!found)
                {
                    if (!line.Contains("#"))
                    {
                        if (line.Contains(indexer))
                        {
                            result = BetweenOfFixed(line, indexer + " = ", ";");
                            found = true;
                        }
                        else
                            result = "La chiave " + indexer + " non è stata trovata.";
                        //result = result + " - " + line;
                    }
                }
            }
            return result;
        }

        private static string BetweenOfFixed(string ActualStr, string StrFirst, string StrLast)
        {
            int startIndex = ActualStr.IndexOf(StrFirst) + StrFirst.Length;
            int endIndex = ActualStr.IndexOf(StrLast, startIndex);
            return ActualStr.Substring(startIndex, endIndex - startIndex);
        }

    }
}
