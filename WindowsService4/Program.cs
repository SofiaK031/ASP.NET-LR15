using System;
using System.ServiceProcess;
using System.Reflection;

namespace WindowsService4
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                if (args != null && args.Length > 0)
                {
                    switch (args[0])
                    {
                        case "--install":
                            try
                            {
                                var appPath = Assembly.GetExecutingAssembly().Location;
                                System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { appPath });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            break;
                        case "--uninstall":
                            try
                            {
                                var appPath = Assembly.GetExecutingAssembly().Location;
                                System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { "/u", appPath });
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.ToString());
                            }
                            break;
                    }
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new ServiceGetCryptoInf()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
