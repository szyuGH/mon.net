using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MonCore
{
    public class MonitoringProcessor
    {
        public object Invoke(MonRequest request)
        {
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



            /*
             Method:    REG
             Path:      HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\CMF\Config
             Query:     SYSTEM
             */
            //object regVal = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\CMF\Config", "SYSTEM", null);



            /*
             Method:    SVC
             Path:      lfscv
             Query:     Status
             */
            //ServiceController controller = new ServiceController("lfsvc");
            //PropertyInfo val = typeof(ServiceController).GetProperty("Status");
            //Console.WriteLine($"{val.Name}: {val.GetValue(controller)}");



            /*
             Method:    PRC
             Path:      postgres
             Query:     Id
             */
            //Process prc = Process.GetProcessesByName("postgres").FirstOrDefault();
            //PropertyInfo pi = typeof(Process).GetProperty("Id");
            //Console.WriteLine($"{prc.ProcessName}: {pi.Name}: {pi.GetValue(prc)}");


            return null;
        }
    }
}
