using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.Participants;

namespace Tests
{
    public static class MockMethods
    {

        public static TournamentPlayer GenerateTournamentPlayer()
        {
            var toReturn = new TournamentPlayer();
            toReturn.Id = new Random().Next().ToString();
            toReturn.FirstName = MethodsForGenericGenerating.RandomString(10);
            toReturn.MiddleName = MethodsForGenericGenerating.RandomString(10);
            toReturn.LastName = MethodsForGenericGenerating.RandomString(10);

            return toReturn;
        }

        public static List<TournamentPlayer> GeneratePlayersForTournament(int count)
        {
            List<TournamentPlayer> toReturn = new List<TournamentPlayer>();

            for (int i = 0; i < count; i++)
            {
                toReturn.Add(GenerateTournamentPlayer());
            }

            return toReturn;
        }

        public static List<TournamentPlayer> GetFirstPlayersForTournament(int count)
        {
            List<TournamentPlayer> toReturn = new List<TournamentPlayer>();

            for (int i = 0; i < count; i++)
            {
                toReturn.Add(new TournamentPlayer() { Id = i.ToString()});
            }

            return toReturn;
        }
    }
}
