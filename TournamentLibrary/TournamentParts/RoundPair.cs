using TournamentLibrary.Participants;

namespace TournamentLibrary.TournamentParts
{
    public class RoundPair<T> : IRoundPair<T> where T : class?, IParticipant
    {

        public RoundPair() { }

        public RoundPair(T white, T black) 
        {
            White = white;
            Black = black;    
        }

        public RoundPair(T white, T black, SingleMatchResult result) : this(white, black)
        {
            Result = result;
        }


        public T White { get; set; }
        public T Black { get; set; }

        public SingleMatchResult Result { get; set; } //potřeba upravit na IMatchResult po konzultaci


        public decimal WhiteScore => Result.WhitePoints;

        public decimal BlackScore => Result.BlackPoints;

		public bool? TieBreakWinnerWhite { get; set; }

		public decimal GetScoreOfParticipant(T participant)
        {
            if(participant is null)
            {
                return 0;
            }
            if (participant.Id == White.Id)
            {
                return WhiteScore;
            }
            if(participant.Id == Black.Id)
            {
                return BlackScore;
            }
            throw new ArgumentException("This participant is not a player in this match");
        }

        public decimal GetScoreOfOpponent(T participant)
        {
            if (participant.Id == White.Id)
            {
                return BlackScore;
            }
            if (participant.Id == Black.Id)
            {
                return WhiteScore;
            }
            throw new ArgumentException("This participant is not a player in this match");
        }

        public void SwapParticipants()
        {
            (Black, White) = (White, Black);
        }

        public bool IsDraw => Result.BlackPoints == Result.WhitePoints;

		public T GetLoser()
        {
            if(Result.WhitePoints < Result.BlackPoints)
            {
                return White;
            }
            if(Result.BlackPoints < Result.WhitePoints)
            {
                return Black;
            }
            if(TieBreakWinnerWhite is not null)
            {
                if (TieBreakWinnerWhite.Value)
                {
                    return Black;
                }
                return White;
            }
            throw new Exception("Can't determine winner or loser, use IsDraw method before and check");
        }

		public T GetWinner()
		{
			if (Result.WhitePoints > Result.BlackPoints)
			{
				return White;
			}
			if (Result.BlackPoints > Result.WhitePoints)
			{
				return Black;
			}
			if (TieBreakWinnerWhite is not null)
			{
				if (TieBreakWinnerWhite.Value)
				{
					return White;
				}
				return Black;
			}
			throw new Exception("Can't determine winner or loser, use IsDraw method before and check");
		}


        public T GetOpponent(IParticipant participant)
        {
            if(White.Id == participant.Id)
            {
                return Black;
            }
            if(Black.Id == participant.Id)
            {
                return White;
            }
            throw new Exception("This participant is not a player in this match");
        }


        public bool PlaysInPair(T participant)
        {
            if(participant is null && (White is null || Black is null))
            {
                return true;
            }
            if(participant is null)
            {
                return false;
            }
            if(White?.Id == participant.Id)
            {
                return true;
            }
            if(Black?.Id == participant.Id)
            {
                return true;
            }
            return false;
        }

		/*
        public override string ToString()
        {
            return $"{White.Identifier} vs {Black.Identifier}";
        }
        */
	}
}
