using DatabaseCommunicator.Models;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.Extensions;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentHandlers;
using TournamentLibrary.TournamentParts;

namespace TournamentLibrary.Tournaments
{
	public class SwissTournament<T> : TournamentHandler<T>, ITournament<T> where T : class, IParticipant, new()
    {
		public int ExpectedTournamentRounds => (int)(Math.Ceiling(Math.Log2(Participants.Count)));
        public bool IsDrawPossible => true;
		public bool AllMatchResultsBeforeNextRound => true;

        public bool CanChangePreviousRounds => true;

        public SwissTournament(List<T> participants) : base(participants) { }

		public SwissTournament() { }

		public RoundDraw<T> GenerateRoundDraw(int roundNumber, List<RoundDraw<T>> roundDraws)
		{
			return GetRoundDraw(roundNumber, roundDraws, Participants);
		}

		//official information how pairing should work
		//https://spp.fide.com/wp-content/uploads/dutch2017-approved.pdf

		/// <summary>
		/// Generates round pairing
		/// </summary>
		/// <param name="roundNumber">Swiss pairing does not need it, it is here to maintain compatibility with ITournament</param>
		/// <param name="previousDraws">Previous rounds of the tournament</param>
		/// <param name="participants">Participants playing the tournament</param>
		/// <returns></returns>
		public RoundDraw<T> GetRoundDraw(int roundNumber, List<RoundDraw<T>> previousDraws, List<T> participants)
		{
			
			if(participants.Count % 2 != 0)
			{
                participants.Add(null);
            }


            RoundDraw<T> finalRoundDraw = new();

			List<List<ParticipantWithPoints<T>>> pointsGroups = SortIntoGroups(participants, previousDraws);

			List<List<RoundPair<T>>> groups = [];

			List<ParticipantWithPoints<T>> currentLeftOver = [];

			for(int i = 0; i < pointsGroups.Count; i++)
			{
				(List<RoundPair<T>>, List<ParticipantWithPoints<T>>) result = ProcessGroup(previousDraws.GetAllMatches().ToList(), pointsGroups[i], currentLeftOver);

				if(result.Item1.Count > 0)
				{
                    groups.Add(result.Item1);
                }
                currentLeftOver = result.Item2;
			}


			if (currentLeftOver.Count > 0)
			{
				List<List<RoundPair<T>>> lastGroups = [];
                while (groups.Count > 0)
                {
					lastGroups.Add(groups[groups.Count - 1]);
					groups.RemoveAt(groups.Count - 1);

                    List<RoundPair<T>>? attemptForLastGroup = GenerateCollapsedGroup(lastGroups, previousDraws, currentLeftOver);

                    if (attemptForLastGroup is not null)
                    {
                        groups.Add(attemptForLastGroup);
                        break;
                    }
                }
            }

			//color allocation
			var participantsWithColor = participants.ToPlayersWithPoints();
			participantsWithColor.CalculateBlackPiecesPlayed(previousDraws);

			for (int i = groups.Count - 1; i >= 0; i--)
			{
				foreach (RoundPair<T> pair in groups[i])
				{
					decimal? participantBlackCountWhite = participantsWithColor.Where(x => x.Participant?.Id == pair.White?.Id).FirstOrDefault()?.BlackPiecesPlayed;
					decimal? participantBlackCountBlack = participantsWithColor.Where(x => x.Participant?.Id == pair.Black?.Id).FirstOrDefault()?.BlackPiecesPlayed;

					if(participantBlackCountBlack > participantBlackCountWhite)
					{
						pair.SwapParticipants();
					}
				}
			}





			////////////////////////////////////////////////////////////////////


			for (int i = groups.Count - 1; i >= 0; i--)
			{
                foreach (RoundPair<T> pair in groups[i])
                {
					pair.Result = new SingleMatchResult("Unknown");
                    if (pair.White is null)
                    {
						pair.SwapParticipants();
                    }
                    if (pair.Black is null)
					{
						pair.Result = new SingleMatchResult(1, 0);
                    }
                    

                }
                finalRoundDraw.AddRange(groups[i]);
			}

			

			return finalRoundDraw;
		}

