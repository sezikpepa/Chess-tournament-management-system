using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;

namespace TournamentLibrary.Extensions
{
    public static class RoundDrawExtensions
    {
        /// <summary>
        /// Returns all matches which are contained in all rounds
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="roundDraws">Round draws from tournament</param>
        /// <returns></returns>
        public static IEnumerable<RoundPair<T>> GetAllMatches<T>(this IEnumerable<RoundDraw<T>> roundDraws) where T : class, IParticipant
        {
            List<RoundPair<T>> allPairs = [];
            foreach (var roundDraw in roundDraws)
            {
                allPairs.AddRange(roundDraw.GetRoundPairs());
            }

            return allPairs;
        }

        /// <summary>
        /// Calculates points of participants based on provided matches from tournament
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="participants"></param>
        /// <param name="roundDraws"></param>
        /// <returns></returns>
        public static IEnumerable<ParticipantWithPoints<T>> CalculatePlayerPoints<T>(this IEnumerable<T> participants, IEnumerable<RoundDraw<T>> roundDraws) where T : TournamentPlayer
        {
            IEnumerable<RoundPair<T>> matches = roundDraws.GetAllMatches();
            return participants.CalculatePlayerPoints(matches);
        }


    }
}
