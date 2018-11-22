using System.Collections.Generic;
using StateNS;

namespace SnapshotsNS.OtherPlayers {
    public class OtherPlayersStates: IInterpolatableState<OtherPlayersStates> {
        public Dictionary<int, OtherPlayerState> _states;
        private readonly float _timeStamp;


        public OtherPlayersStates(float timeStamp, Dictionary<int, OtherPlayerState>  states) {
            _timeStamp = timeStamp;
            _states = states;
        }

        public OtherPlayersStates UpdateState(float progression, OtherPlayersStates target) {
            var newDic = new Dictionary<int, OtherPlayerState>();
            foreach (var kp in target._states) {
                if (target._states.ContainsKey(kp.Key)) {
                    if (_states.ContainsKey(kp.Key)) {
                        newDic[kp.Key] = _states[kp.Key].UpdateState(progression, kp.Value);
                    } else {
                        newDic[kp.Key] = target._states[kp.Key];
                    }
                }
            }

            return new OtherPlayersStates(_timeStamp, newDic);
        }

        public float TimeStamp() {
            return _timeStamp;
        }

        public override bool Equals(object obj) {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            var other = (OtherPlayersStates) obj;
            return other.TimeStamp().Equals(_timeStamp);
        }

        public override int GetHashCode() {
            return _timeStamp.GetHashCode();
        }
        
    }
}