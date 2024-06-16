using DatabaseCommunicator.Models;
using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using ChessTournamentManager.DataServices.Filters;
using TournamentLibrary.TournamentHandlers;
using System.Text;

namespace DatabaseCommunicator.ModelsManipulators
{
	public class ProfileManipulator : DatabaseModelManipulator
    {

        public ProfileManipulator(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings)
        { 
        
        }

        /// <summary>
        /// Updates information about user in the database
        /// </summary>
        /// <param name="player">New values from the user</param>
        /// <returns></returns>
        public async Task UpdatePlayerProfile(Player player)
        {
            if (player.Id == null)
            {
                player.Id = GetRandomId();
            }


            using (var session = _driver.AsyncSession())
            {

                var properties = new
                {
                    Id = player.Id,
                    AccountId = player.AccountId,
                    FirstName = player.FirstName,
                    MiddleName = player.MiddleName,
                    LastName = player.LastName,
                    DateOfBirth = player.DateOfBirth,
                    ChessClub = player.ChessClub,
                    FideId = player.FideId,
                    Elo = player.Elo,
                    ProfilePictureName = player.ProfilePictureName,
                    Country = player.Country
                };


                var query = "MERGE (p:Player {Id: $Id}) SET p = $properties";

                await session.RunAsync(query, new { Id = player.Id, properties = properties });
            }
        }

        /// <summary>
        /// Retreives information about user from the database
        /// </summary>
        /// <param name="accountId">AccountId of the user</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Player?> GetAcountInformation(string accountId)
        {
            using (var session = _driver.AsyncSession())
            {
                List<Player> players = [];

                var query = "MATCH (p:Player) WHERE p.AccountId = $AccountId RETURN p";

                IResultCursor result = await session.RunAsync(query, new { AccountId = accountId });

                players = (await result.ToListAsync<Player>()).ToList();
               

                if (players.Count > 1)
                {
                    throw new InvalidOperationException();
                }
                if (players.Count == 0)
                {
                    return null;
                }


                return players.First();
            }
        }

        /// <summary>
        /// Retreives information about a person from database
        /// </summary>
        /// <param name="id">Id of the person</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<Player?> GetPlayerInformation(string id)
        {
            using (var session = _driver.AsyncSession())
            {
                List<Player> players = [];

                var query = "MATCH (p:Player) WHERE p.Id = $Id RETURN p";

                IResultCursor result = await session.RunAsync(query, new { Id = id });


				players = (await result.ToListAsync<Player>()).ToList();

				if (players.Count > 1)
                {
                    throw new InvalidOperationException();
                }
                if (players.Count == 0)
                {
                    return null;
                }


                return players.First();
            }
        }

        /// <summary>
        /// Retreives people from database, which have created an accout. Results are filtered based on provided conditions
        /// </summary>
        /// <param name="filter">Conditions which returned people has to pass</param>
        /// <param name="skipCount">How many people from the start should not be included in the result</param>
        /// <param name="takeCount">How many people should be returned</param>
        /// <returns></returns>
        public async Task<IEnumerable<Player>> GetAllPlayersAsync(PlayerServiceFilter filter, long skipCount = 0, int takeCount = 20)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    FirstName = filter.FirstName ?? "",
                    LastName = filter.LastName ?? "",
                    ClubName = filter.ClubName ?? "",

                    Skip = skipCount,
                    Take = takeCount
                };


                StringBuilder queryBuilder = new("MATCH (p:Player) " +
                            "WHERE p.FirstName =~ '(?i).*' + $FirstName + '.*' " +
                            "AND p.LastName =~ '(?i).*' + $LastName + '.*' " +
                            "AND p.AccountId IS NOT NULL ");

				if (properties.ClubName != "")
				{
					queryBuilder.Append("AND (p.ChessClub =~ '(?i).*' + $ClubName + '.*') ");
				}

                queryBuilder.Append("RETURN p " +
                                    "ORDER BY p.Id " +
                                    "SKIP $Skip " +
                                    "LIMIT $Take ");

	
                var result = await session.RunAsync(queryBuilder.ToString(), properties);

