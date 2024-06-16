using DatabaseCommunicator.Models;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCommunicator.ModelsManipulators
{
    public class PlayerTournamentRegisteringManipulator : DatabaseModelManipulator
    {

        public PlayerTournamentRegisteringManipulator(DatabaseConnectorSettings databaseConnectorSettings) :base(databaseConnectorSettings)
        { 
        
        }



        public async Task RegisterPlayerForTournament(string accountId, string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId,
                    AccountId = accountId
                };

                var query = "MATCH (t:Tournament {Id: $TournamentId})" +
                            "MATCH (p:Player {AccountId: $AccountId})  " +
                            "MERGE (p)-[:PLAYS_IN]->(t)";

                await session.RunAsync(query, properties);
            }
        }


        /// <summary>
        /// When a player is registered tournament, this information has to be stored in database, this method does that
        /// </summary>
        /// <param name="playerInput">Player which was registered to this tournament</param>
        /// <param name="tournamentId">Id of a tournament for which the player was registered</param>

        public async Task RegisterUnknownPlayerInTournament(Player player, string tournamentId)
        {
            using (var session = _driver.AsyncSession())
            {
				var properties = new
                {
                    Id = GetRandomId(),
                    FirstName = player.FirstName,
                    MiddleName = player.MiddleName,
                    LastName = player.LastName,
                    Elo = player.Elo,
					ChessClub = player.ChessClub,
				};


                var createPlayerQuery = "CREATE (p:Player $properties)";
                await session.RunAsync(createPlayerQuery, new { properties = properties });

                var connectQuery = "MATCH (p:Player), (t:Tournament) WHERE p.Id = $playerId AND t.Id = $tournamentId CREATE (p)-[:PLAYS_IN]->(t)";
                var connectParameters = new { tournamentId = tournamentId, playerId = properties.Id };

                await session.RunAsync(connectQuery, connectParameters);
            }
        }

        public async Task DeletePlayerFromTournament(string tournamentId, string playerId)
        {
            using (var session = _driver.AsyncSession())
            {
                var properties = new
                {
                    TournamentId = tournamentId,
                    PlayerId = playerId
                };

                var query = "MATCH (p:Player {Id : $PlayerId})-[relation:PLAYS_IN]->(t:Tournament {Id : $TournamentId}) " +
                            "DELETE relation";

                await session.RunAsync(query, properties);
            }
        }

    }
}
