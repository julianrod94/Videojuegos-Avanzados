using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StateNS {
    public class OtherPlayersStates: IInterpolatableState<OtherPlayersStates> {
        public Dictionary<int, OtherPlayerState> _states;
        private readonly float _timeStamp;


        public OtherPlayersStates(float timeStamp, Dictionary<int, OtherPlayerState>  states) {
            _timeStamp = timeStamp;
            _states = states;
        }

        public OtherPlayersStates UpdateState(float progression, OtherPlayersStates target) {
            var newDic = new Dictionary<int, OtherPlayerState>();
            foreach (var kp in _states) {
                newDic[kp.Key] = kp.Value.UpdateState(progression, target._states[kp.Key]);
            }

            return new OtherPlayersStates(_timeStamp, newDic);
        }

        public float TimeStamp() {
            return _timeStamp;
        }

        public override bool Equals(object obj) {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            var other = (CubePosition) obj;
            return other.TimeStamp().Equals(_timeStamp);
        }

        public override int GetHashCode() {
            return _timeStamp.GetHashCode();
        }
        
    }
}