using ChessTournamentMate.Shared.AvailableValues;
using DatabaseCommunicator.Models;
using Neo4j.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentLibrary.Participants;

namespace DatabaseCommunicator.ModelsManipulators
{
    public static class MappingFromDatabase
    {

        public static Player? MapPlayer(INode node)
        {
            if(node is null)
            {
                return null;
            }

            var id = node["Id"].As<string>();
            var firstName = node["FirstName"].As<string>();
            var lastName = node["LastName"].As<string>();

            return new Player() { Id = id, FirstName = firstName, LastName = lastName };
        }



        public static TournamentPlayer MapTournamentPlayer(INode node)
        {
            var id = node["Id"].As<string>();
            var firstName = node["FirstName"].As<string>();
            var lastName = node["LastName"].As<string>();

            return new TournamentPlayer() { Id = id, FirstName = firstName, LastName = lastName };
        }

        public static Tournament MapTournament(INode node)
        {
            var Id = node["Id"].As<string>();
            var TournamentName = node["TournamentName"].As<string>();
            var ShortDescription = node.Properties.ContainsKey("ShortDescription") ? node["ShortDescription"].As<string>() : " ";
            var StartDate = node["StartDate"].As<DateTime>();
            DateTime EndDate = node["EndDate"].As<DateTime>();
            var CurrentRound = node["CurrentRound"].As<int>();
            int? ExpectedNumberOfRounds = node.Properties.ContainsKey("ExpectedNumberOfRounds") ? node["ExpectedNumberOfRounds"].As<int>() : null;
            var HasStarted = node.Properties.ContainsKey("HasStarted") ? node["HasStarted"].As<bool>() : false;
            bool IsTeam = node.Properties.ContainsKey("IsTeam") ? node["IsTeam"].As<bool>() : false;
            string? ProfilePictureName = node.Properties.ContainsKey("ProfilePictureName") ? node["ProfilePictureName"].As<string>() : null;

			TournamentTypes TournamentType = (TournamentTypes)Enum.Parse(typeof(TournamentTypes), node["TournamentType"].As<string>());

            return new Tournament()
            {
                Id = Id,
                TournamentName = TournamentName,
                ShortDescription = ShortDescription,
                StartDate = StartDate,
                EndDate = EndDate,
                CurrentRound = CurrentRound,
                ExpectedNumberOfRounds = ExpectedNumberOfRounds,
                HasStarted = HasStarted,
                TournamentType = TournamentType,
                IsTeam = IsTeam,
                ProfilePictureName = ProfilePictureName,
            };
        }


        public static Team? MapTeam(INode node)
        {
            if(node is null)
            {
                return null;
            }

            var Id = node["Id"].As<string>();
            var TeamName = node["Name"].As<string>();

            return new Team()
            {
                Id = Id,
                Name = TeamName
            };
        }
    }
}
