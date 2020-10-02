using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TDSStudios.TomScript.UI.Util;

namespace TDSStudios.TomScript.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static UserSettings Settings;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Settings = UserSettings.Load();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            UserSettings.Save(Settings);
        }

    }
}
