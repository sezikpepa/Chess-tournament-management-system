using TournamentLibrary.Participants;

namespace TournamentLibrary.TournamentParts
{
    public class RoundDraw<T> where T : class, IParticipant
    {

        public List<RoundPair<T>> _roundPairs { get; set; }

        public RoundDraw()
        {
            _roundPairs = [];
        }

        public RoundDraw(List<RoundPair<T>> pairs)
        {
            _roundPairs = pairs;
        }

        public void AddPair(RoundPair<T> pair)
        {
            _roundPairs.Add(pair);
        }

        public void AddRange(List<RoundPair<T>> pairs)
        {
            foreach (RoundPair<T> pair in pairs)
            {
                AddPair(pair);
            }
        }
        

        public List<RoundPair<T>> GetRoundPairs()
        {
            return _roundPairs;
        }


        public void SortMatches(List<ParticipantWithPoints<T>> playersWithPoints)
        {
            _roundPairs = _roundPairs.OrderByDescending(x => Math.Max(playersWithPoints.Where(y => y.Participant.Id == x.White?.Id).FirstOrDefault()?.Score ?? decimal.MinValue, playersWithPoints.Where(y => y.Participant.Id == x.Black?.Id).FirstOrDefault()?.Score ?? decimal.MinValue))
                       .ThenByDescending(x => Math.Min(playersWithPoints.Where(y => y.Participant.Id == x.White?.Id).FirstOrDefault()?.Score ?? decimal.MinValue, playersWithPoints.Where(y => y.Participant.Id == x.Black?.Id).FirstOrDefault()?.Score ?? decimal.MinValue)).ToList();
        }

        public override string ToString()
        {
            string toReturn = "";

            foreach (RoundPair<T>  pair in _roundPairs)
            {
                toReturn += pair.ToString();
                toReturn += " | ";
            }

            return toReturn;
        }

    }
}
