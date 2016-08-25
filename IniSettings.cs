using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace BlockThemAll
{
    class IniSettings
    {
        private readonly FileInfo ini;
        private readonly Dictionary<string, Dictionary<string, string>> settings = new Dictionary<string, Dictionary<string, string>>();

        public IniSettings(FileInfo file)
        {
            ini = file;

            if (file.Exists)
            {
                string section = null;
                foreach (string line in File.ReadAllLines(file.Name))
                {
                    Match sectionMatch = Regex.Match(line, @"\[(?<section>.+?)\]");
                    if (sectionMatch.Success)
                    {
                        section = sectionMatch.Groups["section"].Value;
                        if (!settings.ContainsKey(section))
                            settings.Add(section, new Dictionary<string, string>());
                    }
                    else
                    {
                        string[] s = line.Split('=');
                        if (s.Length != 2) continue;
                        if (string.IsNullOrEmpty(section))
                        {
                            section = @"Default";
                            if (!settings.ContainsKey(section))
                                settings.Add(section, new Dictionary<string, string>());
                        }

                        settings[section].Add(s[0].Trim(), s[1].Trim());
                    }
                }
            }
        }

        internal string GetValue(string section, string key)
        {
            if (!settings.ContainsKey(section)) return string.Empty;
            return settings[section].ContainsKey(key) ? settings[section][key] : string.Empty;
        }

        internal void SetValue(string section, string key, string value)
        {
            if (!settings.ContainsKey(section))
                settings.Add(section, new Dictionary<string, string>());

            if (!settings[section].ContainsKey(key))
                settings[section].Add(key, value);
            else
                settings[section][key] = value;
        }

        internal void Save()
        {
            string text = string.Empty;
            foreach (KeyValuePair<string, Dictionary<string, string>> sections in settings)
            {
                text += @"[" + sections.Key + @"]" + Environment.NewLine;
                text = sections.Value.Aggregate(text, (current, config) => current + (config.Key + @" = " + config.Value + Environment.NewLine));
            }

            File.WriteAllText(ini.Name, text);
        }
    }
}
