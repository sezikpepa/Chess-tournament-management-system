using Microsoft.IdentityModel.Tokens;
using System.ComponentModel;
using System.Globalization;

namespace TournamentLibrary.TournamentParts
{
    public record struct SingleMatchResult : IMatchResult
    {
        public decimal WhitePoints { get; set; }
        public decimal BlackPoints { get; set; }

        public bool NotYetSet { get; set; }

        public SingleMatchResult(decimal whitePoints, decimal blackPoints) 
        { 
            WhitePoints = whitePoints;
            BlackPoints = blackPoints;   
            NotYetSet = false;
        }

        public SingleMatchResult(decimal whitePoints, decimal blackPoints, bool notYetSet)
        {
            WhitePoints = whitePoints;
            BlackPoints = blackPoints;
            NotYetSet = notYetSet;
        }

        public SingleMatchResult(string result) 
        {
            if (result.IsNullOrEmpty() || result == "Unknown" || result == "Unknown result" || result == "0")
            {
                WhitePoints = 0; 
                BlackPoints = 0;
                NotYetSet = true;
            }
            else
            {
                var splitted = result.Split(':');
                WhitePoints = decimal.Parse(splitted[0], new CultureInfo("en-US"));
                BlackPoints = decimal.Parse(splitted[1], new CultureInfo("en-US"));
                NotYetSet = false;
            }
        }

        public readonly bool IsScoreEqual => WhitePoints == BlackPoints;

        public override string ToString()
        {
            if(NotYetSet)
            {
                return "Unknown result";
            }
            return $"{WhitePoints.ToString(new CultureInfo("en-US"))}:{BlackPoints.ToString(new CultureInfo("en-US"))}";
        }

        /// <summary>
        /// Returns string reprezentation of the result, but keeps the lowest possible decimal numbers
        /// </summary>
        /// <returns></returns>
        public string ToShortString()
        {
            if (NotYetSet)
            {
                return "Unknown result";
            }
            return $"{WhitePoints.ToString("0.######")}:{BlackPoints.ToString("0.######")}";
        }
    }
}
