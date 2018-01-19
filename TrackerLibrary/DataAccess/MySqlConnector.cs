﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using MySql.Data.MySqlClient;
using Dapper;
using System.Data;

namespace TrackerLibrary.DataAccess
{
    public class MySqlConnector : IDataConnection
    {
        private const string db = "Tournaments";
        /// <summary>
        /// Save a new prize to the database.
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique indentifier.</returns>
        public void CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.ConnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("PlaceNumber", model.PlaceNumber);
                p.Add("PlaceName", model.PlaceName);
                p.Add("PrizeAmount", model.PrizeAmount);
                p.Add("PrizePercentage", model.PrizePercentage);
                p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("spPrizes_Insert", p, commandType: CommandType.StoredProcedure);
                model.Id = p.Get<int>("id");
            }
        }

        public void CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.ConnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("FirstName", model.FirstName);
                p.Add("LastName", model.LastName);
                p.Add("EmailAddress", model.EmailAddress);
                p.Add("CellphoneNumber", model.CellphoneNumber);
                p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("spPeople_Insert", p, commandType: CommandType.StoredProcedure);
                model.Id = p.Get<int>("id");
            }
        }

        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;

            using (IDbConnection connection = new MySqlConnection(GlobalConfig.ConnString(db)))
            {
                output = connection.Query<PersonModel>("spPeople_GetAll").ToList();
            }

            return output;
        }

        public void CreateTeam(TeamModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.ConnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("TeamName", model.TeamName);
                p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("spTeams_Insert", p, commandType: CommandType.StoredProcedure);
                model.Id = p.Get<int>("id");

                foreach (PersonModel tm in model.TeamMembers)
                {
                    p = new DynamicParameters();
                    p.Add("TeamId", model.Id);
                    p.Add("PersonId", tm.Id);
                    p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("spTeamMembers_Insert", p, commandType: CommandType.StoredProcedure);
                }
            }
        }

        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;

            using (IDbConnection connection = new MySqlConnection(GlobalConfig.ConnString(db)))
            {
                output = connection.Query<TeamModel>("spTeams_GetAll").ToList();

                foreach (TeamModel team in output)
                {
                    var p = new DynamicParameters();
                    p.Add("TeamId", team.Id);

                    team.TeamMembers = connection.Query<PersonModel>("spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                }
            }

            return output;
        }

        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.ConnString(db)))
            {
                SaveTournament(connection, model);

                SaveTournamentPrizes(connection, model);

                SaveTournamentEntries(connection, model);

                SaveTournamentRounds(connection, model);

                TournamentLogic.UpdateTournamentResults(model);
            }
        }

        public List<TournamentModel> GetTournament_All()
        {
            List<TournamentModel> output;

            using (IDbConnection connection = new MySqlConnection(GlobalConfig.ConnString(db)))
            {
                output = connection.Query<TournamentModel>("spTournaments_GetAll").ToList();
                var p = new DynamicParameters();

                foreach (TournamentModel t in output)
                {
                    // populate Prizes
                    p = new DynamicParameters();
                    p.Add("TournamentId", t.Id);
                    t.Prizes = connection.Query<PrizeModel>("spPrizes_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    // populate Teams
                    p = new DynamicParameters();
                    p.Add("TournamentId", t.Id);
                    t.EnteredTeams = connection.Query<TeamModel>("spTeams_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach (TeamModel team in t.EnteredTeams)
                    {
                        p = new DynamicParameters();
                        p.Add("TeamId", team.Id);
                        team.TeamMembers = connection.Query<PersonModel>("spTeamMembers_GetByTeam", p, commandType: CommandType.StoredProcedure).ToList();
                    }

                    // populate Rounds
                    p = new DynamicParameters();
                    p.Add("TournamentId", t.Id);
                    List<MatchupModel> matchups = connection.Query<MatchupModel>("spMatchups_GetByTournament", p, commandType: CommandType.StoredProcedure).ToList();

                    foreach (MatchupModel m in matchups)
                    {
                        p = new DynamicParameters();
                        p.Add("MatchupId", m.Id);
                        m.Entries = connection.Query<MatchupEntryModel>("spMatchupEntries_GetByMatchup", p, commandType: CommandType.StoredProcedure).ToList();

                        // Populate each entry (2 models)
                        // Populate each matchup (1 model)

                        List<TeamModel> allTeams = GetTeam_All();

                        if (m.WinnerId > 0)
                        {
                            m.Winner = allTeams.Where(x => x.Id == m.WinnerId).First();
                        }

                        foreach (MatchupEntryModel me in m.Entries)
                        {
                            if (me.TeamCompetingId > 0)
                            {
                                me.TeamCompeting = allTeams.Where(x => x.Id == me.TeamCompetingId).First();
                            }

                            if (me.ParentMatchupId > 0)
                            {
                                // because order by round, so the parent already complete in the matchups
                                me.ParentMatchup = matchups.Where(x => x.Id == me.ParentMatchupId).First();
                            }
                        }
                    }

                    // List<List<MatchupModel>>
                    List<MatchupModel> currRow = new List<MatchupModel>();
                    int currRound = 1;
                    foreach (MatchupModel m in matchups)
                    {
                        if (m.MatchupRound > currRound)
                        {
                            t.Rounds.Add(currRow);
                            currRow = new List<MatchupModel>();
                            currRound++;
                        }

                        currRow.Add(m);
                    }

                    t.Rounds.Add(currRow);
                }
            }

            return output;
        }

        private void SaveTournament(IDbConnection connection, TournamentModel model)
        {
            var p = new DynamicParameters();
            p.Add("TournamentName", model.TournamentName);
            p.Add("EntryFee", model.EntryFee);
            p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute("spTournaments_Insert", p, commandType: CommandType.StoredProcedure);
            model.Id = p.Get<int>("id");
        }

        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
        {
            foreach (PrizeModel pz in model.Prizes)
            {
                var p = new DynamicParameters();
                p.Add("TournamentId", model.Id);
                p.Add("PrizeId", pz.Id);
                p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("spTournamentPrizes_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentEntries(IDbConnection connection, TournamentModel model)
        {
            foreach (TeamModel tm in model.EnteredTeams)
            {
                var p = new DynamicParameters();
                p.Add("TournamentId", model.Id);
                p.Add("TeamId", tm.Id);
                p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute("spTournamentEntries_Insert", p, commandType: CommandType.StoredProcedure);
            }
        }

        private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
        {
            // List<List<MatchupModel>> Rounds
            // List<MatchupEntryModel> Entries

            // Loop through the rounds
            // Loop through the matchup
            // Save the matchup
            // Loop through the entries and save them

            foreach (List<MatchupModel> round in model.Rounds)
            {
                foreach (MatchupModel matchup in round)
                {
                    var p = new DynamicParameters();
                    p.Add("TournamentId", model.Id);
                    p.Add("MatchupRound", matchup.MatchupRound);
                    p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute("spMatchups_Insert", p, commandType: CommandType.StoredProcedure);
                    matchup.Id = p.Get<int>("id");

                    foreach (MatchupEntryModel entry in matchup.Entries)
                    {
                        p = new DynamicParameters();
                        p.Add("MatchupId", matchup.Id);
                        if (entry.ParentMatchup == null)
                        {
                            p.Add("ParentMatchupId", null);
                        }
                        else
                        {
                            p.Add("ParentMatchupId", entry.ParentMatchup.Id);
                        }
                        if (entry.TeamCompeting == null)
                        {
                            p.Add("TeamCompetingId", null);
                        }
                        else
                        {
                            p.Add("TeamCompetingId", entry.TeamCompeting.Id);
                        }
                        p.Add("id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                        connection.Execute("spMatchupEntries_Insert", p, commandType: CommandType.StoredProcedure);
                    }
                }
            }
        }

        public void UpdateMatchup(MatchupModel model)
        {

            using (IDbConnection connection = new MySqlConnection(GlobalConfig.ConnString(db)))
            {
                // spMatchups_Update, p = (id, WinnerId);
                var p = new DynamicParameters();
                if (model.Winner != null)
                {
                    p.Add("id", model.Id);
                    p.Add("WinnerId", model.Winner.Id);

                    connection.Execute("spMatchups_Update", p, commandType: CommandType.StoredProcedure); 
                }

                //spMatchupEntries_Update, id, TeamCompetingId, Score
                foreach (MatchupEntryModel me in model.Entries)
                {
                    if (me.TeamCompeting != null)
                    {
                        p = new DynamicParameters();
                        p.Add("id", me.Id);
                        p.Add("TeamCompetingId", me.TeamCompeting.Id);
                        p.Add("Score", me.Score);

                        connection.Execute("spMatchupEntries_Update", p, commandType: CommandType.StoredProcedure); 
                    }
                }
            }
        }

        public void CompleteTournament(TournamentModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.ConnString(db)))
            {
                var p = new DynamicParameters();
                p.Add("id", model.Id);

                connection.Execute("spTournaments_Complete", p, commandType: CommandType.StoredProcedure);
            }
        }
    }
}
