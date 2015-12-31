using System;
using System.Collections.Generic;
using System.Configuration;
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
            CreateDomain();
            Console.ReadLine();
        }

        private static void ChangeAppSetting()
        {
            Console.WriteLine("ChangeAppSetting:" + ConfigurationManager.AppSettings["Kim"]);
            setConfigFileAtRuntime();
            Console.WriteLine("ChangeAppSetting:" + ConfigurationManager.AppSettings["Kim"]);
        }

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
