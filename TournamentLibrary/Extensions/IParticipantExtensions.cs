using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.Participants;

namespace TournamentLibrary.Extensions
{
    public static class IParticipantExtensions
    {
        /// <summary>
        /// Create objects where it is possible to store participant and points from participant objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="participants">Participants for transformation</param>
        /// <returns></returns>
        public static IEnumerable<ParticipantWithPoints<T>> ToPlayersWithPoints<T>(this IEnumerable<T> participants) where T : IParticipant
        {
            List<ParticipantWithPoints<T>> playersWithPoints = [];

            foreach (var player in participants)
            {
                playersWithPoints.Add(new ParticipantWithPoints<T>(player));
            }

            return playersWithPoints;
        }



    }
}
