// (c)2018 David Pusch, see License.txt for full license

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

namespace BTMusicRenamer
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<long,string> musicConfig = generateConfigList();
            Console.Write("Choose operation (cleartext names = c, hexnames = h):");
            char decision = Convert.ToChar(Console.ReadLine());
            if (decision == 'c')
            {
                renameToClearText(ref musicConfig);
            }
            else if (decision == 'h')
            {
                renameToHex(ref musicConfig);
            }
            else
            {
                Environment.Exit(0);
            }
            Console.WriteLine("Rename is done, press any key to exit.");
            Console.ReadKey();
        }

        private static void renameToClearText(ref Dictionary<long, string> musicConfig)
        {
            string[] musicFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.wav");
            long key = 0;
            string value = null;
            foreach (string musicFile in musicFiles)
            {
                value = null;
                key = long.Parse(stripPath(musicFile), System.Globalization.NumberStyles.HexNumber);
                try
                {
                    value = musicConfig[key];
                    string newFileName = Environment.CurrentDirectory + "\\" + value + ".wav";
                    Console.WriteLine(String.Format("Renaming {0} to {1}", musicFile, newFileName));
                    File.Move(musicFile, newFileName);
                }
                catch
                {
                    Console.WriteLine(String.Format("Music file {0} does not map to any key in config file", musicFile));
                }
                
            }
        }

        private static void renameToHex(ref Dictionary<long, string> musicConfig)
        {
            string[] musicFiles = Directory.GetFiles(Environment.CurrentDirectory, "*.wav");
            string value = "";
            long key = 0;
            foreach (string musicFile in musicFiles)
            {
                key = 0;
                value = stripPath(musicFile);
                key = musicConfig.FirstOrDefault(x => x.Value == value).Key;
                
                if (key != 0)
                {
                    string newFileName = Environment.CurrentDirectory + "\\" + key.ToString("x8") + ".wav";
                    Console.WriteLine(String.Format("Renaming {0} to {1}", musicFile, newFileName));
                    File.Move(musicFile, newFileName);
                }
                
            }
        }

        private static Dictionary<long, string> generateConfigList()
        {
            var configList = new Dictionary<long, string>();
            string[] musicTxtLines = File.ReadAllLines(Environment.CurrentDirectory + "/music.txt");
            string key = "";
            string value = "";
            foreach (string configLine in musicTxtLines)
            {
                if (Regex.IsMatch(configLine, "^\t"))
                {
                    key = configLine.TrimStart('\t');
                    key = key.Substring(0, key.IndexOf('\t'));
                    
                    value = configLine.TrimStart('\t');
                    value = value.Substring(key.Length, value.Length-key.Length);
                    value = value.TrimStart('\t');
                    value = value.Substring(0, value.IndexOf('\t'));
                    
                    
                    configList.Add(Convert.ToInt64(key), value);
                    
                }
            }
            return configList;
        }


        private static string stripPath(string musicFile)
        {
            musicFile = musicFile.Remove(0, Environment.CurrentDirectory.Length+1);
            musicFile = musicFile.Remove(musicFile.Length-4, 4);
            return musicFile;
        }
    }
}
