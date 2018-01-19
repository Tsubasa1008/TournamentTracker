using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;
using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        /// <summary>
        /// Save a new prize to the database.
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique indentifier.</returns>
        public void CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.ConnString("Tournaments")))
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
            using (IDbConnection connection = new MySqlConnection(GlobalConfig.ConnString("Tournaments")))
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
            throw new NotImplementedException();
        }

        public void CreateTeam(TeamModel model)
        {
            throw new NotImplementedException();
        }

        public List<TeamModel> GetTeam_All()
        {
            throw new NotImplementedException();
        }

        public void CreateTournament(TournamentModel model)
        {
            throw new NotImplementedException();
        }

        public List<TournamentModel> GetTournament_All()
        {
            throw new NotImplementedException();
        }

        public void UpdateMatchup(MatchupModel model)
        {
            throw new NotImplementedException();
        }

        public void CompleteTournament(TournamentModel model)
        {
            throw new NotImplementedException();
        }
    }
}
