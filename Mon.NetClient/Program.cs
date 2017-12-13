using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Mon.NetClient
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        static void Main(String[] args)
        {
            if (System.Environment.UserInteractive)
            {
                if (args.Length > 0)
                {
                    switch (args[0])
                    {
                        case "-i":
                            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
                            processInstaller.Account = ServiceAccount.LocalSystem;

                            ServiceInstaller installer = new ServiceInstaller();
                            InstallContext context = new InstallContext();
                            string path = String.Format("/assemblypath={0}", Assembly.GetExecutingAssembly().Location);
                            string[] cmdLine = { path };

                            context = new InstallContext(@"C:\Users\Szyu\Desktop\servicelog.log", cmdLine);
                            installer.Context = context;
                            installer.DisplayName = "MY_SERVICE";
                            installer.Description = "XXX";
                            installer.ServiceName = "SelfHostService";
                            installer.StartType = ServiceStartMode.Automatic;
                            installer.Parent = processInstaller;

                            ListDictionary state = new ListDictionary();
                            installer.Install(state);
                            break;
                        case "-u":
                            ServiceInstaller ServiceInstallerObj = new ServiceInstaller();
                            InstallContext Context = new InstallContext(@"C:\Users\Szyu\Desktop\servicelog.log", null);
                            ServiceInstallerObj.Context = Context;
                            ServiceInstallerObj.ServiceName = "SelfHostService";
                            ServiceInstallerObj.Uninstall(null);
                            break;
                        case "-s":
                            {
                                ServiceController[] serviceController = ServiceController.GetServices();
                                ServiceController service = serviceController.FirstOrDefault(c => c.ServiceName == "SelfHostService");
                                if (service != null)
                                {
                                    service.Start();
                                    service.WaitForStatus(ServiceControllerStatus.Running);
                                }
                                break;
                            }
                        case "-e":
                            {
                                ServiceController[] serviceController = ServiceController.GetServices();
                                ServiceController service = serviceController.FirstOrDefault(c => c.ServiceName == "SelfHostService");
                                if (service != null)
                                {
                                    service.Stop();
                                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                                }
                                break;
                            }
                    }
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    new SelfHostService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
