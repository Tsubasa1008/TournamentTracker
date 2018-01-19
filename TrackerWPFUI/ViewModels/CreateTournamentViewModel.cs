using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary;
using TrackerLibrary.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows.Controls;
using TrackerWPFUI.Validations;
using System.Data.Linq;
using System.Data;
using TrackerWPFUI.Models;

namespace TrackerWPFUI.ViewModels
{
    public class CreateTournamentViewModel : Conductor<object>.Collection.AllActive, IHandle<TeamModel>, IHandle<PrizeModel>, IHandle<Team>, IHandle<Prize>
    {
        protected TournamentsTestContext db = new TournamentsTestContext();

        private string _tournamentName;
        private decimal _entryFee;
        private BindableCollection<Team> _availableTeams;
        private Team _selectedTeamToAdd;
        private BindableCollection<Team> _selectedTeams = new BindableCollection<Team>();
        private Team _selectedTeamToRemove;
        private Screen _activeAddTeamView;
        private BindableCollection<Prize> _selectedPrizes = new BindableCollection<Prize>();
        private Prize _selectedPrizeToRemove;
        private Screen _activeAddPrizeView;
        private bool _selectedTeamsIsVisible = true;
        private bool _addTeamIsVisible = false;
        private bool _selectedPrizesIsVisible = true;
        private bool _addPrizeIsVisible = false;

        private string _emailAddress;

        [Required(ErrorMessage = "Email is required")]
        [EmailValidator(ErrorMessage = "The format of the email address is not valid")]
        [ValidationGroup(IncludeInErrorsValidation = false, GroupName = "ExcludeFromButton")]
        public string EmailAddress
        {
            get { return _emailAddress; }
            set
            {
                _emailAddress = value;
                NotifyOfPropertyChange(() => EmailAddress);
            }
        }
        public static int Errors { get; set; }

        private string _error = "test";

        public string Error
        {
            get { return _error; }
            set { _error = value; }
        }


        public CreateTournamentViewModel()
        {
            //Table<TeamModel> Teams = db.GetTable<TeamModel>();
            //AvailableTeams = new BindableCollection<TeamModel>(GlobalConfig.Connection.GetTeam_All());
            AvailableTeams = new BindableCollection<Team>(db.Teams.ToList());
            EventAggregationProvider.TrackerEventAggregator.Subscribe(this);
        }

        public string TournamentName
        {
            get { return _tournamentName; }
            set
            {
                _tournamentName = value;
                NotifyOfPropertyChange(() => TournamentName);
                NotifyOfPropertyChange(() => CanCreateTournament);
            }
        }

        [Required(ErrorMessage = "Field 'EntryFee' is required.")]
        public decimal EntryFee
        {
            get { return _entryFee; }
            set
            {
                _entryFee = value;
                NotifyOfPropertyChange(() => EntryFee);
            }
        }

        public BindableCollection<Team> AvailableTeams
        {
            get { return _availableTeams; }
            set { _availableTeams = value; }
        }

        public Team SelectedTeamToAdd
        {
            get { return _selectedTeamToAdd; }
            set
            {
                _selectedTeamToAdd = value;
                NotifyOfPropertyChange(() => SelectedTeamToAdd);
                NotifyOfPropertyChange(() => CanAddTeam);
            }
        }

        public BindableCollection<Team> SelectedTeams
        {
            get { return _selectedTeams; }
            set
            {
                _selectedTeams = value;
                NotifyOfPropertyChange(() => SelectedTeams);
                NotifyOfPropertyChange(() => CanCreateTournament);
            }
        }

        public Team SelectedTeamToRemove
        {
            get { return _selectedTeamToRemove; }
            set
            {
                _selectedTeamToRemove = value;
                NotifyOfPropertyChange(() => SelectedTeamToRemove);
                NotifyOfPropertyChange(() => CanRemoveTeam);
            }
        }

        public Screen ActiveAddTeamView
        {
            get { return _activeAddTeamView; }
            set
            {
                _activeAddTeamView = value;
                NotifyOfPropertyChange(() => ActiveAddTeamView);
            }
        }

        public BindableCollection<Prize> SelectedPrizes
        {
            get { return _selectedPrizes; }
            set
            {
                _selectedPrizes = value;
                NotifyOfPropertyChange(() => SelectedPrizes);
            }
        }

