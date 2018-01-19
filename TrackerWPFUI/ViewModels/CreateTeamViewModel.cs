using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary;
using TrackerLibrary.Models;
using TrackerWPFUI.Models;

namespace TrackerWPFUI.ViewModels
{
    public class CreateTeamViewModel : Conductor<object>, IHandle<PersonModel>, IHandle<People>
    {
        protected TournamentsTestContext db = new TournamentsTestContext();

        private string _teamName = "";
        private BindableCollection<People> _availableTeamMembers;
        private People _selectedTeamMemberToAdd;
        private BindableCollection<People> _selectedTeamMembers = new BindableCollection<People>();
        private People _selectedTeamMemberToRemove;
        private bool _selectedTeamMembersIsVisible = true;
        private bool _addPersonIsVisible = false;
        private string _errorMessage = "";

        public CreateTeamViewModel()
        {
            //AvailableTeamMembers = new BindableCollection<people>(db.people.ToList());
            //AvailableTeamMembers = new BindableCollection<PersonModel>(GlobalConfig.Connection.GetPerson_All());

            AvailableTeamMembers = new BindableCollection<People>(db.People.ToList());
            EventAggregationProvider.TrackerEventAggregator.Subscribe(this);
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public string TeamName
        {
            get { return _teamName; }
            set
            {
                _teamName = value;
                NotifyOfPropertyChange(() => TeamName);
                NotifyOfPropertyChange(() => CanCreateTeam);
            }
        }

        public bool SelectedTeamMembersIsVisible
        {
            get { return _selectedTeamMembersIsVisible; }
            set
            {
                _selectedTeamMembersIsVisible = value;
                NotifyOfPropertyChange(() => SelectedTeamMembersIsVisible);
            }
        }

        public bool AddPersonIsVisible
        {
            get { return _addPersonIsVisible; }
            set
            {
                _addPersonIsVisible = value;
                NotifyOfPropertyChange(() => AddPersonIsVisible);
            }
        } 

        public BindableCollection<People> AvailableTeamMembers
        {
            get { return _availableTeamMembers; }
            set
            {
                _availableTeamMembers = value;
            }
        }

        public People SelectedTeamMemberToAdd
        {
            get { return _selectedTeamMemberToAdd; }
            set
            {
                _selectedTeamMemberToAdd = value;
                NotifyOfPropertyChange(() => SelectedTeamMemberToAdd);
                NotifyOfPropertyChange(() => CanAddMember);
            }
        }

        public BindableCollection<People> SelectedTeamMembers
        {
            get { return _selectedTeamMembers; }
            set
            {
                _selectedTeamMembers = value;
                NotifyOfPropertyChange(() => CanCreateTeam);
            }
        }

        public People SelectedTeamMemberToRemove
        {
            get { return _selectedTeamMemberToRemove; }
            set
            {
                _selectedTeamMemberToRemove = value;
                NotifyOfPropertyChange(() => SelectedTeamMemberToRemove);
                NotifyOfPropertyChange(() => CanRemoveMember);
            }
        }

        public bool CanAddMember
        {
            get
            {
                return SelectedTeamMemberToAdd != null;
            }
        }

        public void AddMember()
        {
            SelectedTeamMembers.Add(SelectedTeamMemberToAdd);
            AvailableTeamMembers.Remove(SelectedTeamMemberToAdd);
            NotifyOfPropertyChange(() => CanCreateTeam);
        }

        public void CreateMember()
        {
            ActivateItem(new CreatePersonViewModel());
            SelectedTeamMembersIsVisible = false;
            AddPersonIsVisible = true;
        }

        public bool CanRemoveMember
        {
            get
            {
                return SelectedTeamMemberToRemove != null;
            }
        }

        public void RemoveMember()
        {
            AvailableTeamMembers.Add(SelectedTeamMemberToRemove);
            SelectedTeamMembers.Remove(SelectedTeamMemberToRemove);
            NotifyOfPropertyChange(() => CanCreateTeam);
        }

        public bool CanCreateTeam
        {
            get
            {
                if (SelectedTeamMembers != null)
                {
                    if (!string.IsNullOrWhiteSpace(TeamName) && SelectedTeamMembers.Count > 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public void CreateTeam()
        {
            /*teams t = new teams { TeamName = TeamName };

            foreach (people p in SelectedTeamMembers)
            {
                t.teammembers.Add(new teammembers { teams = t, people = p });
            }

            db.teams.Add(t);
            UpdateDB();

            EventAggregationProvider.TrackerEventAggregator.PublishOnUIThread(t);
            this.TryClose();*/

            /*TeamModel t = new TeamModel();

            t.TeamName = TeamName;
            t.TeamMembers = SelectedTeamMembers.ToList();

            GlobalConfig.Connection.CreateTeam(t);*/

            /*Team t = new Team
            {
                TeamName = TeamName,
                TeamMembers = SelectedTeamMembers
            };*/

            //db = new TournamentsTestContext();

            AvailableTeamMembers = new BindableCollection<People>(db.People.ToList());

            Team t = new Team();
            t.TeamName = TeamName;

            foreach (People p in SelectedTeamMembers)
            {
                t.TeamMembers.Add(db.People.Where(x => x.Id == p.Id).FirstOrDefault());
            }

            db.Teams.Add(t);
            UpdateDB();

            EventAggregationProvider.TrackerEventAggregator.PublishOnUIThread(t);
            this.TryClose();
        }

        public void Handle(PersonModel message)
        {
            /*if (!string.IsNullOrWhiteSpace(message.FirstName))
            {
                SelectedTeamMembers.Add(message);
                NotifyOfPropertyChange(() => CanCreateTeam);
            }

            SelectedTeamMembersIsVisible = true;
            AddPersonIsVisible = false;*/
        }

        public void CancelCreation()
        {
            EventAggregationProvider.TrackerEventAggregator.PublishOnUIThread(new Team());
            this.TryClose();
        }

        public void Handle(People message)
        {
            if (!string.IsNullOrWhiteSpace(message.FirstName))
            {
                SelectedTeamMembers.Add(message);
                NotifyOfPropertyChange(() => CanCreateTeam);
            }

            SelectedTeamMembersIsVisible = true;
            AddPersonIsVisible = false;
        }

        private async void UpdateDB()
        {
            try
            {
                await db.SaveChangesAsync();
                ErrorMessage = "Database Update Success!";
            }
            catch (Exception e)
            {
                ErrorMessage = $"Database Update Fail! ({ e.Message })";
            }
        }

        /*public void Handle(people message)
        {
            if (!string.IsNullOrWhiteSpace(message.FirstName))
            {
                SelectedTeamMembers.Add(message);
                NotifyOfPropertyChange(() => CanCreateTeam);
            }

            SelectedTeamMembersIsVisible = true;
            AddPersonIsVisible = false;
        }

        protected async void GetData()
        {
            var x = await db.people.ToListAsync();

            AvailableTeamMembers = new BindableCollection<people>(x);
        }

        private async void UpdateDB()
        {
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    
                }
                System.Windows.MessageBox.Show("There was a problem updating the database");
            }
        }*/
    }
}
