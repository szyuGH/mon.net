using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MonCore
{
    public class MonitoringProcessor
    {
        public object Invoke(MonRequest request)
        {
            #region ##Definitions
            /*
             Method:    WMI
             Path:      root\WMI
             Query:     MSAcpi_ThermalZoneTemperature; CurrentTemperature, ThermalStamp
             */
            //ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"root\WMI", "SELECT * FROM MSAcpi_ThermalZoneTemperature");
            //foreach (ManagementObject obj in searcher.Get())
            //{
            //    Double temp = Convert.ToDouble(obj["CurrentTemperature"].ToString());
            //    UInt32 thermalStamp = Convert.ToUInt32(obj["ThermalStamp"].ToString());
            //    Console.WriteLine(temp);
            //    Console.WriteLine(thermalStamp);
            //}
            //------------------------------------------------------
            //var res = Wmi(@"root\WMI", "MSAcpi_ThermalZoneTemperature", "CurrentTemperature", "ThermalStamp");


            /*
             Method:    REG
             Path:      HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\CMF\Config
             Query:     SYSTEM
             */
            //object regVal = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\CMF\Config", "SYSTEM", null);
            //------------------------------------------------------
            //var res = Reg(@"HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\CMF\Config", "SYSTEM");


            /*
             Method:    SVC
             Path:      lfscv
             Query:     Status
             */
            //ServiceController controller = new ServiceController("lfsvc");
            //PropertyInfo val = typeof(ServiceController).GetProperty("Status");
            //Console.WriteLine($"{val.Name}: {val.GetValue(controller)}");
            //------------------------------------------------------
            //var res = Svc("lfsvc", "Status");


            /*
             Method:    PRC
             Path:      postgres
             Query:     Id
             */
            //Process prc = Process.GetProcessesByName("postgres").FirstOrDefault();
            //PropertyInfo pi = typeof(Process).GetProperty("Id");
            //Console.WriteLine($"{prc.ProcessName}: {pi.Name}: {pi.GetValue(prc)}");
            //------------------------------------------------------
            //var res = Prc("postgres", "Id");


            /*
             Method:    CMD
             Path:      netstat
             Query:     -t
             */
            //Process prc = new Process();
            //ProcessStartInfo psi = new ProcessStartInfo("netstat", "");
            //psi.RedirectStandardOutput = true;
            //psi.UseShellExecute = false;
            //prc.StartInfo = psi;
            //prc.Start();
            //prc.WaitForExit();
            //Console.WriteLine($"Output:\n{prc.StandardOutput.ReadToEnd()}");
            //------------------------------------------------------
            //var res = Cmd("netstat", "");
            #endregion

            try
            {
                switch (request.Method)
                {
                    case "WMI":
                        string[] wmiSplit = request.Query.Split(';');
                        return Wmi(request.Path, wmiSplit[0].Trim(), wmiSplit[1].Split(',').Select(s => s.Trim()).ToArray());
                    case "REG":
                        return Reg(request.Path, request.Query);
                    case "SVC":
                        return Svc(request.Path, request.Query.Split(',').Select(s => s.Trim()).ToArray());
                    case "PRC":
                        return Prc(request.Path, request.Query.Split(',').Select(s => s.Trim()).ToArray());
                    case "CMD":
                        return Cmd(request.Path, request.Query);
                    default:
                        return null;
                }
            } catch (Exception e)
            {
                return e;
            }
        }

        private object Wmi(string path, string manObject, params string[] properties)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(path, "SELECT * FROM " + manObject);
            Dictionary<string, Dictionary<string, object>> values = new Dictionary<string, Dictionary<string, object>>();
            
            foreach (ManagementObject obj in searcher.Get())
            {
                values[obj.Path.ToString()] = new Dictionary<string, object>();
                foreach (string property in properties)
                {
                    values[obj.Path.ToString()].Add(property, obj[property]);
                }
            }
            return values;
        }

        private object Reg(string path, string query)
        {
            return Registry.GetValue(path, query, null);
        }

        private object Svc(string serviceName, params string[] properties)
        {
            ServiceController controller = new ServiceController(serviceName);
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (string property in properties)
            {
                values[property] = typeof(ServiceController).GetProperty(property);
            }
            return values;
        }

        private object Prc(string processName, params string[] properties)
        {
            Dictionary<string, Dictionary<string, object>> values = new Dictionary<string, Dictionary<string, object>>();
            Process[] prcs = Process.GetProcessesByName(processName);
            foreach (Process prc in prcs)
            {
                values[prc.ProcessName] = new Dictionary<string, object>();
                foreach (string property in properties)
                {
                    values[prc.ProcessName][property] = typeof(Process).GetProperty(property)?.GetValue(prc);
                }
            }
            return values;
        }

        private object Cmd(string command, string args)
        {
            ProcessStartInfo psi = new ProcessStartInfo(command, args);
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            object lockObject = new object();
            object val = null;
            Process prc = Process.Start(psi);
            while (!prc.StandardOutput.EndOfStream)
            {
                Console.WriteLine(prc.StandardOutput.ReadLine());
            }
            return val;
        }
    }
}
