using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary;
using TrackerWPFUI.Models;

namespace TrackerWPFUI.ViewModels
{
    public class TournamentViewerViewModel : Screen
    {
        protected TournamentsTestContext db = new TournamentsTestContext();

        public Tournament Tournament { get; set; }
        private BindableCollection<int> _rounds = new BindableCollection<int>();
        private BindableCollection<Matchup> _matchups = new BindableCollection<Matchup>();
        private bool _unplayedOnly;
        private string _teamOne;
        private string _teamTwo;
        private double _teamOneScore;
        private double _teamTwoScore;
        private Matchup _selectedMatchup;
        private int _selectedRound;

        public TournamentViewerViewModel(Tournament model)
        {
            Tournament = db.Tournaments.Where(x => x.Id == model.Id).FirstOrDefault();

            TournamentName = model.TournamentName;

            Tournament.OnTournamentComplete += Tournament_OnTournamentComplete;

            LoadRounds();

            //LoadMatchups();
        }

        private void Tournament_OnTournamentComplete(object sender, DateTime e)
        {
            this.TryClose();
        }

        public BindableCollection<int> Rounds
        {
            get { return _rounds; }
            set { _rounds = value; }
        }

        public BindableCollection<Matchup> Matchups
        {
            get { return _matchups; }
            set { _matchups = value; }
        }

        public Matchup SelectedMatchup
        {
            get { return _selectedMatchup; }
            set
            {
                _selectedMatchup = value;
                NotifyOfPropertyChange(() => SelectedMatchup);
                LoadMatchup();
            }
        }

        public int SelectedRound
        {
            get { return _selectedRound; }
            set
            {
                _selectedRound = value;
                NotifyOfPropertyChange(() => SelectedRound);
                LoadMatchups();
            }
        }

        public bool UnplayedOnly
        {
            get { return _unplayedOnly; }
            set
            {
                _unplayedOnly = value;
                NotifyOfPropertyChange(() => UnplayedOnly);
                LoadMatchups();
            }
        }

        public string TeamOne
        {
            get { return _teamOne; }
            set
            {
                _teamOne = value;
                NotifyOfPropertyChange(() => TeamOne);
            }
        }

        public string TeamTwo
        {
            get { return _teamTwo; }
            set
            {
                _teamTwo = value;
                NotifyOfPropertyChange(() => TeamTwo);
            }
        }

        public double TeamOneScore
        {
            get { return _teamOneScore; }
            set
            {
                _teamOneScore = value;
                NotifyOfPropertyChange(() => TeamOneScore);
            }
        }

        public double TeamTwoScore
        {
            get { return _teamTwoScore; }
            set
            {
                _teamTwoScore = value;
                NotifyOfPropertyChange(() => TeamTwoScore);
            }
        }

        private string _tournamentName;

        public string TournamentName
        {
            get
            {
                return $"Tournament: { _tournamentName }";
            }
            set
            {
                _tournamentName = value;
                NotifyOfPropertyChange(() => TournamentName);
            }
        }


        /*public bool CanScoreMatch()
        {

        }*/

        public void ScoreMatch()
        {
            //Tournament.TournamentName = "Try Change The Record Name";
            //db.Entry(Tournament).State = System.Data.Entity.EntityState.Modified;
            //db.SaveChanges();

            for (int i = 0; i < SelectedMatchup.Entries.Count; i++)
            {
                if (i == 0)
                {
                    if (SelectedMatchup.Entries.First().TeamCompeting != null)
                    {
                        SelectedMatchup.Entries.First().Score = TeamOneScore;
                    }
                }

                if (i == 1)
                {
                    if (SelectedMatchup.Entries.Last().TeamCompeting != null)
                    {
                        SelectedMatchup.Entries.Last().Score = TeamTwoScore;
                    }
                }
            }

            Tournament.Matchups.Where(x => x.Id == SelectedMatchup.Id).First().Entries.First().Score = SelectedMatchup.Entries.First().Score;
            Tournament.Matchups.Where(x => x.Id == SelectedMatchup.Id).First().Entries.Last().Score = SelectedMatchup.Entries.Last().Score;

            try
            {
                TournamentLogic.UpdateTournamentResults(Tournament);
                db.Entry(Tournament).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"The application had the following error: { ex.Message }");
                return;
            }

            LoadMatchups();
        }

        private void LoadRounds()
        {
            Rounds.Clear();

            int maxRound = Tournament.Matchups.Select(x => x.MatchupRound).Max();
            for (int i = 1; i <= maxRound; i++)
            {
                Rounds.Add(i);
            }

            SelectedRound = Rounds.First();
        }

        private void LoadMatchups()
        {
            List<Matchup> matchups = Tournament.Matchups.Where(x => x.MatchupRound == SelectedRound).ToList();

            Matchups.Clear();

            foreach (Matchup m in matchups)
            {
                if (m.Winner == null || !UnplayedOnly)
                {
                    Matchups.Add(m);
                }
            }

            //foreach (List<Matchup> matchups in Tournament.Rounds)
            //{
            //    if (matchups.First().MatchupRound == SelectedRound)
            //    {
            //        Matchups.Clear();

            //        foreach (Matchup m in matchups)
            //        {
            //            if (m.Winner == null || !UnplayedOnly)
            //            {
            //                Matchups.Add(m);
            //            }
            //        }
            //    }
            //}

            if (Matchups.Count > 0)
            {
                SelectedMatchup = Matchups.First();
            }
        }

        private void LoadMatchup()
        {
            if (SelectedMatchup != null)
            {
                for (int i = 0; i < SelectedMatchup.Entries.Count; i++)
                {
                    if (i == 0)
                    {
                        if (SelectedMatchup.Entries.First().TeamCompeting != null)
                        {
                            TeamOne = SelectedMatchup.Entries.First().TeamCompeting.TeamName;
                            TeamOneScore = SelectedMatchup.Entries.First().Score.Value;

                            TeamTwo = "<bye>";
                            TeamTwoScore = 0;
                        }
                        else
                        {
                            TeamOne = "Not Yet Set";
                            TeamOneScore = 0;
                        }
                    }

                    if (i == 1)
                    {
                        if (SelectedMatchup.Entries.Last().TeamCompeting != null)
                        {
                            TeamTwo = SelectedMatchup.Entries.Last().TeamCompeting.TeamName;
                            TeamTwoScore = SelectedMatchup.Entries.Last().Score.Value;
                        }
                        else
                        {
                            TeamTwo = "Not Yet Set";
                            TeamTwoScore = 0;
                        }
                    }
                } 
            }
        }
    }
}
