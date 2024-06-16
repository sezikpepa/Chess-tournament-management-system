using System.ComponentModel;
using TournamentLibrary.Participants;
using TournamentLibrary.TournamentParts;

namespace TournamentLibrary.RoundHandlers
{
    public class RoundRobinRoundHandler<T> where T : class, IParticipant
    {

        private List<T> _list1;
        private List<T> _list2;
        private T _fixedPlayer;

        private int _moveToNextRoundIterations = 0;


        public RoundDraw<T> CurrentRoundDraw { get; set; }

        public RoundRobinRoundHandler(List<T> participants) 
        {
            if(participants.Count <= 1)
            {
                throw new ArgumentException($"Tournament makes sence when participants count is higher than 1, participants count was {participants.Count}");
            }

            _list1 = [];
            _list2 = [];

            FillQueuesAndFixedPlayer(participants);
        }

        private void FillQueuesAndFixedPlayer(List<T> participants)
        {
            for (int i = 0; i < participants.Count; i++)
            {
                if (i % 2 == 0)
                {
                    _list1.Add(participants[i]);
                }
                else
                {
                    _list2.Add(participants[i]);
                }
            }

            if(_list1.Count == _list2.Count)
            {
                _fixedPlayer = _list2[0];
                _list2.RemoveAt(0);
            }
            
            
        }

        public void MoveToNextRound()
        {
            try
            {
                _list1.Add(_list2[0]);
                _list2.RemoveAt(0);

                _list2.Add(_list1[0]);
                _list1.RemoveAt(0);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
            

            _moveToNextRoundIterations++;
        }


        public void GenerateCurrentRound()
        {
            RoundDraw<T> roundDraw = new();

            for (int i = 0; i < _list2.Count; i++)
            {
                RoundPair<T> newRoundPair = new()
                {
                    White = _list1[i],
                    Black = _list2[^(i + 1)]
                };

                if (i % 2 == 0)
                    newRoundPair.SwapParticipants();

                roundDraw.AddPair(newRoundPair);
            }

            if(_fixedPlayer is not null)
            {
                RoundPair<T> roundPairWithFixedPlayer = new()
                {
                    White = _list1[^1],
                    Black = _fixedPlayer
                };

                if (_moveToNextRoundIterations % 2 != 0)
                    roundPairWithFixedPlayer.SwapParticipants();


                roundDraw.AddPair(roundPairWithFixedPlayer);
            }
            


            CurrentRoundDraw = roundDraw;
        }
    }



}
