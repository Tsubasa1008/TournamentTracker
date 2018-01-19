using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerWPFUI.Models;
using System.Configuration;
using TrackerLibrary;

namespace TrackerWPFUI
{
    public static class TournamentLogic
    {
        public static void UpdateTournamentResults(Tournament model)
        {
            int startingRound = model.CheckCurrentRound();

            List<Matchup> toScore = new List<Matchup>();

            foreach (Matchup m in model.Matchups)
            {
                if (m.Winner == null && (m.Entries.Any(x => x.Score != 0) || m.Entries.Count == 1))
                {
                    toScore.Add(m);
                }
            }

            MarkWinnerInMatchups(toScore);

            AdvanceWinners(toScore, model);

            int endingRound = model.CheckCurrentRound();

            if (endingRound > startingRound)
            {
                model.AlertUsersToNewRound();
            }
        }

        public static void AlertUsersToNewRound(this Tournament model)
        {
            int currentRoundNumber = model.CheckCurrentRound();

            List<Matchup> currentRound = model.Matchups.Where(x => x.MatchupRound == currentRoundNumber).ToList();

            List<string> to = new List<string>();

            foreach (Matchup matchup in currentRound)
            {
                foreach (MatchupEntry me in matchup.Entries)
                {
                    foreach (People p in me.TeamCompeting.TeamMembers)
                    {
                        AlertPersonToNewRound(p, me.TeamCompeting.TeamName, matchup.Entries.Where(x => x.TeamCompeting != me.TeamCompeting).FirstOrDefault());
                    }
                }
            }
        }

        private static void AlertPersonToNewRound(People p, string teamName, MatchupEntry competitor)
        {
            if (p.EmailAddress.Length == 0)
            {
                return;
            }

            string to = "";
            string subject = "";
            StringBuilder body = new StringBuilder();

            if (competitor != null)
            {
                subject = $"You have a new matchup with { competitor.TeamCompeting.TeamName }";

                body.AppendLine("<h1>You have a new mathcup</h1>");
                body.Append("<strong>Competitor: </strong>");
                body.Append(competitor.TeamCompeting.TeamName);
                body.AppendLine();
                body.AppendLine();
                body.AppendLine("Have a great time!");
                body.AppendLine("~Tournament Tracker");
            }
            else
            {
                subject = $"You have a bye week this round";

                body.AppendLine("Enjoy your round off!");
                body.AppendLine("~Tournament Tracker");
            }

            to = p.EmailAddress;

            EmailLogic.SendEmail(to, subject, body.ToString());
        }

        private static void AdvanceWinners(List<Matchup> models, Tournament tournament)
        {
            foreach (Matchup m in models)
            {
                foreach (Matchup rm in tournament.Matchups)
                {
                    foreach (MatchupEntry me in rm.Entries)
                    {
                        if (me.ParentMatchup != null)
                        {
                            if (me.ParentMatchupId == m.Id)
                            {
                                me.TeamCompeting = m.Winner;
                            }
                        }
                    }
                }
            }
        }

        private static void MarkWinnerInMatchups(List<Matchup> models)
        {
            // greater or lesser
            string greaterWins = ConfigurationManager.AppSettings["greaterWins"];

            foreach (Matchup m in models)
            {
                // Checks for bye week entry
                if (m.Entries.Count == 1)
                {
                    m.Winner = m.Entries.First().TeamCompeting;
                    continue;
                }

                if (greaterWins == "0")
                {
                    // 0 means false, or low score wins
                    if (m.Entries.First().Score < m.Entries.Last().Score)
                    {
                        m.Winner = m.Entries.First().TeamCompeting;
                    }
                    else if (m.Entries.Last().Score < m.Entries.First().Score)
                    {
                        m.Winner = m.Entries.Last().TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("We do not allow ties in this application.");
                    }
                }
                else
                {
                    // 1 means true, or high score wins
                    // 0 means false, or low score wins
                    if (m.Entries.First().Score > m.Entries.Last().Score)
                    {
                        m.Winner = m.Entries.First().TeamCompeting;
                    }
                    else if (m.Entries.Last().Score > m.Entries.First().Score)
                    {
                        m.Winner = m.Entries.Last().TeamCompeting;
                    }
                    else
                    {
                        throw new Exception("Wd do not allow ties in this application.");
                    }
                }
            }
        }

        private static int CheckCurrentRound(this Tournament model)
        {
            int output = 1;
            int maxRound = model.Matchups.Select(x => x.MatchupRound).Max();

            for (int i = 1; i <= maxRound; i++)
            {
                List<Matchup> round = model.Matchups.Where(x => x.MatchupRound == i).ToList();

                if (round.All(x => x.Winner != null))
                {
                    output += 1;
                }
                else
                {
                    return output;
                }
            }

            CompleteTournament(model);

            return output - 1;
        }

        private static void CompleteTournament(Tournament model)
        {
            model.Active = false;

            Team winners = model.Matchups.Last().Winner;
            Team runnerUp = model.Matchups.Last().Entries.Where(x => x.TeamCompeting != winners).First().TeamCompeting;

            decimal winnerPrize = 0;
            decimal runnerUpPrize = 0;
            
            // TODO - this

            //if (model.Prizes.Count > 0)
            //{
            //    decimal totalIncome = model.EnteredTeams.Count * model.EntryFee;

            //    Prize firstPlacePrize = model.Prizes.Where(x => x.PlaceNumber == 1).FirstOrDefault();
            //    Prize secondPlacePrize = model.Prizes.Where(x => x.PlaceNumber == 2).FirstOrDefault();

            //    if (firstPlacePrize != null)
            //    {
            //        winnerPrize = firstPlacePrize.CalculatePrizePayout(totalIncome);
            //    }

            //    if (secondPlacePrize != null)
            //    {
            //        runnerUpPrize = secondPlacePrize.CalculatePrizePayout(totalIncome);
            //    }
            //}

            // Send Email to all tournament
            string subject = "";
            StringBuilder body = new StringBuilder();

            subject = $"In { model.TournamentName }, { winners.TeamName } has own!";

            body.AppendLine("<h1>We have a WINNER!</h1>");
            body.AppendLine("<p>Congratulations to our winner on a great tournament.</p>");
            body.AppendLine("<br />");

            if (winnerPrize > 0)
            {
                body.AppendLine($"<p>{ winners.TeamName } will receive ${ winnerPrize }</p>");
            }

            if (runnerUpPrize > 0)
            {
                body.AppendLine($"<p>{ runnerUp.TeamName } will receive ${ runnerUpPrize }");
            }

            body.AppendLine("<p>Thanks for a great tournament everyone!</p>");
            body.AppendLine("~Tournament Tracker");

            List<string> bcc = new List<string>();

            foreach (Team t in model.EnteredTeams)
            {
                foreach (People p in t.TeamMembers)
                {
                    if (p.EmailAddress.Length > 0)
                    {
                        bcc.Add(p.EmailAddress);
                    }
                }
            }

            EmailLogic.SendEmail(new List<string>(), bcc, subject, body.ToString());
            
            // TODO - Complete Tournament
        }

        private static decimal CalculatePrizePayout(this Prize prize, decimal totalIncome)
        {
            decimal output = 0;

            if (prize.PrizeAmount > 0)
            {
                output = prize.PrizeAmount;
            }
            else
            {
                output = decimal.Multiply(totalIncome, Convert.ToDecimal(prize.PrizePercentage / 100));
            }

            return output;
        }
    }
}
