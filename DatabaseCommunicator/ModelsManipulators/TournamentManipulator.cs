using ChessTournamentManager.DataServices.Filters;
using DatabaseCommunicator.Models;
using Microsoft.IdentityModel.Tokens;
using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.TournamentHandlers;

namespace DatabaseCommunicator.ModelsManipulators
{
    public class TournamentManipulator : DatabaseModelManipulator
    {
        public TournamentManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings) { }

        /// <summary>
        /// Start the tournament. Sets current round to 1, mark tournament as HasStarted and set expected number of rounds
        /// </summary>
        /// <param name="tournamentId">Id of the tournament</param>
        /// <param name="expectedNumberOfRounds">How many rounds will be played in the tournament</param>
        /// <returns></returns>
        public async Task StartTournament(string tournamentId, int expectedNumberOfRounds)
        {

            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId,
                    ExpectedNumberOfRounds = expectedNumberOfRounds,
                    CurrentRound = 1,
                    HasStarted = true
                };


                var query = "MATCH(t:Tournament) WHERE t.Id = $TournamentId " +
                            "SET t.ExpectedNumberOfRounds = $ExpectedNumberOfRounds, " +
                            "t.CurrentRound = $CurrentRound, " +
                            "t.HasStarted = $HasStarted";

                await session.RunAsync(query, properties);
            }
        }

        /// <summary>
        /// Retreives basic information about the tournament
        /// </summary>
        /// <param name="tournamentId">Id of the tournament</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Tournament?> GetTournamentInformation(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                List<Tournament> tournaments = [];

                var query = "MATCH (t:Tournament) WHERE t.Id = $tournamentId RETURN t";

                IResultCursor result = await session.RunAsync(query, new { tournamentId = tournamentId });

                await result.ForEachAsync(record =>
                {
                    tournaments.Add(MappingFromDatabase.MapTournament(record["t"].As<INode>()));
                });

                if (tournaments.Count == 0)
                {
                    return null;
                }

                if (tournaments.Count != 1)
                {
                    throw new InvalidOperationException();
                }

                return tournaments.First();
            }
        }


        /// <summary>
        /// When new tournament is created by a user, this method stores this information into database
        /// </summary>
        /// <param name="newTournament">Tournament information to be stored</param>
        public async Task<string> CreateNewTournament(Tournament newTournament, string accountId)
        {

            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    Id = GetRandomId(),
                    TournamentName = newTournament.TournamentName,
                    ShortDescription = newTournament.ShortDescription,
                    StartDate = newTournament.StartDate,
                    EndDate = newTournament.EndDate,
                    CurrentRound = newTournament.CurrentRound,
                    ExpectedNumberOfRound = newTournament.ExpectedNumberOfRounds,
                    TournamentType = newTournament.TournamentType.ToString(),
                    HasStarted = newTournament.HasStarted,
                    IsTeam = newTournament.IsTeam
                };



                var query = "CREATE (tournament:Tournament $properties) " +
                            "WITH tournament " +
                            "MATCH (player:Player { AccountId : $AccountId }) " +
                            "CREATE (tournament)-[:MANAGED_BY]->(player)";

                await session.RunAsync(query, new { properties = properties, AccountId = accountId });

                return properties.Id;
            }
        }


        /// <summary>
        /// When we start a new round. We have to change current round number of a tournament stored in database. This method update tournament data
        /// so, tournament current round is up-to-date
        /// </summary>
        /// <param name="tournamentId">Id of a tournament for which we want to udpate current round</param>
        public async Task TournamentMovedToNextRound(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId
                };

                var query = "MATCH(t: Tournament { Id: $TournamentId}) " +
                            "SET t.CurrentRound = t.CurrentRound + 1";

                await session.RunAsync(query, properties);
            }
        }

        /// <summary>
        /// Returns all tournaments from the database which satisfy provided conditions
        /// </summary>
        /// <param name="filter">conditions which returned tournaments have to satisfy</param>
        /// <param name="limit">Maximum number of tournaments which will be returned</param>
        /// <returns></returns>
        public async Task<IEnumerable<Tournament>> GetAllTournaments(TournamentsFilter filter, int limit)
        {
            List<Tournament> tournaments = [];


            using (var session = _driver.AsyncSession())
            {

                var properties = new
                {
                    Name = filter?.Name ?? "",
                    StartDateFrom = filter?.StartDateFrom ?? new DateOnly(1, 1, 1),
                    StartDateTo = filter?.StartDateTo ?? new DateOnly(9999, 1, 1),
                    City = filter?.City ?? "",
                    Country = filter?.Country ?? "",
                    Limit = limit
                };

                string MatchAddress = ((filter?.City.IsNullOrEmpty() ?? true) && (filter?.Country.IsNullOrEmpty() ?? true)) ? "OPTIONAL " : "";

                var query = "MATCH (t:Tournament) " +
                            "WHERE t.TournamentName =~ '(?i).*' + $Name + '.*' " +
                            "AND date(t.StartDate) >= date($StartDateFrom) " +
                            "AND date(t.StartDate) <= date($StartDateTo) " +
                            "WITH t " +
                            MatchAddress + "MATCH (t)-[:HAS_ADDRESS]->(address:TournamentAddress) " +
                            "WHERE address.City =~ '(?i).*' + $City + '.*' " +
                            "AND address.Country =~ '(?i).*' + $Country + '.*' " +
                            "RETURN t " +
                            "LIMIT $Limit";

                var result = await session.RunAsync(query, properties);

                await result.ForEachAsync(record =>
                {
                    tournaments.Add(MappingFromDatabase.MapTournament(record["t"].As<INode>()));
                });

                return tournaments;
            }

        }

        /// <summary>
        /// Cancel registration of the team from the tournament
        /// </summary>
        /// <param name="tournamentId">Id of the tournament</param>
        /// <param name="teamId">Id of the team</param>
        /// <returns></returns>
        public async Task DeleteTeamFromTournamentAsync(string tournamentId, string teamId)
		{
			using (var session = _driver.AsyncSession())
			{
				var properties = new
				{
					TournamentId = tournamentId,
					TeamId = teamId
				};

				var query = "MATCH (p:Team {Id : $TeamId})-[relation:PLAYS_IN]->(t:Tournament {Id : $TournamentId}) " +
							"DELETE relation";

				await session.RunAsync(query, properties);
			}		
		}

        /// <summary>
        /// Returns all tournaments where the team is/was registered
        /// </summary>
        /// <param name="teamId">Id of the team</param>
        /// <returns></returns>
		public async Task<IEnumerable<Tournament>> GetTournamentsOfTeam(string teamId)
		{
			List<Tournament> tournaments = [];


			using (var session = _driver.AsyncSession())
			{
				var properties = new
				{
					TeamId = teamId
				};

				var query = "MATCH (p:Team {Id : $TeamId})-[relation:PLAYS_IN]->(t:Tournament) " +
							"RETURN t";

				var result = await session.RunAsync(query, properties);

				await result.ForEachAsync(record =>
				{
					tournaments.Add(MappingFromDatabase.MapTournament(record["t"].As<INode>()));
				});

				return tournaments;
			}
		}

        /// <summary>
        /// Returns all people which can manage the tournament
        /// </summary>
        /// <param name="tournamentId">Id of the tournament</param>
        /// <returns></returns>
		public async Task<IEnumerable<Player>> GetManagersOfTournaments(string tournamentId)
		{
			var properties = new
			{
				TournamentId = tournamentId
			};

			using (var session = _driver.AsyncSession())
			{
				var query = "MATCH (t:Tournament {Id : $TournamentId})-[:MANAGED_BY]->(manager:Player) RETURN manager";

				var result = await session.RunAsync(query, properties);

				return (await result.ToListAsync<Player>());
			}			
		}

        /// <summary>
        /// Delete whole tournament from the database
        /// </summary>
        /// <param name="tournamentId">Id of the tournament</param>
        /// <returns></returns>
		public async Task DeleteTournamentNodeAsync(string tournamentId)
		{
			var properties = new
			{
				TournamentId = tournamentId
			};

			using (var session = _driver.AsyncSession())
			{
				var query = "MATCH (tournament:Tournament {Id : $TournamentId}) " +
                            "DETACH DELETE tournament";

				await session.RunAsync(query, properties);
			}
		}
	}
}
