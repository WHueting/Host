using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Host
{
    // Class representing repository, containing all methods to interact with repository
    class Repository
    {
        public string location_local_dir;
        public string location_repo;

        // Constructor
        public Repository() 
        {
            
        }

        // Method to initialize the repository
        public void Init() 
        {
            location_local_dir = ConfigurationManager.AppSettings.Get("locallocation");
            location_repo = FetchRepositories();
            CreateCommitList(location_repo);
        }

        // Method to get a textfile containing a commit as bytes
        // param hash; string containing hash of the commit to return
        // return; byte array representing a textfile of commit
        public byte[] GetCurrentCommitAsBytes(string hash) 
        {
            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.AddScript($"cd {location_repo}");
                powershell.AddScript($"git show {hash} > {location_local_dir}\\Commit_{hash}.txt");
                powershell.Invoke();
            }

            return File.ReadAllBytes(location_local_dir + "\\Commit_" + hash + ".txt");
        }

        // Method to get a file from the repository as bytes
        // param hash; string containing hash of commit
        // param pad; string containing path of file in commit
        public byte[] GetFileAsBytes(string hash, string pad) 
        {

            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.AddScript($"cd {location_repo}");
                powershell.AddScript($"git checkout {hash} {location_repo}\\{pad}");
                powershell.Invoke();

            }

            return File.ReadAllBytes(location_repo + "\\" + pad);
        }

        // Method to create a textfile of all commithashes
        // param path; string containing path of repository
        private static void CreateCommitList(string path)
        {
            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.AddScript($"sl " + path);
                powershell.AddScript($"git rev-list --all --remotes > commitlist.txt");
                powershell.Invoke();
            }
        }

        // Method to download a repository
        private string FetchRepositories()
        {
            string locatie_local_dir = ConfigurationManager.AppSettings.Get("locallocation");
            string git_url = ConfigurationManager.AppSettings.Get("java");
            string[] url = git_url.Split("/");
            string foldername = url[url.Length - 1].Substring(0, url[url.Length - 1].Length - 4);

            using (PowerShell powershell = PowerShell.Create())
            {
                powershell.AddScript($"sl " + locatie_local_dir);
                powershell.AddScript($"git clone " + git_url);
                powershell.Invoke();
            }

            return (locatie_local_dir + "/" + foldername);
        }

        // Method to return the time of commit of a commit
        // param hash; string containing hash of the commit
        // return commitTime; dateTime containing time of commit
        public DateTime GetDateTimeOfCommit(string hash)
        {

            DateTime commitTime = new DateTime();

            using (PowerShell powershell = PowerShell.Create())
            {

                powershell.AddScript($"cd {location_repo}");
                powershell.AddScript($@"git show -s --format=%ci {hash}");
                Collection<PSObject> results = powershell.Invoke();

                foreach (var result in results)
                {
                    string datetime = result.ToString();
                    commitTime = DateTime.Parse(datetime);
                }
            }

            return commitTime;
        }

        // Method to return the e-mail address of a commit
        // param hash; string containing hash of the commit
        // return; string containing e-mail address of commit
        public string GetEMailOfCommit(string hash)
        {
            string email = null;

            using (PowerShell powershell = PowerShell.Create())
            {

                powershell.AddScript($"cd {location_repo}");
                powershell.AddScript($@"git show -s --format='%ae' {hash}");
                Collection<PSObject> results = powershell.Invoke();

                foreach (var result in results)
                {
                    email = result.ToString();

                }
            }


            return email;
        }

        // Method to return the parents of a commit
        // param hash; string containing hash of the commit
        // param commits; the list of all commits
        // return; list of commits
        public List<Commit> GetParentsOfCommit(string hash, List<Commit> commits)
        {

            List<Commit> parents = new List<Commit>();

            using (PowerShell powershell = PowerShell.Create())
            {

                powershell.AddScript($"cd {location_repo}");
                powershell.AddScript($@"git log --pretty=%P -n 1 {hash}");
                Collection<PSObject> results = powershell.Invoke();

                foreach (var result in results)
                {
                    string[] parent_hashes = result.ToString().Split(' ');
                    foreach (var parent_hash in parent_hashes)
                    {
                        Commit ouder = commits.Find(Commit => Commit.Id.Equals(parent_hash));
                        if (ouder != null)
                        {
                            parents.Add(ouder);
                        }
                    }
                }
            }

            return parents;
        }
    }
}
