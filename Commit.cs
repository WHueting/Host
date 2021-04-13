using System;
using System.Collections.Generic;

namespace Host
{
    // Class representing a commit with metadata
    class Commit
    {
        public string Id { get; }
        public List<Commit> Parents { get; set; }
        public string Email { get; set; }
        public DateTime CommitTime { get; }

        // Constructor
        public Commit()
        {

        }

        // Constructor
        // param hash; string containing hash of commit
        // param dateTime; dateTime containing time of commit
        // param parents; List of commits, representing the parents of commit
        // param email; string containing email address used in commit
        public Commit(string hash, DateTime dateTime, List<Commit> parents, string email)
        {
            Id = hash;
            CommitTime = dateTime;
            Parents = parents;
            Email = email;
        }
    }
}

