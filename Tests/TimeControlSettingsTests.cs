using ChessTournamentMate.Shared.AvailableValues;
using DatabaseCommunicator.Models;
using TournamentLibrary;
using TournamentLibrary.Extensions;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;
using TournamentLibrary.Tournaments;

namespace Tests
{
    public class TimeControlSettingsTests
    {
        [Fact]
        public void TestGetTournamentTimeTypeNoElements()
        {
			TimeControlSettings timeSettings = new TimeControlSettings();

            TournamentTimeTypes timeType = timeSettings.GetTournamentTimeType();

			Assert.Equal(TournamentTimeTypes.None, timeType);
		}

        [Fact]
        public void TestGetTournamentTimeTypeSingleElement()
        {
            TimeControlSettings timeSettings = new TimeControlSettings()
            {
                Elements = [new TimeControlSettingsPiece() { AvailableTimeHours = 0, AvailableTimeMinutes = 10, AvailableTimeSeconds = 0, Increment = 3 }]
            };

            TournamentTimeTypes timeType = timeSettings.GetTournamentTimeType();

            Assert.Equal(TournamentTimeTypes.Blitz, timeType);
        }

        [Fact]
        public void TestGetTournamentTimeTypeMultipleElements1()
        {
            TimeControlSettings timeSettings = new TimeControlSettings()
            {
                Elements = [new TimeControlSettingsPiece() { AvailableTimeHours = 0, AvailableTimeMinutes = 10, AvailableTimeSeconds = 0, Increment = 3 },
                            new TimeControlSettingsPiece() { AvailableTimeHours = 0, AvailableTimeMinutes = 15, AvailableTimeSeconds = 0, Increment = 3 }]
            };

            TournamentTimeTypes timeType = timeSettings.GetTournamentTimeType();

            Assert.Equal(TournamentTimeTypes.Rapid, timeType);
        }

        [Fact]
        public void TestGetTournamentTimeTypeMultipleElements2()
        {
            TimeControlSettings timeSettings = new TimeControlSettings()
            {
                Elements = [new TimeControlSettingsPiece() { AvailableTimeHours = 0, AvailableTimeMinutes = 53, AvailableTimeSeconds = 0, Increment = 3 },
                            new TimeControlSettingsPiece() { AvailableTimeHours = 0, AvailableTimeMinutes = 15, AvailableTimeSeconds = 0, Increment = 3 }]
            };

            TournamentTimeTypes timeType = timeSettings.GetTournamentTimeType();

            Assert.Equal(TournamentTimeTypes.Classic, timeType);
        }
    }
}
