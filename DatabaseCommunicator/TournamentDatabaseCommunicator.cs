using DatabaseCommunicator.Models;
using DatabaseCommunicator.ModelsManipulators;
using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;
using Neo4j.Driver.Preview.Mapping;
using TournamentLibrary.TournamentParts;

namespace DatabaseCommunicator
{
    public class TournamentManager : DatabaseModelManipulator
    {
        public TournamentManager(DatabaseConnectorSettings databaseConnectorSettings) : base(databaseConnectorSettings)
        {


        }

        public async Task<IEnumerable<Player>> GetTournamentParticipants(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                var query = "MATCH (p:Player)-[:PLAYS_IN]->(t:Tournament) WHERE t.Id = $tournamentId RETURN p;";

                var result = await session.RunAsync(query, new { tournamentId = tournamentId });

                return await result.ToListAsync<Player>();
            }

        }

        /// <summary>
        /// Retrives information about players of tournament from database
        /// </summary>
        /// <param name="tournamentId">Id of a tournament</param>
        /// <returns>List of players of the tournament</returns>
        public async Task<List<Player>> GetPlayersInTournament(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
				var query = "MATCH (player:Player)-[:PLAYS_IN]->(t:Tournament) WHERE t.Id = $TournamentId RETURN player " +
                            "UNION " +
							"MATCH (player:Player)-[:PLAYS_FOR]->(team:Team)-[:PLAYS_IN]->(tournament:Tournament {Id : $TournamentId}) " + 
                            "RETURN player";

                var result = await session.RunAsync(query, new { TournamentId = tournamentId });

                var registeredPlayers = await result.ToListAsync<Player>();

                return registeredPlayers.ToList();
            }
        }


        /// <summary>
        /// Saves generated matches into database, firstly it creates a new round and then it saves matches
        /// </summary>
        /// <param name="roundDraw">Matches of the round</param>
        /// <param name="tournamendId">Id of a tournament which belongs to round</param>
        /// <param name="roundNumber">Number of round from the start of a tournament</param>
        public async Task SaveTournamentRoundDraw(RoundDraw<Player> roundDraw, string tournamendId, int roundNumber)
        {
            await CreateNewTournamentRoundAsync(tournamendId, roundNumber);
            
            foreach (RoundPair<Player> pair in roundDraw.GetRoundPairs())
            {
                await SaveTournamentPairAsync(tournamendId, roundNumber, pair);
            }          
        }

        public async Task SaveTournamentRoundDrawTeams(RoundDraw<Team> roundDraw, string tournamendId, int roundNumber)
        {
            await CreateNewTournamentRoundAsync(tournamendId, roundNumber);

            foreach (RoundPair<Team> pair in roundDraw.GetRoundPairs())
            {
                await SaveTournamentPairTeamsAsync(tournamendId, roundNumber, pair);
            }
        }



        /// <summary>
        /// Accept generated pair, which is supposed to play with each other and saves it into database
        /// </summary>
        /// <param name="tournamentId">Id of a tournament, where this match is held</param>
        /// <param name="roundId">Id of a round, where this match is held</param>
        /// <param name="pair">Pair of players which attend this match</param>
        private async Task SaveTournamentPairAsync(string tournamentId, int roundNumber, RoundPair<Player> pair) 
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    WhiteId = pair.White?.Id,
                    BlackId = pair.Black?.Id,
                    TournamentId = tournamentId,
                    RoundNumber = roundNumber,
                    Result = pair.Result.ToString()
                };
                
                var query = "MATCH (r:Round { RoundNumber: $RoundNumber})-[:BELONGS_TO]->(t:Tournament {Id: $TournamentId}) " +
					        "CREATE (roundPair:RoundPair {Result : $Result})-[:BELONGS_TO]->(r) " +
                            "WITH roundPair " +
							"MATCH (p1: Player { Id: $WhiteId}) " +
                            "CREATE (roundPair)-[:WHITE]->(p1) " +
                            "WITH roundPair " +
							"MATCH (p2: Player { Id: $BlackId}) " +
							"CREATE (roundPair)-[:BLACK]->(p2) " +
							"RETURN roundPair;";

                await session.RunAsync(query, properties);
			}
        }

        private async Task SaveTournamentPairTeamsAsync(string tournamentId, int roundNumber, RoundPair<Team> pair)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    WhiteId = pair.White?.Id,
                    BlackId = pair.Black?.Id,
                    TournamentId = tournamentId,
                    RoundNumber = roundNumber,
                    Result = pair.Result.ToString()
                };

                var query = "MATCH (r:Round { RoundNumber: $RoundNumber})-[:BELONGS_TO]->(t:Tournament {Id: $TournamentId}) " +
                            "CREATE (roundPair:RoundPair {Result : $Result})-[:BELONGS_TO]->(r) " +
                            "WITH roundPair " +
                            "MATCH (p1: Team { Id: $WhiteId}) " +
                            "CREATE (roundPair)-[:WHITE]->(p1) " +
                            "WITH roundPair " +
                            "MATCH (p2: Team { Id: $BlackId}) " +
                            "CREATE (roundPair)-[:BLACK]->(p2) " +
                            "RETURN roundPair;";

                await session.RunAsync(query, properties);
            }
        }

        /// <summary>
        /// Create new round in a tournament. Later it is possible to store matches for this round
        /// </summary>
        /// <param name="tournamendId">Id of a tournament for which round is created</param>
        /// <param name="roundNumber">Round number from the start of a tournament</param>
        private async Task CreateNewTournamentRoundAsync(string tournamentId, int roundNumber)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId,
                    RoundNumber = roundNumber 
                };

                var query = "MATCH (t:Tournament) WHERE t.Id = $TournamentId CREATE (r:Round {RoundNumber: $RoundNumber, RoundDrawGenerated: true})-[:BELONGS_TO]->(t)";

                await session.RunAsync(query, properties);
            }
        }


        /// <summary>
        /// When new results are insert by the user. Round draw has to be updated in database. This method saves match results into matches, which are saved
        /// in the database
        /// </summary>
        /// <param name="tournamentId">Id of a tournament of the round</param>
        /// <param name="roundNumber">Number of a round from start of the tournament</param>
        /// <param name="results">Results insert by the user</param>
        public async Task SaveMatchResults(string tournamentId, int roundNumber, List<RoundPair<Player>> results) 
        {
            using (var session = _driver.AsyncSession())
            {
                foreach (RoundPair<Player> result in results)
                {
                    var properties = new
                    {
                        TournamentId = tournamentId,
                        RoundNumber = roundNumber,
                        WhiteId = result.White?.Id,
                        BlackId = result.Black?.Id,
                        Result = result.Result.ToString()
                    };

                    string query;

                    if(properties.WhiteId is not null && properties.BlackId is not null)
                    {
						query = "MATCH (pair:RoundPair)-[:BELONGS_TO]->(round:Round {RoundNumber : $RoundNumber})-[:BELONGS_TO]->(t:Tournament {Id : $TournamentId})" +
								"MATCH (white:Player {Id : $WhiteId})<-[:WHITE]-(pair) " +
								"MATCH (black:Player {Id : $BlackId})<-[:BLACK]-(pair) " +
								"SET pair.Result = $Result;";
					}
                    else if (properties.WhiteId is null && properties.BlackId is not null)
                    {
						query = "MATCH (pair:RoundPair)-[:BELONGS_TO]->(round:Round {RoundNumber : $RoundNumber})-[:BELONGS_TO]->(t:Tournament {Id : $TournamentId})" +
								"MATCH (black:Player {Id : $BlackId})<-[:BLACK]-(pair) " +
								"SET pair.Result = $Result;";
					}
                    else
                    {
						query = "MATCH (pair:RoundPair)-[:BELONGS_TO]->(round:Round {RoundNumber : $RoundNumber})-[:BELONGS_TO]->(t:Tournament {Id : $TournamentId})" +
								"MATCH (white:Player {Id : $WhiteId})<-[:WHITE]-(pair) " +
								"SET pair.Result = $Result;";
					}


					await session.RunAsync(query, properties);
                }
            }
        }

        public async Task SaveSubMatchResultsAsync(string tournamentId, int roundNumber, RoundPair<Team> teamPair, List<RoundPair<Player>> subPairs)
        {
            using (var session = _driver.AsyncSession())
            {
                foreach (RoundPair<Player> subPair in subPairs)
                {
                    var properties = new
                    {
                        TournamentId = tournamentId,
                        RoundNumber = roundNumber,
                        WhiteId = subPair.White.Id,
                        BlackId = subPair.Black.Id,
                        Result = subPair.Result.ToString(),

                        Team1Id = teamPair.White.Id,
                        Team2Id = teamPair.Black.Id,
                    };

                    var query = "MATCH (playerPair:RoundPair)<-[:SUBPAIR]-(teamPair:RoundPair)-[:BELONGS_TO]->(round:Round {RoundNumber : $RoundNumber})-[:BELONGS_TO]->(t:Tournament {Id : $TournamentId}) " +
                                "MATCH (white:Player {Id : $WhiteId})<-[:Player1]-(playerPair) " +
                                "MATCH (black:Player {Id : $BlackId})<-[:Player2]-(playerPair) " +
                                "SET playerPair.Result = $Result;";

                    await session.RunAsync(query, properties);
                }
            }
        }


        /// <summary>
        /// To retrive pairing for a specific round of a tournament, we use this method.
        /// </summary>
        /// <param name="tournamentId">Id of a tournament</param>
        /// <param name="roundNumber">Number of a round from start of the tournament</param>
        /// <returns>Return all player pairs in specified round</returns>
        public async Task<RoundDraw<Player>> GetRoundDrawAsync(string tournamentId, int roundNumber)
        {
            RoundDraw<Player> toReturn = new RoundDraw<Player>();

            var roundPairs = await GetTournamentPairsAsync(tournamentId, roundNumber);

            toReturn.AddRange(roundPairs);

            return toReturn;
        }

        public async Task<RoundDraw<Team>> GetRoundDrawTeamsAsync(string tournamentId, int roundNumber)
        {
            RoundDraw<Team> toReturn = new();

            var roundPairs = await GetTournamentPairsTeamsAsync(tournamentId, roundNumber);

            toReturn.AddRange(roundPairs);

            return toReturn;
        }

        public async Task<List<RoundPair<Player>>> GetTournamentPairsAsync(string tournamentId, int roundNumber)
        {
            var toReturn = new List<RoundPair<Player>>();


            using (var session = _driver.AsyncSession())
            {
                //all
                var properties = new
                {
                    TournamentId = tournamentId,
                    RoundNumber = roundNumber
                };

                var query = "MATCH(t:Tournament {Id: $TournamentId})<-[:BELONGS_TO]-(round: Round {RoundNumber: $RoundNumber})<-[:BELONGS_TO]-(pair:RoundPair) " +
                            "OPTIONAL MATCH (white:Player)<-[:WHITE]-(pair:RoundPair)-[:BELONGS_TO]->(round)" +
                            "OPTIONAL MATCH (black:Player)<-[:BLACK]-(pair:RoundPair)-[:BELONGS_TO]->(round)" +
                            "RETURN pair, white, black;";

                IResultCursor result = await session.RunAsync(query, properties);

                await result.ForEachAsync(record =>
                {
                    var newPair = new RoundPair<Player>();
                    var result = record["pair"].As<INode>().Properties.ContainsKey("Result") ? new SingleMatchResult(record["pair"].As<INode>()["Result"].As<string>()) : new SingleMatchResult("");
                    Player? white = MappingFromDatabase.MapPlayer(record["white"].As<INode>());
                    Player? black = MappingFromDatabase.MapPlayer(record["black"].As<INode>());


                    newPair.White = white;
                    newPair.Black = black;
                    newPair.Result = result;

                    toReturn.Add(newPair);
                });
			}
           

            return toReturn;
        }

        public async Task<List<RoundPair<Team>>> GetTournamentPairsTeamsAsync(string tournamentId, int roundNumber)
        {
            List<RoundPair<Team>> toReturn = [];


            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId,
                    RoundNumber = roundNumber
                };

                var query = "MATCH(t:Tournament {Id: $TournamentId})<-[:BELONGS_TO]-(round: Round {RoundNumber: $RoundNumber})<-[:BELONGS_TO]-(pair:RoundPair) " +
                            "OPTIONAL MATCH (white:Team)<-[:WHITE]-(pair:RoundPair)-[:BELONGS_TO]->(round)" +
                            "OPTIONAL MATCH (black:Team)<-[:BLACK]-(pair:RoundPair)-[:BELONGS_TO]->(round)" +
                            "RETURN pair, white, black;";

                IResultCursor result = await session.RunAsync(query, properties);

                await result.ForEachAsync(record =>
                {
                    var newPair = new RoundPair<Team>();
                    var result = record["pair"].As<INode>().Properties.ContainsKey("Result") ? new SingleMatchResult(record["pair"].As<INode>()["Result"].As<string>()) : new SingleMatchResult("");
                    var whiteTieBreakWinner = record["pair"].As<INode>().Properties.ContainsKey("TieBreakWinnerWhite") ? record["pair"].As<INode>()["TieBreakWinnerWhite"].As<bool?>() : null;
                    var white = MappingFromDatabase.MapTeam(record["white"].As<INode>());
                    var black = MappingFromDatabase.MapTeam(record["black"].As<INode>());

                    newPair.White = white;
                    newPair.Black = black;
                    newPair.Result = result;
                    newPair.TieBreakWinnerWhite = whiteTieBreakWinner;

                    toReturn.Add(newPair);
                });
            }


            return toReturn;
        }


		public async Task<List<RoundPair<Player>>> GetTournamentPairs(string tournamentId)
        {
            var toReturn = new List<RoundPair<Player>>();


            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId
                };

                var query = "MATCH(t:Tournament {Id: $TournamentId})<-[:BELONGS_TO]-(round:Round)<-[:BELONGS_TO]-(pair:RoundPair) " +
                            "MATCH (white:Player)<-[:WHITE]-(pair:RoundPair)-[:BELONGS_TO]->(round)" +
                            "MATCH (black:Player)<-[:BLACK]-(pair:RoundPair)-[:BELONGS_TO]->(round)" +
                            "RETURN pair, white, black;";

                IResultCursor result = await session.RunAsync(query, properties);

                await result.ForEachAsync(record =>
                {
                    var newPair = new RoundPair<Player>();
                    var result = record["pair"].As<INode>().Properties.ContainsKey("Result") ? new SingleMatchResult(record["pair"].As<INode>()["Result"].As<string>()) : new SingleMatchResult("");
                    var white = MappingFromDatabase.MapPlayer(record["white"].As<INode>());
                    var black = MappingFromDatabase.MapPlayer(record["black"].As<INode>());

                    newPair.White = white;
                    newPair.Black = black;
                    newPair.Result = result;

                    toReturn.Add(newPair);
                });
            }
            return toReturn;
        }

		public async Task<List<RoundPair<Team>>> GetAllTournamentPairsTeams(string tournamentId)
		{
			var toReturn = new List<RoundPair<Team>>();


			using (var session = _driver.AsyncSession())
			{
				var properties = new
				{
					TournamentId = tournamentId
				};

				var query = "MATCH(t:Tournament {Id: $TournamentId})<-[:BELONGS_TO]-(round:Round)<-[:BELONGS_TO]-(pair:RoundPair) " +
							"MATCH (white:Team)<-[:WHITE]-(pair:RoundPair)-[:BELONGS_TO]->(round)" +
							"MATCH (black:Team)<-[:BLACK]-(pair:RoundPair)-[:BELONGS_TO]->(round)" +
							"RETURN pair, white, black;";

				IResultCursor result = await session.RunAsync(query, properties);

				await result.ForEachAsync(record =>
				{
					var newPair = new RoundPair<Team>();
					var result = record["pair"].As<INode>().Properties.ContainsKey("Result") ? new SingleMatchResult(record["pair"].As<INode>()["Result"].As<string>()) : new SingleMatchResult("");
					var white = MappingFromDatabase.MapTeam(record["white"].As<INode>());
					var black = MappingFromDatabase.MapTeam(record["black"].As<INode>());

					newPair.White = white;
					newPair.Black = black;
					newPair.Result = result;

					toReturn.Add(newPair);
				});
			}
			return toReturn;
		}


		/// <summary>
		/// Checks if a combination of tournamentId and roundNumber exists in the database
		/// </summary>
		/// <param name="tournamentId">Id of a tournament</param>
		/// <param name="roundNumber">Number of a round from the start of the tournament</param>
		/// <returns></returns>
		public async Task<bool> RoundDrawExistsAsync(string tournamentId, int roundNumber)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    Id = tournamentId,
                    RoundNumber = roundNumber
                };


                var query = "MATCH (r:Round)-[:BELONGS_TO]->(t:Tournament) WHERE r.RoundNumber = $RoundNumber AND t.Id = $Id RETURN r;";
                var result = await session.RunAsync(query, properties);

                await foreach(var record in result)
                {
					return record["r"].As<INode>()["RoundDrawGenerated"].As<bool>();
				}

                return false;
			}
        }

        public async Task CreateNewTeam(Team teamToCreate, string accountId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    Id = GetRandomId(),
                    Name = teamToCreate.Name,
                };

                var query = "CREATE (newTeam:Team $properties) " +
                            "WITH newTeam " +
                            "MATCH (manager:Player {AccountId : $AccountId}) " +
                            "CREATE (newTeam)-[:MANAGED_BY]->(manager)";

                await session.RunAsync(query, new { properties = properties, AccountId = accountId });
            }
        }


        public async Task SaveSubMatches(Dictionary<RoundPair<Team>, List<RoundPair<Player>>> subMatches, string tournamentId, int roundNumber)
        {
            foreach(var key in subMatches.Keys)
            {
                await SaveSubMatchesOfTeam(key, subMatches[key], tournamentId, roundNumber);
            }
        }

        public async Task SaveSubMatchesOfTeam(RoundPair<Team> pair, List<RoundPair<Player>> subMatches, string tournamentId, int roundNumber)
        {
            for(int i = 0; i < subMatches.Count; i++)
            {
                await SaveSubMatch(pair, subMatches[i], i, tournamentId, roundNumber);
            }
        }

        public async Task SaveSubMatch(RoundPair<Team> pair, RoundPair<Player> subMatch, int board, string tournamentId, int roundNumber)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    WhiteId = pair.White?.Id,
                    BlackId = pair.Black?.Id,

                    Player1 = subMatch.White?.Id,
                    Player2 = subMatch.Black?.Id,

                    Board = board,

                    TournamentId = tournamentId,
                    RoundNumber = roundNumber,

                    Result = subMatch.Result.ToString()
                };

                string whiteOptional = pair.White is null ? "OPTIONAL" : "";
                string blackOptional = pair.Black is null ? "OPTIONAL" : "";

                var query = "MATCH (tournament:Tournament {Id : $TournamentId})<-[:BELONGS_TO]-(round:Round {RoundNumber : $RoundNumber})<-[:BELONGS_TO]-(pair:RoundPair) " +
                            whiteOptional + " MATCH (pair:RoundPair)-[:WHITE]->(white:Team {Id: $WhiteId}) " +
                            blackOptional + " MATCH (pair:RoundPair)-[:BLACK]->(black:Team {Id: $BlackId}) " +
                            "WITH pair " +
                            "CREATE (subPair:RoundPair) " +
                            "CREATE (pair)-[:SUBPAIR {Board: $Board}]->(subPair) " +
                            "SET subPair.Result = $Result ";


                if (properties.WhiteId is not null)
                {
                    query += "WITH subPair " +
                             "MATCH (player1:Player {Id: $Player1}) " +
                             "CREATE (subPair)-[:Player1]->(player1) ";
                             
                }

                if (properties.BlackId is not null)
                {
                    query += "WITH subPair " +
                             "MATCH (player2:Player {Id: $Player2}) " +
                             "CREATE (subPair)-[:Player2]->(player2) ";
                             
                }

                IResultCursor result = await session.RunAsync(query, properties);
            }
        }

        public async Task<List<Tuple<RoundPair<Team>, List<RoundPair<Player>>>>> GetSubPairs(string tournamentId, int roundNumber, RoundDraw<Team> roundDraw)
        {
            List<Tuple<RoundPair<Team>, List<RoundPair<Player>>>> result = [];
            foreach (var pair in roundDraw.GetRoundPairs())
            {
                Tuple<RoundPair<Team>, List<RoundPair<Player>>> subPairs = new(pair, await GetSubPairsForTeamPairAsync(pair, tournamentId, roundNumber));
                result.Add(subPairs);
            }

            return result;
        }

        public async Task<List<RoundPair<Player>>> GetSubPairsForTeamPairAsync(RoundPair<Team> pair, string tournamentId, int roundNumber)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId,
                    RoundNumber = roundNumber,

                    WhiteId = pair.White?.Id,
                    BlackId = pair.Black?.Id
                };

                string whiteOptional = pair.White is null ? "OPTIONAL" : "";
                string blackOptional = pair.Black is null ? "OPTIONAL" : "";


                var query = "MATCH (tournament:Tournament {Id : $TournamentId})<-[:BELONGS_TO]-(round:Round {RoundNumber : $RoundNumber})<-[:BELONGS_TO]-(roundPair:RoundPair) " +
                            whiteOptional + " MATCH (team1:Team {Id : $WhiteId})<-[:WHITE]-(roundPair) " +
                            blackOptional + " MATCH (roundPair)-[:BLACK]->(team2:Team {Id : $BlackId}) " +
                            "MATCH (roundPair)-[:SUBPAIR]->(subPair:RoundPair) " +
                            whiteOptional + " MATCH (player1:Player)<-[:Player1]-(subPair) " +
                            blackOptional + " MATCH (player2:Player)<-[:Player2]-(subPair) " +
                            "RETURN subPair, player1, player2";


                IResultCursor result = await session.RunAsync(query, properties);

                List<RoundPair<Player>> toReturn = [];

                await result.ForEachAsync(record =>
                {
                    var newPair = new RoundPair<Player>();
                    var result = record["subPair"].As<INode>().Properties.ContainsKey("Result") ? new SingleMatchResult(record["subPair"].As<INode>()["Result"].As<string>()) : new SingleMatchResult("");
                    var white = MappingFromDatabase.MapPlayer(record["player1"].As<INode>());
                    var black = MappingFromDatabase.MapPlayer(record["player2"].As<INode>());

                    newPair.White = white;
                    newPair.Black = black;
                    newPair.Result = result;

                    toReturn.Add(newPair);
                });

                return toReturn;
            }
        }

        public async Task SaveTeamResult(string tournamentId, int roundNumber, RoundPair<Team> teamPair)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId,
                    RoundNumber = roundNumber,
                    WhiteId = teamPair.White.Id,
                    BlackId = teamPair.Black.Id,
                    Result = teamPair.Result.ToString(),
                    TieBreakWinnerWhite = teamPair.TieBreakWinnerWhite
                };

                var query = "MATCH (pair:RoundPair)-[:BELONGS_TO]->(round:Round {RoundNumber : $RoundNumber})-[:BELONGS_TO]->(t:Tournament {Id : $TournamentId})" +
                            "MATCH (white:Team {Id : $WhiteId})<-[:WHITE]-(pair) " +
                            "MATCH (black:Team {Id : $BlackId})<-[:BLACK]-(pair) " +
                            "SET pair.Result = $Result, pair.TieBreakWinnerWhite = $TieBreakWinnerWhite;";

                await session.RunAsync(query, properties);                
            }
        }

        public async Task<List<RoundPair<Player>>> GetAllPlayerPairsInTeamTournamentAsync(string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId,
                };

                var query = "MATCH (tournament:Tournament {Id : $TournamentId})<-[:BELONGS_TO]-(:Round)<-[:BELONGS_TO]-(roundPair:RoundPair) " +
                            "MATCH (roundPair)-[:SUBPAIR]->(subPair:RoundPair) " +
                            "MATCH (player1:Player)<-[:Player1]-(subPair) " +
                            "MATCH (player2:Player)<-[:Player2]-(subPair) " +
                            "RETURN subPair, player1, player2";


                IResultCursor result = await session.RunAsync(query, properties);

                List<RoundPair<Player>> toReturn = [];

                await result.ForEachAsync(record =>
                {
                    var newPair = new RoundPair<Player>();
                    var result = record["subPair"].As<INode>().Properties.ContainsKey("Result") ? new SingleMatchResult(record["subPair"].As<INode>()["Result"].As<string>()) : new SingleMatchResult("");
                    var white = MappingFromDatabase.MapPlayer(record["player1"].As<INode>());
                    var black = MappingFromDatabase.MapPlayer(record["player2"].As<INode>());

                    newPair.White = white;
                    newPair.Black = black;
                    newPair.Result = result;

                    toReturn.Add(newPair);
                });

                return toReturn;
            }
        }
    }
}