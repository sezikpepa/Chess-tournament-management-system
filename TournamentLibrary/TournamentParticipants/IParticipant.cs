namespace TournamentLibrary.Participants
{
    /// <summary>
    /// Participants of tournaments (other classes) have to implement this interface to be able to play in 
    /// the tournament
    /// </summary>
    public interface IParticipant
    {
        /// <summary>
        /// Identifier in the tournament, it can be seen in RoundDraws, participants can recognize them
        /// by this Identifier and find their opponents
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// How the entity should be identifiable in UI -> team of the team, surname of player...
        /// </summary>
        public string DisplayName { get; }
    }
}
