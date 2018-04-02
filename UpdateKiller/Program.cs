using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Security.Principal;

namespace UpdateKiller
{
    class Program
    {
        static List<string> svcList = new List<string>(new string[] {"wuauserv","usoSvc","bits"});
        static void Main(string[] args)
        {
            Console.Write("Enter monitoring delay: ");
            int delayVal = Convert.ToInt32(Console.ReadLine());

            if (IsAdministrator() == false)
            {
                Console.WriteLine("Update Killer needs to be run as administrator.");
                Console.ReadKey();
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("Update Killer is running as administrator and monitoring update services.");
            }

            while (true)
            {
                try
                {
                    foreach (string svcName in svcList)
                    {
                        ServiceController sc = new ServiceController(svcName);
                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            System.Diagnostics.Process.Start("CMD.exe", "/C net stop " + svcName);
                            Console.WriteLine("Service " + svcName + " was found and killed.");
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("An error occured.");
                }
                System.Threading.Thread.Sleep(delayVal);
            }
            
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