		/// <summary>
		/// In case, where it is not possible to generate pairing, collapsed group is created
		/// </summary>
		/// <param name="groupsToUse">Which groups should be used to create collaped group</param>
		/// <param name="previousDraws">Previous rounds of the tournament</param>
		/// <param name="leftover">Players which were not in any group</param>
		/// <returns></returns>
		private List<RoundPair<T>>? GenerateCollapsedGroup(List<List<RoundPair<T>>> groupsToUse, List<RoundDraw<T>> previousDraws, List<ParticipantWithPoints<T>> leftover)
		{
			List<T> participantsToUse = leftover.Select(x => x.Participant).ToList();
			foreach(List<RoundPair<T>> group in groupsToUse)
			{
                participantsToUse.AddRange(group.GetAllParticipants());

            }

			if(participantsToUse.Count % 2 != 0)
			{
				return null;
			}
			return TryAllParticipantCombinations(participantsToUse, previousDraws);
        }

        ///modifed code from https://www.codeproject.com/Articles/43767/A-C-List-Permutation-Iterator (next two methods)

        /// <summary>
        /// Rotate list by a position
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="count"></param>
        public void RotateRight(List<T> sequence, int count)
        {
            T tmp = sequence[count - 1];
            sequence.RemoveAt(count - 1);
            sequence.Insert(0, tmp);
        }

		/// <summary>
		/// Pemutates the list
		/// </summary>
		/// <param name="sequence"></param>
		/// <param name="count"></param>
		/// <returns></returns>
        public IEnumerable<List<T>> Permutate(List<T> sequence, int count)
        {
            if (count == 1)
            {
                yield return sequence;
            }

            for (int i = 0; i < count; i++)
            {
                foreach (var perm in Permutate(sequence, count - 1))
                {
                    yield return perm;
                }
                RotateRight(sequence, count);
            }
        }

		/// <summary>
		/// Try all pairings witihin the players. Used to pair collapsed group
		/// </summary>
		/// <param name="participants">Registered participants in the tournament</param>
		/// <param name="previousDraws">Previous rounds of the tournament</param>
		/// <returns></returns>
        private List<RoundPair<T>>? TryAllParticipantCombinations(List<T> participants, List<RoundDraw<T>> previousDraws)
        {
            List<RoundPair<T>> prohibitedMatches = previousDraws.GetAllMatches().ToList();
            foreach (List<T> permutace in Permutate(participants, participants.Count))
            {
				var attempt = TryAllParticipantCombination(permutace, prohibitedMatches);

				if(attempt != null)
				{
					return attempt;
				}					
            }
			return null;
        }

