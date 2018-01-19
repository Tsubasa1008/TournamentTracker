using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using TrackerLibrary;
using TrackerLibrary.Models;
using TrackerWPFUI.Models;

namespace TrackerWPFUI.ViewModels
{
    public class ShellViewModel : Conductor<object>, IHandle<TournamentModel>, IHandle<Tournament>
    {
        protected TournamentsTestContext db = new TournamentsTestContext();

        public ShellViewModel()
        { 
            // Initialize the database connections
            GlobalConfig.InitializeConnections(Database.MySQL);

            EventAggregationProvider.TrackerEventAggregator.Subscribe(this);

            //_existingTournaments = new BindableCollection<tournaments>(db.tournaments.ToList());
            //_existingTournaments = new BindableCollection<TournamentModel>(GlobalConfig.Connection.GetTournament_All());

            _existingTournaments = new BindableCollection<Tournament>(db.Tournaments.ToList());
        }

        public void CreateTournament()
        {
            ActivateItem(new CreateTournamentViewModel());
        } 

        public void LoadTournament()
        {
            if (SelectedTournament != null && !String.IsNullOrWhiteSpace(SelectedTournament.TournamentName))
            {
                ActivateItem(new TournamentViewerViewModel(SelectedTournament));
            }
        }

        public void Handle(TournamentModel message)
        {
            //// Open the tournaemnt viewer to the given tournament
            //if (!String.IsNullOrWhiteSpace(message.TournamentName))
            //{
            //    ExistingTournaments.Add(message);
            //    SelectedTournament = message;
            //}
        }

        public void Handle(Tournament message)
        {
            // Open the tournaemnt viewer to the given tournament
            if (!String.IsNullOrWhiteSpace(message.TournamentName))
            {
                ExistingTournaments.Add(message);
                SelectedTournament = message;
            }
        }

        /*public void Handle(tournaments message)
        {
            // Open the tournaemnt viewer to the given tournament
            if (!String.IsNullOrWhiteSpace(message.TournamentName))
            {
                ExistingTournaments.Add(message);
                SelectedTournament = message;
            }
        }*/

        private BindableCollection<Tournament> _existingTournaments;

        public BindableCollection<Tournament> ExistingTournaments
        {
            get { return _existingTournaments; }
            set { _existingTournaments = value; }
        }

        private Tournament _selectedTournament = new Tournament();

        public Tournament SelectedTournament
        {
            get { return _selectedTournament; }
            set
            {
                _selectedTournament = value;
                NotifyOfPropertyChange(() => SelectedTournament);

                LoadTournament();
            }
        }

    }
}