                return await result.ToListAsync<Player>();
            }
        }

        /// <summary>
        /// Returns all tournament where the player played
        /// </summary>
        /// <param name="playerId">Id of the player</param>
        /// <returns></returns>
        public async Task<List<Tournament>> GetPlayersTournament(string playerId)
        {
            List<Tournament> tournaments = [];

            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (t:Tournament)<-[:PLAYS_IN]-(:Player {Id: $Id}) " +
                            "RETURN t " +
                            "UNION " +
							"MATCH (t:Tournament)<-[:PLAYS_IN]-(:Team)<-[:PLAYS_FOR]-(:Player {Id: $Id})  " +
							"RETURN t";

                var result = await session.RunAsync(query, new { Id = playerId });

                await result.ForEachAsync(record =>
                {
                    tournaments.Add(MappingFromDatabase.MapTournament(record["t"].As<INode>()));
                });

                return tournaments;
            }
        }

        /// <summary>
        /// Returns id of the person based on provided accountId
        /// </summary>
        /// <param name="accountId">AccountId of the person</param>
        /// <returns></returns>
        public async Task<string?> GetIdOfAccount(string accountId)
        {
			using (var session = _driver.AsyncSession())
			{
				var query = "MATCH (p:Player {AccountId: $AccountId}) " +
							"RETURN p.Id AS id";

				var result = await session.RunAsync(query, new { AccountId = accountId });

                var id = new List<string>();

				await result.ForEachAsync(record =>
				{
                    id.Add(record["id"].As<string>());
				});

                if(id.Count == 0)
                {
                    return null;
                }

                return id[0];
			}
		}

        /// <summary>
        /// Returns how many people in the database satisfy the provided conditions
        /// </summary>
        /// <param name="filter">Provided conditions</param>
        /// <returns></returns>
        public async Task<long> GetTotalNumberOfPlayers(PlayerServiceFilter filter)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    FirstName = filter.FirstName ?? "",
                    LastName = filter.LastName ?? "",
                    ClubName = filter.ClubName ?? "",
                };


                StringBuilder queryBuilder = new("MATCH (p:Player) " +
                            "WHERE p.FirstName =~ '(?i).*' + $FirstName + '.*' " +
                            "AND p.LastName =~ '(?i).*' + $LastName + '.*' " +
                            "AND p.AccountId IS NOT NULL ");

                if(properties.ClubName != "")
                {
					queryBuilder.Append("AND (p.ChessClub =~ '(?i).*' + $ClubName + '.*') ");
                }

				queryBuilder.Append("RETURN COUNT(p) AS numberOfPlayers;");


                var result = await session.RunAsync(queryBuilder.ToString(), properties);

                await foreach(var record in result)
                {
                    return record["numberOfPlayers"].As<long>();
                }

                return 0;
            }
        }

        /// <summary>
        /// Returns if the user marked the tournament as favourite or not
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="tournamentId">Id of the tournament</param>
        /// <returns></returns>
        public async Task<bool> GetTournamentFavouritness(string id, string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    Id = id,
                    TournamentId = tournamentId
                };


                var query = "MATCH (player:Player {Id : $Id})-[:LIKES]->(tournament:Tournament {Id : $TournamentId}) " +
                            "RETURN tournament";


                var result = await session.RunAsync(query, properties);

                return (await result.ToListAsync()).Any();
            }
        }

        /// <summary>
        /// Saves the information, that the user marked the tournament as favourite
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="tournamentId">Id of the tournament</param>
        /// <returns></returns>
        public async Task MarkTournamentAsFavourite(string id, string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    Id = id,
                    TournamentId = tournamentId
                };


                var query = "MATCH (player:Player {Id : $Id}) " +
                            "MATCH (tournament:Tournament {Id : $TournamentId}) " +
                            "CREATE (player)-[:LIKES]->(tournament) ";

                await session.RunAsync(query, properties);
            }
        }

        /// <summary>
        /// Saves the information, that the user unmarked the tournament as favourite
        /// </summary>
        /// <param name="id">Id of the user</param>
        /// <param name="tournamentId">Id of the tournament</param>
        /// <returns></returns>
        public async Task UnMarkTournamentAsFavourite(string id, string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    Id = id,
                    TournamentId = tournamentId
                };


                var query = "MATCH (player:Player {Id : $Id})-[relation:LIKES]->(tournament:Tournament {Id : $TournamentId}) " +
                            "DELETE relation";

                await session.RunAsync(query, properties);
            }
        }

        /// <summary>
        /// Returns all tournaments which person marked as favourite
        /// </summary>
        /// <param name="playerId">Id of the person</param>
        /// <returns></returns>
        public async Task<IEnumerable<Tournament>> GetFavouriteTournamets(string playerId)
        {
            List<Tournament> tournaments = [];

            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    Id = playerId
                };


                var query = "MATCH (player:Player {Id : $Id})-[:LIKES]->(tournament:Tournament) " +
                            "RETURN tournament";

                var result = await session.RunAsync(query, properties);

				await result.ForEachAsync(record =>
				{
					tournaments.Add(MappingFromDatabase.MapTournament(record["tournament"].As<INode>()));
				});

                return tournaments;
			}
        }
    }
}