        public Prize SelectedPrizeToRemove
        {
            get { return _selectedPrizeToRemove; }
            set
            {
                _selectedPrizeToRemove = value;
                NotifyOfPropertyChange(() => SelectedPrizeToRemove);
                NotifyOfPropertyChange(() => CanRemovePrize);
            }
        }

        public Screen ActiveAddPrizeView
        {
            get { return _activeAddPrizeView; }
            set
            {
                _activeAddPrizeView = value;
                NotifyOfPropertyChange(() => ActiveAddPrizeView);
            }
        }

        public bool SelectedTeamsIsVisible
        {
            get { return _selectedTeamsIsVisible; }
            set
            {
                _selectedTeamsIsVisible = value;
                NotifyOfPropertyChange(() => SelectedTeamsIsVisible);
            }
        }

        public bool AddTeamIsVisible
        {
            get { return _addTeamIsVisible; }
            set
            {
                _addTeamIsVisible = value;
                NotifyOfPropertyChange(() => AddTeamIsVisible);
            }
        }

        public bool SelectedPrizesIsVisible
        {
            get { return _selectedPrizesIsVisible; }
            set
            {
                _selectedPrizesIsVisible = value;
                NotifyOfPropertyChange(() => SelectedPrizesIsVisible);
            }
        }

        public bool AddPrizeIsVisible
        {
            get { return _addPrizeIsVisible; }
            set
            {
                _addPrizeIsVisible = value;
                NotifyOfPropertyChange(() => AddPrizeIsVisible); 
            }
        }

        public bool CanAddTeam
        {
            get
            {
                return SelectedTeamToAdd != null;
            }
        }

        public void AddTeam()
        {
            SelectedTeams.Add(SelectedTeamToAdd);
            AvailableTeams.Remove(SelectedTeamToAdd);
            NotifyOfPropertyChange(() => CanCreateTournament);
        }

        public void CreateTeam()
        {
            ActiveAddTeamView = new CreateTeamViewModel();
            Items.Add(ActiveAddTeamView);

            SelectedTeamsIsVisible = false;
            AddTeamIsVisible = true;
        }

        public bool CanRemoveTeam
        {
            get
            {
                return SelectedTeamToRemove != null;
            }
        }

        public void RemoveTeam()
        {
            AvailableTeams.Add(SelectedTeamToRemove);
            SelectedTeams.Remove(SelectedTeamToRemove);
            NotifyOfPropertyChange(() => CanCreateTournament);
        }

        public void AddPrize()
        {
            ActiveAddPrizeView = new CreatePrizeViewModel();
            Items.Add(ActiveAddPrizeView);

            SelectedPrizesIsVisible = false;
            AddPrizeIsVisible = true;
        }

        public bool CanRemovePrize
        {
            get
            {
                return SelectedPrizeToRemove != null;
            }
        }

        public void RemovePrize()
        {
            SelectedPrizes.Remove(SelectedPrizeToRemove);
        }

