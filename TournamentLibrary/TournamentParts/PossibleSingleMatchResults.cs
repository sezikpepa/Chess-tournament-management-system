namespace TournamentLibrary.TournamentParts
{

    public static class PossibleSingleMatchResults
    {

        public static SingleMatchResult GetSingleMatchResult(string name)
        {
            return name switch
            {
                nameof(Undefined) => Undefined,
                nameof(WhiteWins) => WhiteWins,
                nameof(BlackWins) => BlackWins,
                nameof(Draw) => Draw,
                _ => throw new ArgumentException(),
            };
        }

        public static SingleMatchResult Undefined = new SingleMatchResult(0, 0);
        public static SingleMatchResult WhiteWins = new SingleMatchResult(1, 0);
        public static SingleMatchResult BlackWins = new SingleMatchResult(0, 1);
        public static SingleMatchResult Draw = new SingleMatchResult(0.5m, 0.5m);

     
    }
}
