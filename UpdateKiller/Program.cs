using System;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace UpdateKiller
{
    public class Program
    {
        private static readonly string[] SvcList = {"wuauserv", "usoSvc", "bits"};

        public static bool IsAdministrator =>
            new WindowsPrincipal(WindowsIdentity.GetCurrent())
                .IsInRole(WindowsBuiltInRole.Administrator);

        public static void Main() => RunIndefiniteTask().Wait();

        private static async Task RunIndefiniteTask()
        {
            Console.Title = "Update Killer";

            if (!IsAdministrator)
            {
                Console.WriteLine("Update Killer needs to be run as administrator.");
                await Task.Delay(-1);
            }

            Console.Write("Enter monitoring delay: ");
            var userInput = Console.ReadLine();
            var isNumeric = int.TryParse(userInput, out var n);

            if (!isNumeric)
            {
                Console.Clear();
                Console.WriteLine("Please enter a numeric value.");
                await Task.Delay(2000);
                RunIndefiniteTask().Wait();
            }

            var delay = Convert.ToInt32(n);
            Console.Title += $" | Delay: {delay}";
            Console.Clear();

            while (true)
            {
                try
                {
                    foreach (var service in SvcList)
                    {
                        var sc = new ServiceController(service);

                        if (sc.Status != ServiceControllerStatus.Running) continue;

                        StopService(service, 2000);
                        Console.WriteLine($"Service: \"{service}\" was stopped.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex.Message}");
                }

                await Task.Delay(delay);
            }
        }

        private static void StopService(string serviceName, int timeoutMilliseconds)
        {
            var service = new ServiceController(serviceName);
            try
            {
                var timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}