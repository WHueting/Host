using System;
using System.Collections.Generic;
using System.Text;

namespace Host
{
    // Class representing a user or workspace
    class User
    {
        public string Name { get; set; }
        public string HuidigeCommit { get; set; }
        public string EMail { get; set; }

        // Constructor
        public User(string user, string hash, string email)
        {
            Name = user;
            HuidigeCommit = hash;
            EMail = email;
        }


    }
}
