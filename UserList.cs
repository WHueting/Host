using System;
using System.Collections.Generic;
using System.Text;

namespace Host
{
    // Class representing a list of users encountered
    class UserList
    {
        public User currentUser;
        public List<User> users;

        // Constructor
        public UserList() 
        {
        
        }

        // Method to initialize the list with a first user
        // param commit; commit containing the information of the first user
        public void Init(Commit commit) 
        {
            users = new List<User>();

            User firstUser = new User("User1", commit.Id, commit.Email);
            users.Add(firstUser);

            currentUser = firstUser;

        }
        
        // Method to get the current user
        // return user; the current user
        public User GetCurrentUser() 
        {
            return currentUser;
        }

        // Method to get a user by the id of the last commit
        // param hash; string containing hash of a commit
        public User GetUserByID(string hash) 
        {
            return users.Find(x => x.HuidigeCommit.Equals(hash));
        }

        // Method to add a user to the list
        // param id; string containing hash of commit
        // param email; string containing email of commit
        // retrun user; the created user
        public User AddUser(string id, string email) 
        {
            if (users.Exists(x => x.HuidigeCommit.Equals(id) && x.EMail.Equals(email)))
            {
                return null;
            }
            else 
            {
                int number = users.Count + 1;
                User newuser = new User("User" + number, id, email);
                users.Add(newuser);

                return newuser;
            }
        }

        
    }
}