        public bool CanCreateTournament
        {
            // TODO - Add logic for creating the tournament
            get
            {
                if (SelectedTeams != null)
                {
                    if (!String.IsNullOrWhiteSpace(TournamentName) && SelectedTeams.Count > 1)
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

        public void CreateTournament()
        {
            Tournament t = new Tournament
            {
                TournamentName = TournamentName,
                EntryFee = EntryFee
            };

            // Avoid Duplicate Data
            foreach (Team team in SelectedTeams)
            {
                t.EnteredTeams.Add(db.Teams.Where(x => x.Id == team.Id).FirstOrDefault());
            }

            // TODO - this

            //foreach (Prize prize in SelectedPrizes)
            //{
            //    t.Prizes.Add(db.Prizes.Where(x => x.Id == prize.Id).FirstOrDefault());
            //}

            CreateRounds(t);

            db.Tournaments.Add(t);
            db.SaveChanges();

            TournamentLogic.UpdateTournamentResults(t);

            db.Entry(t).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();

            EventAggregationProvider.TrackerEventAggregator.PublishOnUIThread(t);
            TryClose();

            /*tournaments t = new tournaments();
            t.TournamentName = TournamentName;
            t.EntryFee = EntryFee;

            foreach (teams team in SelectedTeams)
            {
                t.tournamententries.Add(new tournamententries { teams = team });
            }

            CreateRounds(t);
            db.tournaments.Add(t);
            
            db.SaveChanges();*/

            //// Create our tournament model
            //TournamentModel tm = new TournamentModel();

            //tm.TournamentName = TournamentName;
            //tm.EntryFee = EntryFee;
            //tm.Prizes = SelectedPrizes.ToList();
            //tm.EnteredTeams = SelectedTeams.ToList();

            //// Wire our matchups
            //TournamentLogic.CreateRounds(tm);

            //// Create Tournament entry
            //// Create all of the prizes entries
            //// Create all of the team entrie
            //GlobalConfig.Connection.CreateTournament(tm);

            //tm.AlertUsersToNewRound();

            //EventAggregationProvider.TrackerEventAggregator.PublishOnUIThread(tm);
            //this.TryClose();
        }

        public void Handle(TeamModel message)
        {
            //if (!String.IsNullOrWhiteSpace(message.TeamName))
            //{
            //    SelectedTeams.Add(message);
            //    NotifyOfPropertyChange(() => CanCreateTournament);
            //}

            //SelectedTeamsIsVisible = true;
            //AddTeamIsVisible = false;
        }

        public void Handle(PrizeModel message)
        {
            //if (!String.IsNullOrWhiteSpace(message.PlaceName))
            //{
                //SelectedPrizes.Add(message);
                //NotifyOfPropertyChange(() => CanCreateTournament);
            //}

            //SelectedPrizesIsVisible = true;
            //AddPrizeIsVisible = false;
        }

        public void Handle(Team message)
        {
            if (!String.IsNullOrWhiteSpace(message.TeamName))
            {
                SelectedTeams.Add(message);
                NotifyOfPropertyChange(() => CanCreateTournament);
            }

            SelectedTeamsIsVisible = true;
            AddTeamIsVisible = false;
        }

        public void Handle(Prize message)
        {
            if (!String.IsNullOrWhiteSpace(message.PlaceName))
            {
                SelectedPrizes.Add(message);
                NotifyOfPropertyChange(() => CanCreateTournament);
            }

            SelectedPrizesIsVisible = true;
            AddPrizeIsVisible = false;
        }

        private void CreateRounds(Tournament model)
        {
            List<Team> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams.ToList());
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);

            foreach (Matchup m in CreateFirstRound(byes, randomizedTeams))
            {
                model.Matchups.Add(m);
            }

            CreateOtherRounds(model, rounds);
        }

        private List<Team> RandomizeTeamOrder(List<Team> teams)
        {
            return teams.OrderBy(x => Guid.NewGuid()).ToList();
        }

        private int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;

            while (val < teamCount)
            {
                output += 1;
                val *= 2;
            }

            return output;
        }

        private int NumberOfByes(int rounds, int numberOfTeams)
        {
            int output = 0;
            int totalTeams = 1;

            for (int i = 1; i <= rounds; i++)
            {
                totalTeams *= 2;
            }

            output = totalTeams - numberOfTeams;

            return output;
        }

        private List<Matchup> CreateFirstRound(int byes, List<Team> teams)
        {
            List<Matchup> output = new List<Matchup>();
            Matchup curr = new Matchup();

            foreach (Team team in teams)
            {
                curr.Entries.Add(new MatchupEntry { TeamCompeting = team, Matchup = curr });

                if (byes > 0 || curr.Entries.Count > 1)
                {
                    curr.MatchupRound = 1;
                    output.Add(curr);
                    curr = new Matchup();

                    if (byes > 0)
                    {
                        byes -= 1;
                    }
                }
            }

            return output;
        }

        private void CreateOtherRounds(Tournament model, int rounds)
        {
            int round = 2;
            List<Matchup> previousRound = model.Matchups.Where(x => x.MatchupRound == 1).ToList();
            Matchup currMatchup = new Matchup();

            while (round <= rounds)
            {
                foreach (Matchup match in previousRound)
                {
                    currMatchup.Entries.Add(new MatchupEntry { Matchup = currMatchup, ParentMatchup = match });

                    if (currMatchup.Entries.Count > 1)
                    {
                        currMatchup.MatchupRound = round;
                        model.Matchups.Add(currMatchup);
                        currMatchup = new Matchup();
                    }
                }

                previousRound = model.Matchups.Where(x => x.MatchupRound == round).ToList();
                round++;
            }
        }
    }
}
