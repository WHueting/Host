using System;
using System.Configuration;
using System.Management.Automation;

namespace Host
{
    // Class representing the main class of the client
    class Host
    {
        static void Main(string[] args)
        {
            Simulator simulator = new Simulator();
            simulator.Init();

            Console.WriteLine("Simulator ready, VM is started");

            StartVM();

            Listener listener = new Listener(simulator);
            listener.Listen();

            Console.WriteLine("Repository created");
        }

        static void StartVM()
        {
            string directory = ConfigurationManager.AppSettings.Get("rootdirectory_virtualbox");
            string path = ConfigurationManager.AppSettings.Get("directory_virtualbox");
            string vm_name = ConfigurationManager.AppSettings.Get("name_vm");

            using (PowerShell powershell = PowerShell.Create())
            {

                powershell.AddScript($@"{directory}");
                powershell.AddScript($@"{path}");
                powershell.AddScript($@"{vm_name}");
                powershell.Invoke();
            }
        }
    }
}