        /// <summary>
        /// Checks if the pairing of the round is correct in collapsed group
        /// </summary>
        /// <param name="participants">Registered participants in the tournament</param>
        /// <param name="prohibitedMatches">Matches which cannot occured in the pairing</param>
        /// <returns></returns>
        private List<RoundPair<T>>? TryAllParticipantCombination(List<T> participants, List<RoundPair<T>> prohibitedMatches)
        {
            List<RoundPair<T>> generatedPairs = [];
            for (int i = 0; i < participants.Count; i += 2)
            {
                var newPair = new RoundPair<T>(participants[i], participants[i + 1]);
                if (!ViolatesAbsoluteCriteriaC1(prohibitedMatches, newPair))
                {
                    generatedPairs.Add(newPair);
                }
				else
				{
					return null;
				}
            }
			return generatedPairs;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        ///modifed code from https://www.codeproject.com/Articles/43767/A-C-List-Permutation-Iterator (this method)
		private void RotateRight(List<ParticipantWithPoints<T>> sequence, int count)
        {
            ParticipantWithPoints<T> tmp = sequence[count - 1];
            sequence.RemoveAt(count - 1);
            sequence.Insert(0, tmp);
        }

		/// <summary>
		/// Returns best pairing for current round
		/// </summary>
		/// <param name="allRoundPairsInTournamentList">All pairs which were already generated</param>
		/// <param name="S1">First half of the pairing</param>
		/// <param name="S2">Second half of the pairing</param>
		/// <param name="remainToPermutate">How many players have not yet been permutated (tried in different positions)</param>
		/// <param name="fromBeginning">How many pairs have been created</param>
		/// <param name="correctGeneratedPairs">How many pairs are valid</param>
		/// <returns></returns>
        private List<RoundPair<T>> GetBestCandidate(List<RoundPair<T>> allRoundPairsInTournamentList, List<ParticipantWithPoints<T>> S1, List<ParticipantWithPoints<T>> S2, int remainToPermutate, int fromBeginning, List<RoundPair<T>> correctGeneratedPairs)
        {
            List<List<RoundPair<T>>> candidates = [];
            if (_currentBestResultLength < remainToPermutate + correctGeneratedPairs.Count)
			{
                for (int i = 0; i < remainToPermutate; i++)
                {
                    RoundPair<T> newPair = new(S1[^(fromBeginning + 1)].Participant, S2[^(fromBeginning + 1)].Participant);

                    if (!ViolatesAbsoluteCriteria(allRoundPairsInTournamentList, newPair))
                    {
                        var copy = new List<RoundPair<T>>(correctGeneratedPairs)
						{
							newPair
						};

						if(copy.Count > _currentBestResultLength)
						{
							_currentBestResultLength = copy.Count;
						}
                        candidates.Add(GetBestCandidate(allRoundPairsInTournamentList, S1, S2, remainToPermutate - 1, fromBeginning + 1, copy));
                    }
                    else
                    {
                        candidates.Add(GetBestCandidate(allRoundPairsInTournamentList, S1, S2, remainToPermutate - 1, fromBeginning + 1, correctGeneratedPairs));
                    }

                    RotateRight(S1, remainToPermutate);
                }
            }
			else
			{

			}

			
			candidates.Add(correctGeneratedPairs);

			return candidates.OrderByDescending(x => x.Count).First();         
        }

		/// <summary>
		/// Returns best pairing of a group in round
		/// </summary>
		/// <param name="allRoundPairsInTournamentList">Pairs which occured in the tournament</param>
		/// <param name="S1">First half of the group</param>
		/// <param name="S2">Second half of the group</param>
		/// <returns></returns>
        private List<RoundPair<T>> GetBestGroupPairing(List<RoundPair<T>> allRoundPairsInTournamentList, List<ParticipantWithPoints<T>> S1, List<ParticipantWithPoints<T>> S2)
        {
			_currentBestResultLength = 0;

            List<RoundPair<T>> bestResult = GetBestCandidate(allRoundPairsInTournamentList, S1, S2, S1.Count, 0, []);

			return bestResult;
        }


		private int _currentBestResultLength = 0;

		private List<RoundPair<T>> HowManyCorrectPairs(List<RoundPair<T>> allRoundPairsInTournament, List<ParticipantWithPoints<T>> S1, List<ParticipantWithPoints<T>> S2)
		{
			int i = 0;
			List<RoundPair<T>> correctPairs = [];
			while(i < S1.Count && i < S2.Count)
			{
				RoundPair<T> newPair = new(S1[i].Participant, S2[i].Participant);

				if (!allRoundPairsInTournament.ContainsRoundPair(newPair))
				{
					correctPairs.Add(newPair);
				}
				
				i++;
			}

			return correctPairs;
		}



        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private (List<RoundPair<T>>, List<ParticipantWithPoints<T>>) MakePairs(List<RoundPair<T>> allRoundPairsInTournament, List<ParticipantWithPoints<T>> S1, List<ParticipantWithPoints<T>> S2)
		{
            List<RoundPair<T>> pairedPlayers = GetBestGroupPairing(allRoundPairsInTournament, S1, S2);


            List<ParticipantWithPoints<T>> unpaired = [];

			foreach(var participant in S1)
			{
				if (!pairedPlayers.IsPaired(participant.Participant))
				{
					unpaired.Add(participant);
				}
			}

            foreach (var participant in S2)
            {
                if (!pairedPlayers.IsPaired(participant.Participant))
                {
                    unpaired.Add(participant);
                }
            }

			return (pairedPlayers, unpaired);

        }

		/// <summary>
		/// Generates best pairing for the group. Splits it and then pairs it
		/// </summary>
		/// <param name="allPreviousMatches">All matches which occured in the tournament</param>
		/// <param name="playersWithPoints">Participants which should be paired</param>
		/// <param name="fromPreviousGroup">Participants coming from the previous group</param>
		/// <returns></returns>
        private (List<RoundPair<T>>, List<ParticipantWithPoints<T>>) ProcessGroup(List<RoundPair<T>> allPreviousMatches, List<ParticipantWithPoints<T>> playersWithPoints, List<ParticipantWithPoints<T>> fromPreviousGroup)
		{
			fromPreviousGroup.AddRange(playersWithPoints);
			playersWithPoints = fromPreviousGroup;

			(List<ParticipantWithPoints<T>>, List<ParticipantWithPoints<T>>, List<ParticipantWithPoints<T>>) result = SplitIntoHalves(playersWithPoints);
			var group1 = result.Item1;
			var group2 = result.Item2;
			var leftover = result.Item3;


            (List<RoundPair<T>>, List<ParticipantWithPoints<T>>) madePairs = MakePairs(allPreviousMatches, group1, group2);

            List<RoundPair<T>> pairedPlayers = madePairs.Item1;

			leftover.AddRange(madePairs.Item2);


            return (pairedPlayers, leftover);
		}

		/// <summary>
		/// Splits participants into two halves in the group
		/// </summary>
		/// <param name="playersWithPoints"></param>
		/// <returns></returns>
		private (List<ParticipantWithPoints<T>>, List<ParticipantWithPoints<T>>, List<ParticipantWithPoints<T>>) SplitIntoHalves(List<ParticipantWithPoints<T>> playersWithPoints)
		{
			int numberOfPlayers = playersWithPoints.Count;

			if (numberOfPlayers == 0)
			{
				return (new List<ParticipantWithPoints<T>>(), new List<ParticipantWithPoints<T>>(), new List<ParticipantWithPoints<T>>());
			}
			if (numberOfPlayers == 1)
			{
				return (new List<ParticipantWithPoints<T>>(), new List<ParticipantWithPoints<T>>(), new List<ParticipantWithPoints<T>>() { playersWithPoints[0] });
			}

			playersWithPoints = playersWithPoints.OrderByDescending(x => x.Score).ToList();

			if (numberOfPlayers % 2 == 0)
			{
				List<ParticipantWithPoints<T>> firstHalf = playersWithPoints.GetRange(0, numberOfPlayers / 2);
				List<ParticipantWithPoints<T>> secondHalf = playersWithPoints.GetRange(numberOfPlayers / 2, numberOfPlayers / 2);

				return (firstHalf, secondHalf, []);
			}
			else
			{
				List<ParticipantWithPoints<T>> firstHalf = playersWithPoints.GetRange(0, numberOfPlayers / 2);
				List<ParticipantWithPoints<T>> secondHalf = playersWithPoints.GetRange(numberOfPlayers / 2, numberOfPlayers / 2);

				return (firstHalf, secondHalf, [playersWithPoints[numberOfPlayers - 1]]);
			}
		}



		/// <summary>
		/// Sort players into separate groups based on their points
		/// </summary>
		/// <param name="participants">Participants of the tournament, not currently used, but can be used in the future to influence sorting process (for example in first rounds, if the players joined in later rounds)</param>
		/// <param name="previousDraws">Rounds in the past in the tournament</param>
		/// <returns></returns>
		private List<List<ParticipantWithPoints<T>>> SortIntoGroups(List<T> participants, List<RoundDraw<T>> previousDraws)
		{
			List<ParticipantWithPoints<T>> playersWithPoints = GetCurrentStandings(previousDraws, new ResultsSettings() { CalculateByPointsPerWinDraw = false }, false);

			List<List<ParticipantWithPoints<T>>> groups = [];


			decimal maximumPoints = playersWithPoints.Select(x => x.Score).Max();

			//number of groups based on number of different results in players with points
			for (int i = 0; i <= maximumPoints * 2; i += 1)
			{
				groups.Add([]);
			}

			foreach (var playerWithPoints in playersWithPoints)
			{
				groups[groups.Count - 1 - (int)(playerWithPoints.Score * 2)].Add(playerWithPoints);
			}

			return groups;

		}


		/// <summary>
		/// Check if the match satisyfy all criteria, now it is here only one - in the future there can be implemented other variants
		/// </summary>
		/// <param name="roundPairs">Matches in the tournament</param>
		/// <param name="pairToCheck">Round pair which should be checked</param>
		/// <returns></returns>
		private bool ViolatesAbsoluteCriteria(List<RoundPair<T>> roundPairs, RoundPair<T> pairToCheck)
		{
			if(ViolatesAbsoluteCriteriaC1(roundPairs, pairToCheck))
			{
				return true;
			}
			return false;
		}

		/// <summary>
		/// Checkes if the match already occured in the tournament
		/// </summary>
		/// <param name="roundPairs">Matches of the tournament</param>
		/// <param name="pairToCheck">Match to be validated</param>
		/// <returns></returns>
        private bool ViolatesAbsoluteCriteriaC1(List<RoundPair<T>> roundPairs, RoundPair<T> pairToCheck)
        {
			return roundPairs.Where(x => x.PlaysInPair(pairToCheck.White) && x.PlaysInPair(pairToCheck.Black)).ToList().Count != 0;
        }
    }
}
