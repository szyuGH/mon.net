using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Mon.NetClient
{
    static class Program
    {
        const string SERVICE_NAME = "mon.net.client";
        const string SERVICE_DESC = ".NET Monitoring Client";
        const string SERVICE_DISPLAYNAME = "Mon.NET Client";


        static ServiceController _service;
        static ServiceController Service
        {
            get
            {
                if (_service == null) 
                    _service = new ServiceController(SERVICE_NAME);

                return _service;
            }
        }

        static string LogPath
        {
            get
            {
                return $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}/Logs/servicelog.log";
            }
        }
        static object logLock = new object();

        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [MTAThread]
        static void Main(String[] args)
        {
            if (System.Environment.UserInteractive)
            {
                if (args.Length > 0)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(LogPath)))
                        Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
                    switch (args[0])
                    {
                        case "-i":
                            InstallService();

                            if (args.Length > 1 && args[1] == "-s")
                            {
                                StartService();
                            }
                            break;
                        case "-u":
                            UninstallService();
                            break;
                        case "-s":
                            StartService();
                            break;
                        case "-e":
                            StopService();
                            break;
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


        static void InstallService()
        {
            try
            {
                ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
                processInstaller.Account = ServiceAccount.LocalSystem;

                ServiceInstaller installer = new ServiceInstaller();
                InstallContext context = new InstallContext();
                string path = String.Format("/assemblypath={0}", Assembly.GetExecutingAssembly().Location);
                string[] cmdLine = { path };
                
                context = new InstallContext(LogPath, cmdLine);
                installer.Context = context;
                installer.DisplayName = SERVICE_DISPLAYNAME;
                installer.Description = SERVICE_DESC;
                installer.ServiceName = SERVICE_NAME;
                installer.StartType = ServiceStartMode.Automatic;
                installer.Parent = processInstaller;

                ListDictionary state = new ListDictionary();
                installer.Install(state);
            } catch (Exception e)
            {
                Log(e.Message);
            }
        }

        static void UninstallService()
        {
            StopService(); // if service is running, stop it
            try
            {
                ServiceInstaller ServiceInstallerObj = new ServiceInstaller();
                InstallContext Context = new InstallContext(LogPath, null);
                ServiceInstallerObj.Context = Context;
                ServiceInstallerObj.ServiceName = SERVICE_NAME;
                ServiceInstallerObj.Uninstall(null);
            } catch (Exception e)
            {
                Log(e.Message);
            }
        }

        static void StartService()
        {
            if (Service != null && Service.Status == ServiceControllerStatus.Stopped)
            {
                Log($"Starting Service {SERVICE_NAME}");
                try
                {
                    Service.Start();
                    Service.WaitForStatus(ServiceControllerStatus.Running);
                    Log($"Service {SERVICE_NAME} started!");
                } catch (Exception e)
                {
                    Log(e.Message);
                }
            }
        }

        static void StopService()
        {
            if (Service != null && Service.CanStop && Service.Status == ServiceControllerStatus.Running)
            {
                Log($"Stopping Service {SERVICE_NAME}");
                try
                {
                    Service.Stop();
                    Service.WaitForStatus(ServiceControllerStatus.Stopped);
                    Log($"Service {SERVICE_NAME} stopped!");
                }
                catch (Exception e)
                {
                    Log(e.Message);
                }
            }
        }

        static void Log(string s)
        {
            lock (logLock)
            {
                using (StreamWriter sw = new StreamWriter(new FileStream(LogPath, FileMode.Append)))
                {
                    sw.WriteLine($"{DateTime.Now.ToString("dd.MM.yyyy / HH:mm:ss")} - {s}");
                }
            }
        }
    }
}
