using System.Collections.Generic;

namespace Host
{
    // Class representing the list of actions to be send to the client
    class ActionList
    {
        private List<Action> actionlist;

        //Constructor
        public ActionList() 
        {
            actionlist = new List<Action>();
        }

        // Method for adding a merge action to the list
        // param ouder1; string containing name of user1
        // param ouder2; string containing name of user2 
        public void AddMerge(string ouder1, string ouder2) 
        {
            string message = "Merge_" + ouder1 + "_" + ouder2;
            Action action = new Action(message);
            actionlist.Add(action);
        }

        // Method for adding a commit action to the list
        // param hash; string containing hash of commit
        public void AddCommit(string hash) 
        {
            string message = "Commit_" + hash;
            Action action = new Action(message);
            actionlist.Add(action);
        }

        // Method for adding a branch action to the list
        // param name1; string containing name of repo to branch from
        // param name2; string containing name of repo to branch to
        public void AddBranch(string name1, string name2) 
        {
            string message = "Branch_" + name1 + "_" + name2;
            Action action = new Action(message);
            actionlist.Add(action);
            
        }

        // Method for adding a stop action to the list
        public void AddStop() 
        {
            string message = "Stop_";
            Action action = new Action(message);
            actionlist.Add(action);
        }

        // Method for adding a change workspace action to the list
        // param user; string containing name of workspace to change to
        public void AddChangeWorkspace(string user) 
        {
            string message = "ChangeWorkspace_" + user;
            Action action = new Action(message);
            actionlist.Add(action);
        }

        // Method for getting the size of the list
        public int GetCount() 
        {
            return actionlist.Count;
        }

        // Method for getting the next action to be executed by the client
        public string GetNextActionMessage() 
        {
            string nextMessage = actionlist[0].action;
            actionlist.RemoveAt(0);

            return nextMessage;

        }
    }
}
