using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Host
{
    // Class representing the directed acyclic graph of repository
    class DAG
    { 
        public int commitcounter;
        public List<Commit> commitlist;

        // Constructor
        public DAG() 
        {

        }

        // Method to initialise the DAG
        // param repository; repository to create DAG from
        public void Init(Repository repository) 
        {
            commitcounter = 0;
            commitlist = CreateCommitList(repository);
        }

        // Method to create the DAG
        // param repository; repository to create DAG from
        public List<Commit> CreateCommitList(Repository repository)
        {
            List<Commit> commits = new List<Commit>();
            string path_commitlist = repository.location_repo + "\\commitlist.txt";

            foreach (var line in File.ReadLines(path_commitlist).Reverse())
            {
                Commit commit;

                if (!String.IsNullOrWhiteSpace(line))
                {
                    string hash = line.Trim();
                    DateTime commitTime = repository.GetDateTimeOfCommit(hash);
                    String email = repository.GetEMailOfCommit(hash);
                    List<Commit> ouders = repository.GetParentsOfCommit(hash, commits);
                    commit = new Commit(hash, commitTime, ouders, email);
                    commits.Add(commit);
                }
            }

            var orderedCommits = from c in commits orderby c.CommitTime select c;
            
            return orderedCommits.ToList();



        }

        // Method to increase counter for current commit
        public void IncreaseCounter() 
        {
            commitcounter++;
        }   

        // Method to get the size of the DAG
        public int GetLength() 
        {
            return commitlist.Count();
        }

        // Method to get the value of commitcounter
        public int GetCount()
        {
            return commitcounter;
        }

        // Method to get a commit from the DAG
        // param index; int representing the place of Commit in DAG
        public Commit GetCommit(int index) 
        {
            return commitlist[index];
        }

        // Method to get the commit the commitcounter is pointing to, current commit
        public Commit GetCurrentCommit() 
        {
            return commitlist[commitcounter];
        }


        // Method to get children of commit
        // param hash; string containing hash of commit whose children we want to find
        public List<Commit> GetChildrenOfCommit(string hash) 
        {
            List<Commit> children = new List<Commit>();

            foreach (Commit commit in commitlist)
            {
                if (commit.Parents != null)
                {
                    foreach (Commit parent in commit.Parents)
                    {
                        if (parent.Id.Equals(hash))
                        {
                            children.Add(commit);
                        }
                    }
                }

            }

            return children;
        }

        // Method to get the e-mail address of current commit
        public string GetEMailOfCurrentCommit() 
        {
            return commitlist[commitcounter].Email;
        }
    }


}
