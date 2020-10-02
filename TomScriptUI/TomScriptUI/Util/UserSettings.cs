using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace TDSStudios.TomScript.UI.Util
{
    public class UserSettings
    {

        private string pythonExecutableLocation;
        public string PythonExecutableLocation
        {
            get
            {
                if (pythonExecutableLocation == null)
                {
                    string pythonPath = PathLocater.GetPythonPath();

                    if (!string.IsNullOrEmpty(pythonPath))
                        pythonExecutableLocation = pythonPath;
                }
                return pythonExecutableLocation;
            }
            set
            {
                pythonExecutableLocation = value;
            }
        }


        public static void Save(UserSettings settings)
        {
            if (!Directory.Exists(PathLocater.BaseDataPath)) Directory.CreateDirectory(PathLocater.BaseDataPath);

            var json = JsonSerializer.Serialize(settings);

            File.WriteAllText(PathLocater.SettingsLocation, json);
        }

        public static UserSettings Load()
        {
            if (!File.Exists(PathLocater.SettingsLocation))
            {
                return new UserSettings();
            }

            var json = File.ReadAllText(PathLocater.SettingsLocation);

            return JsonSerializer.Deserialize<UserSettings>(json);
        }
    }
}
