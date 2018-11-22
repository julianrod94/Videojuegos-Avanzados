using System.Collections.Generic;
using StateNS;

namespace SnapshotsNS.GrenadeStateNS {
    public class GrenadesState: IInterpolatableState<GrenadesState> {
        public Dictionary<int, GrenadeState> _states;
        private readonly float _timeStamp;


        public GrenadesState(float timeStamp, Dictionary<int, GrenadeState>  states) {
            _timeStamp = timeStamp;
            _states = states;
        }

        public GrenadesState UpdateState(float progression, GrenadesState target) {
            var newDic = new Dictionary<int, GrenadeState>();
            foreach (var kp in target._states) {
                if (target._states.ContainsKey(kp.Key)) {
                    if (_states.ContainsKey(kp.Key)) {
                        newDic[kp.Key] = _states[kp.Key].UpdateState(progression, kp.Value);
                    } else {
                        newDic[kp.Key] = target._states[kp.Key];
                    }
                }
            }

            return new GrenadesState(_timeStamp, newDic);
        }

        public float TimeStamp() {
            return _timeStamp;
        }

        public override bool Equals(object obj) {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            var other = (GrenadesState) obj;
            return other.TimeStamp().Equals(_timeStamp);
        }

        public override int GetHashCode() {
            return _timeStamp.GetHashCode();
        }
    
    }
}

