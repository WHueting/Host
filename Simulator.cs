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
using System.Threading;
using System.Threading.Tasks;
namespace Host
{
    // Class representing the host, containing all methods to control the simulation
    class Simulator
    {
        private Repository repository;
        private DAG dag;
        private UserList users;
        private ActionList actions;

        // Constructor
        public Simulator()
        {

        }

        // Method to initialize the simulator
        public void Init()
        {
            repository = new Repository();
            repository.Init();

            Console.WriteLine("Repository ready");

            dag = new DAG();
            dag.Init(repository);

            Console.WriteLine("DAG ready");

            users = new UserList();
            users.Init(dag.GetCommit(0));

            Console.WriteLine("Userlist ready");

            actions = new ActionList();
            actions = CreateNewActionList();

            Console.WriteLine("Actionlist ready");



        }
        // Method to get the textfile containing the current commit as bytes
        // return bytes of textfile
        public byte[] GetCurrentCommitAsBytes() 
        {
            string hash = dag.GetCurrentCommit().Id;

            return repository.GetCurrentCommitAsBytes(hash);

        }

        // Method to get the file requested as bytes
        // param pad; string containing path of file to get
        // return bytes of file
        public byte[] GetFileAsBytes(string pad)
        {
            string hash = dag.GetCurrentCommit().Id;

            return repository.GetFileAsBytes(hash, pad);

        }

        // Method to get the next action to be executed
        // return; string containing message of action
        public string NextAction()
        {
            string nextActionMessage = "";

            if (actions.GetCount() == 0)
            {
                dag.IncreaseCounter();
                actions = CreateNewActionList();
            }

            nextActionMessage = actions.GetNextActionMessage();

            return nextActionMessage;
        }


        // Method to create a new list of actions to be exectuted
        // return; list of actions to execute
        public ActionList CreateNewActionList()
        {
            ActionList actionlist = new ActionList();

            actionlist = CreateNewCommitActions(actionlist);
            actionlist = CreateUpdateUsersActions(actionlist);

            return actionlist;
        }
        // Method to get actions concerning the commit
        // param actionlist; actionlist to add the actions
        // return actionlist; actionlist containing all actions to be executed
        public ActionList CreateNewCommitActions(ActionList actionlist)
        {

            if (dag.GetLength() == dag.GetCount())
            {
                actionlist.AddStop();
            }
            else
            {
                Commit commit = dag.GetCurrentCommit();


                User currentuser = users.GetUserByID(commit.Id); 

                    if (currentuser != users.currentUser)
                    {
                        users.currentUser = currentuser;
                        actionlist.AddChangeWorkspace(users.currentUser.Name);
                    }
                

                if (commit.Parents.Count == 2)
                {
                    string nameOuder1;
                    string nameOuder2;
                    User ouder1 = users.GetUserByID(commit.Parents[0].Id);
                    if (ouder1 == null)
                    {
                        nameOuder1 = "Huidig";
                    }
                    else
                    {
                        nameOuder1 = ouder1.Name;
                    }

                    User ouder2 = users.GetUserByID(commit.Parents[1].Id);
                    if (ouder2 == null)
                    {
                        nameOuder2 = "Huidig";
                    }
                    else
                    {
                        nameOuder2 = ouder2.Name;
                    }

                    actionlist.AddMerge(nameOuder1, nameOuder2);



                }
                else
                {
                    actionlist.AddCommit(commit.Id);

                }

            }
            return actionlist;
        }

        // Method to get list of actions concerning users and workspaces
        // param actionlist; actionlist to add the actions
        // return actionlist; actionlist containing all actions to be executed
        public ActionList CreateUpdateUsersActions(ActionList actionlist)
        {
            if (dag.GetLength() != dag.GetCount())
            {
                List<Commit> children = dag.GetChildrenOfCommit(dag.GetCurrentCommit().Id);

                Boolean mastergevonden = false;
                User newuser = users.GetCurrentUser();

                if (children.Count == 1)
                {
                    if (users.GetUserByID(children[0].Id) == null)
                    {
                        users.currentUser.HuidigeCommit = children[0].Id;
                    }
                }

                if (children.Count > 1)
                {
                    foreach (Commit child in children)
                    {
                        if (child.Email.Equals(dag.GetEMailOfCurrentCommit()) && !mastergevonden)
                        {
                            mastergevonden = true;
                            users.currentUser.HuidigeCommit = child.Id;
                        }
                        else
                        {
                            User addedUser = users.AddUser(child.Id, child.Email);


                            if (addedUser != null)
                            {
                                actionlist.AddBranch(users.GetCurrentUser().Name, addedUser.Name);
                            }

                        }
                    }
                }
                users.currentUser = newuser;
            }

            return actionlist;
        }

        

    }


}
