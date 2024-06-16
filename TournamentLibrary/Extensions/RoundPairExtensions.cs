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
    public static class RoundPairExtensions
    {
        /// <summary>
        /// Calculates points of participants based on provided match results
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="participants"></param>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static IEnumerable<ParticipantWithPoints<T>> CalculatePlayerPoints<T>(this IEnumerable<T> participants, IEnumerable<RoundPair<T>> matches) where T : TournamentPlayer
        {
            IEnumerable<ParticipantWithPoints<T>> playersWithPoints = participants.ToPlayersWithPoints();

            foreach (var match in matches)
            {
                ParticipantWithPoints<T>? playerWithPointsWhite = playersWithPoints.Where(x => x.Participant.Id == match.White?.Id).FirstOrDefault();
                if (playerWithPointsWhite != null)
                {
                    playerWithPointsWhite.Score += match.WhiteScore;
                }

                ParticipantWithPoints<T>? playerWithPointsBlack = playersWithPoints.Where(x => x.Participant.Id == match.Black?.Id).FirstOrDefault();
                if (playerWithPointsBlack != null)
                {
                    playerWithPointsBlack.Score += match.BlackScore;
                }
            }

            return playersWithPoints;

        }

        /// <summary>
        /// Returns all matches which are contained in round draws.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matches"></param>
        /// <param name="participant"></param>
        /// <returns></returns>
        public static IEnumerable<RoundPair<T>> GetMatchesOfParticipant<T>(this IEnumerable<RoundPair<T>> matches, T participant) where T : class, IParticipant
        {
            return matches.Where(x => x.White?.Id == participant.Id 
                                || x.Black?.Id == participant.Id).ToList();
        }

        /// <summary>
        /// Checks if all matches already have a result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static bool AreAllMatchesSet<T>(this IEnumerable<RoundPair<T>> matches) where T : class, IParticipant
        {
            foreach (var match in matches)
            {
                if (match.Result.NotYetSet)
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Checks if match is in match collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matches"></param>
        /// <param name="pairToCheck"></param>
        /// <returns></returns>
        public static bool ContainsRoundPair<T>(this IEnumerable<RoundPair<T>> matches, RoundPair<T> pairToCheck) where T : class, IParticipant
        {
            return matches.Where(x => (x.White?.Id == pairToCheck.White?.Id
                                && x.Black?.Id == pairToCheck.Black.Id)
                                || (x.White?.Id == pairToCheck.Black?.Id
                                && x.Black?.Id == pairToCheck.White?.Id)).Any();
        }

        /// <summary>
        /// Checks if participant already have a match in tournament
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matches"></param>
        /// <param name="participantToCheck"></param>
        /// <returns></returns>
        public static bool IsPaired<T>(this IEnumerable<RoundPair<T>> matches, T participantToCheck) where T : class, IParticipant
        {
            if(participantToCheck is null)
            {
                return matches.Where(x => x.White is null
                               || x.Black is null).Any();
            }

            return matches.Where(x => x.White?.Id == participantToCheck.Id
                                || x.Black?.Id == participantToCheck.Id).Any();
        }


        /// <summary>
        /// Returns all participants which are contained in provided matches.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="matches"></param>
        /// <returns></returns>
        public static List<T> GetAllParticipants<T>(this List<RoundPair<T>> matches) where T : class, IParticipant
        {
            List<T> participants = [];

            foreach(var match in matches)
            {
                participants.Add(match.White);
                participants.Add(match.Black);
            }

            return participants;
        }
    }
}
