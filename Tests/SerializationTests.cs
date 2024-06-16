using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;

namespace Tests
{
	public class SerializationTests
    {

        [Fact] 
        public void SerializationMatchResult() 
        {
            SingleMatchResult undefined = PossibleSingleMatchResults.Undefined;
            SingleMatchResult whiteWins = PossibleSingleMatchResults.WhiteWins;
            SingleMatchResult blackWins = PossibleSingleMatchResults.BlackWins;
            SingleMatchResult draw = PossibleSingleMatchResults.Draw;


            string undefinedSerialize = System.Text.Json.JsonSerializer.Serialize(undefined);
            string whiteWinsSerialize = System.Text.Json.JsonSerializer.Serialize(whiteWins);
            string blackWinsSerialize = System.Text.Json.JsonSerializer.Serialize(blackWins);
            string drawSerialize = System.Text.Json.JsonSerializer.Serialize(draw);


            SingleMatchResult? undefinedDeserialize = System.Text.Json.JsonSerializer.Deserialize<SingleMatchResult>(undefinedSerialize);
            SingleMatchResult? whiteWinsDeserialize = System.Text.Json.JsonSerializer.Deserialize<SingleMatchResult>(whiteWinsSerialize);
            SingleMatchResult? blackWinsDeserialize = System.Text.Json.JsonSerializer.Deserialize<SingleMatchResult>(blackWinsSerialize);
            SingleMatchResult? drawDeserialize = System.Text.Json.JsonSerializer.Deserialize<SingleMatchResult>(drawSerialize);

            Assert.Equal(undefined, undefinedDeserialize);
            Assert.Equal(whiteWins, whiteWinsDeserialize);
            Assert.Equal(blackWins, blackWinsDeserialize);
            Assert.Equal(draw, drawDeserialize);
        }


        [Fact]
        public void SerializationTournamentPlayer()
        {
            TournamentPlayer player = MockMethods.GenerateTournamentPlayer();

            string serializePlayer = System.Text.Json.JsonSerializer.Serialize(player);

            var deserializePlayer = System.Text.Json.JsonSerializer.Deserialize<TournamentPlayer>(serializePlayer); 
            
            Assert.Equal(player.Id, deserializePlayer.Id);
            Assert.Equal(player.FirstName, deserializePlayer.FirstName);
            Assert.Equal(player.MiddleName, deserializePlayer.MiddleName);
            Assert.Equal(player.LastName, deserializePlayer.LastName);
        }

        [Fact]
        public void SerializationTournamentPair()
        {
            RoundPair<TournamentPlayer> pair = new RoundPair<TournamentPlayer>(MockMethods.GenerateTournamentPlayer(), MockMethods.GenerateTournamentPlayer());

            string serializePair = System.Text.Json.JsonSerializer.Serialize(pair);

            RoundPair<TournamentPlayer>? deserializePair = System.Text.Json.JsonSerializer.Deserialize<RoundPair<TournamentPlayer>>(serializePair);

            Assert.Equal(deserializePair.White.Id, pair.White.Id);
            Assert.Equal(deserializePair.White.FirstName, pair.White.FirstName);
            Assert.Equal(deserializePair.White.MiddleName, pair.White.MiddleName);
            Assert.Equal(deserializePair.White.LastName, pair.White.LastName);

            Assert.Equal(deserializePair.Black.Id, pair.Black.Id);
            Assert.Equal(deserializePair.Black.FirstName, pair.Black.FirstName);
            Assert.Equal(deserializePair.Black.MiddleName, pair.Black.MiddleName);
            Assert.Equal(deserializePair.Black.LastName, pair.Black.LastName);

            Assert.Equal(pair.Result, deserializePair.Result);
        }



    }
}
