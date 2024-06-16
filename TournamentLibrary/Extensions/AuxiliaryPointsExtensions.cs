using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;

namespace TournamentLibrary.Extensions
{
    /// <summary>
    /// Methods which will calculate specific auxiliary points in swiss tournament
    /// </summary>
    public static class AuxiliaryPointsExtensions
    {
        public static void CalculateAllAuxiliaryPoints<T>(this IEnumerable<ParticipantWithPoints<T>> participants, IEnumerable<RoundDraw<T>> roundDraws) where T : class, IParticipant
        {
            participants.CalculateBlackPiecesPlayed(roundDraws);
            participants.CalculateBuchholz(roundDraws);
            participants.CalculateBuchholzShortened(roundDraws);
            participants.CalculateSonnenBornBerger(roundDraws);
        }

        public static void CalculateBlackPiecesPlayed<T>(this IEnumerable<ParticipantWithPoints<T>> participants, IEnumerable<RoundDraw<T>> roundDraws) where T : class, IParticipant
        {
            foreach (var participant in participants)
            {
                participant.BlackPiecesPlayed = 0;
            }


            foreach (var roundDraw in roundDraws)
            {
                List<RoundPair<T>> pairs = roundDraw.GetRoundPairs();

                foreach (var pair in pairs)
                {
                    if (participants.Where(x => x.Participant?.Id == pair.Black?.Id).Any())
                    {
                        participants.Where(x => x.Participant?.Id == pair.Black?.Id).First().BlackPiecesPlayed++;
                    }
                }
            }
        }
        public static void CalculateBuchholz<T>(this IEnumerable<ParticipantWithPoints<T>> participants, IEnumerable<RoundDraw<T>> roundDraws) where T : class, IParticipant
        {
            foreach (var participant in participants)
            {
                participant.Buchholz = 0;
            }

            IEnumerable<RoundPair<T>> allPairs = roundDraws.GetAllMatches();

            foreach (var participant in participants)
            {
                IEnumerable<RoundPair<T>> participantsMatches = allPairs.GetMatchesOfParticipant(participant.Participant);

                foreach (var match in participantsMatches)
                {
                    participant.Buchholz += participants.Where(x => x.Participant.Id == match.GetOpponent(participant.Participant)?.Id).FirstOrDefault()?.Score ?? 0;
                }
            }
        }

        public static void CalculateBuchholzShortened<T>(this IEnumerable<ParticipantWithPoints<T>> participants, IEnumerable<RoundDraw<T>> roundDraws) where T : class, IParticipant
        {
            foreach (var participant in participants)
            {
                participant.BuchholzShortened = 0;
            }

            IEnumerable<RoundPair<T>> allPairs = roundDraws.GetAllMatches();

            foreach (var participant in participants)
            {
                IEnumerable<RoundPair<T>> participantsMatches = allPairs.GetMatchesOfParticipant(participant.Participant);

                if (participantsMatches.Count() <= 2)
                {
                    continue;
                }


                decimal minOpponentScore = 0;
                decimal maxOpponentScore = 0;

                foreach (var match in participantsMatches)
                {
                    decimal opponentScore = participants.Where(x => x.Participant.Id == match.GetOpponent(participant.Participant)?.Id).FirstOrDefault()?.Score ?? 0;

                    minOpponentScore = opponentScore < minOpponentScore ? opponentScore : minOpponentScore;
                    maxOpponentScore = opponentScore > maxOpponentScore ? opponentScore : maxOpponentScore;

                    participant.BuchholzShortened += opponentScore;
                }

                participant.BuchholzShortened -= minOpponentScore + maxOpponentScore;

            }
        }

        public static void CalculateSonnenBornBerger<T>(this IEnumerable<ParticipantWithPoints<T>> participants, IEnumerable<RoundDraw<T>> roundDraws) where T : class, IParticipant
        {
            foreach (var participant in participants)
            {
                participant.SonnenbornBerger = 0;
            }

            IEnumerable<RoundPair<T>> allPairs = roundDraws.GetAllMatches();

            foreach (var participant in participants)
            {
                IEnumerable<RoundPair<T>> participantsMatches = allPairs.GetMatchesOfParticipant(participant.Participant);

                foreach (var match in participantsMatches)
                {
                    decimal opponentScore = participants.Where(x => x.Participant.Id == match.GetOpponent(participant.Participant)?.Id).FirstOrDefault()?.Score ?? 0;
                    decimal participantScoreFromMatch = match.GetScoreOfParticipant(participant.Participant);

                    participant.SonnenbornBerger += opponentScore * participantScoreFromMatch;
                }
            }
        }
    }
}
