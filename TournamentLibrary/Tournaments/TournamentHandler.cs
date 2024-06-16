using DatabaseCommunicator.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.Extensions;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentHandlers;
using TournamentLibrary.TournamentParts;

namespace TournamentLibrary.Tournaments
{
    public abstract class TournamentHandler<T> where T : class, IParticipant
    {

        public List<T> Participants { get; set; } = [];


        public TournamentHandler(List<T> participants)
        {
            Participants = participants;
        }

        public TournamentHandler() { }


        public void AddParticipant(T participant)
        {
            Participants.Add(participant);
        }

        public void AddParticipantRange(IEnumerable<T> tournamentPlayers)
        {
            Participants.AddRange(tournamentPlayers);
        }


        protected void AddPointsToParticipant(ParticipantWithPoints<T>? participant, IRoundPair<T> match, ResultsSettings resultsSettings)
        {
            if(participant is null)
            {
                return;
            }

            if(resultsSettings.CalculateByPointsPerWinDraw)
            {
                if (match.IsDraw)
                {
                    participant.AddScore(resultsSettings.PointsForDraw!.Value);
                    return;
                }

                var winner = match.GetWinner();
                if(winner.Id == participant.Participant.Id)
                {
                    participant.AddScore(resultsSettings.PointsForWin!.Value);
                    return;
                }

                return;
            }

            participant.AddScore(match.GetScoreOfParticipant(participant.Participant));
        }

		public virtual List<ParticipantWithPoints<T>> GetCurrentStandings(List<RoundDraw<T>> roundDraws, ResultsSettings resultsSettings, bool calculateAuxiliaryPoints = true)
		{
			IEnumerable<ParticipantWithPoints<T>> playersWithPoints = Participants.ToPlayersWithPoints();

			foreach (RoundDraw<T> roundDraw in roundDraws)
			{
				foreach (RoundPair<T> pair in roundDraw.GetRoundPairs())
				{
					ParticipantWithPoints<T>? white = playersWithPoints.Where(x => x.Participant?.Id == pair.White?.Id).FirstOrDefault();
					ParticipantWithPoints<T>? black = playersWithPoints.Where(x => x.Participant?.Id == pair.Black?.Id).FirstOrDefault();

					AddPointsToParticipant(white, pair, resultsSettings);
					AddPointsToParticipant(black, pair, resultsSettings);
				}
			}

            if (calculateAuxiliaryPoints)
            {
                playersWithPoints.CalculateAllAuxiliaryPoints(roundDraws);
            }


            var sortedPlayers = playersWithPoints.OrderByDescending(p => p.Score).ToList();

			return sortedPlayers.ToList();
		}

	}
}
