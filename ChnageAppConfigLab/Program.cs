using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ChnageAppConfigLab
{
    class Program
    {
        static void Main(string[] args)
        {
            //   ChangeAppSetting();
            //  CreateDomain();
            UseExeConfigurationFileMap();
            Console.ReadLine();
        }

        private static void ChangeAppSetting()
        {
            Console.WriteLine("ChangeAppSetting:" + ConfigurationManager.AppSettings["Kim"]);
            setConfigFileAtRuntime();
            Console.WriteLine("ChangeAppSetting:" + ConfigurationManager.AppSettings["Kim"]);
        }

        //http://dotnet-posts.blogspot.tw/2013/12/switching-between-configuration-config.html
        private static void UseExeConfigurationFileMap()
        {
            Console.WriteLine("UseExeConfigurationFileMap:" + ConfigurationManager.AppSettings["Kim"]);
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            ExeConfigurationFileMap map = new ExeConfigurationFileMap();
            map.ExeConfigFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App.QAS.config");
            Configuration configStage = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

            config.AppSettings.Settings.Clear();
            //replace appsettings
            foreach (KeyValueConfigurationElement setting in configStage.AppSettings.Settings)
            {
                config.AppSettings.Settings.Add(setting);
            }

            config.ConnectionStrings.ConnectionStrings.Clear();
            //replace connectionString
            foreach (ConnectionStringSettings setting in configStage.ConnectionStrings.ConnectionStrings)
            {
                config.ConnectionStrings.ConnectionStrings.Add(setting);
            }
            config.Save();
            ConfigurationManager.RefreshSection("connectionStrings");
            ConfigurationManager.RefreshSection("appSettings");
            Console.WriteLine("UseExeConfigurationFileMap:" + ConfigurationManager.AppSettings["Kim"]);
            Console.WriteLine("UseExeConfigurationFileMap:" + ConfigurationManager.ConnectionStrings["Kim"].ConnectionString);

        }

        private KeyValueConfigurationCollection GetAppSettingsFromSelectedConfig(string path)
        {
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = path };
            Configuration newConfiguration = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            if ((newConfiguration.AppSettings == null) || (newConfiguration.AppSettings.Settings == null))
                return null;

            return newConfiguration.AppSettings.Settings; ;
        }

        //http://stackoverflow.com/questions/658498/how-to-load-an-assembly-to-appdomain-with-all-references-recursively
        private static void CreateDomain()
        {
            string exeAssembly = Assembly.GetEntryAssembly().FullName;

            // setup - there you put the path to the config file
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = System.Environment.CurrentDirectory;
            setup.ConfigurationFile = @"E:\GitHub\ChnageAppConfigLab\ChnageAppConfigLab\bin\Debug\App.QAS.config";

            // create the app domain
            Console.WriteLine("CreateDomain:" + ConfigurationManager.AppSettings["Kim"]);
            AppDomain appDomain = AppDomain.CreateDomain("My AppDomain", null, setup);
            appDomain.Load(AssemblyName.GetAssemblyName($"{System.Environment.CurrentDirectory}\\ChnageAppConfigLab.exe").Name);
            // create proxy used to call the startup method 
            KimClass proxy = (KimClass)appDomain.CreateInstanceAndUnwrap(exeAssembly, typeof(KimClass).FullName);
            proxy.Write();
            AppDomain.Unload(appDomain);
        }

        protected static void setConfigFileAtRuntime()
        {
            string runtimeconfigfile = @"E:\GitHub\ChnageAppConfigLab\ChnageAppConfigLab\bin\Debug\App.QAS-appsetting.config";
            // Specify config settings at runtime.
            System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.File = runtimeconfigfile;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
public class KimClass : MarshalByRefObject
{

    public void Write()
    {
        Console.WriteLine("CreateDomain:" + ConfigurationManager.AppSettings["Kim"]);

    }
}
