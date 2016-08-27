using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace BlockThemAll
{
    internal class IniSettings
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
                        if (s.Length < 2) continue;
                        if (string.IsNullOrEmpty(section))
                        {
                            section = "Default";
                            if (!settings.ContainsKey(section))
                                settings.Add(section, new Dictionary<string, string>());
                        }

                        settings[section].Add(s[0].Trim(), string.Join("=", s, 1, s.Length - 1).Trim());
                    }
                }
            }
        }

        internal string GetValue(string section, string key)
        {
            if (!settings.ContainsKey(section)) return null;
            return settings[section].ContainsKey(key) ? settings[section][key] : null;
        }

        internal string GetValue(string section, string key, string value) {
            var result = GetValue(section, key);
            if (result == null) {
                SetValue(section, key, value);
                return value;
            }
            return result;
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
            var sb = new StringBuilder();
            foreach (var sections in settings)
            {
                sb.AppendFormat("[{0}]{1}", sections.Key, Environment.NewLine);
                foreach (var config in sections.Value)
                    sb.AppendLine(string.Join(" = ", config.Key, config.Value));
            }

            File.WriteAllText(ini.Name, sb.ToString());
        }
    }
}