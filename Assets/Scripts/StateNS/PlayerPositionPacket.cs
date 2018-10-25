using UnityEngine;

namespace StateNS {
    public class PlayerPositionState: IInterpolatableState {
        public Vector3 Position;
        private readonly float _timeStamp;

        public PlayerPositionState(Vector3 position, float timeStamp) {
            Position = position;
            _timeStamp = timeStamp;
        }

        public void UpdateState(float progression, IInterpolatableState target) {
            var castedTarget = (PlayerPositionState) target;
            Position = Vector3.Lerp(Position, castedTarget.Position, progression);
        }

        public float TimeStamp() {
            return _timeStamp;
        }
    
        public override bool Equals(object obj) {
            //Check for null and compare run-time types.
            if ((obj == null) || GetType() != obj.GetType()) {
                return false;
            } else { 
                var other = (CubePosition) obj;
                return other.TimeStamp().Equals(TimeStamp());
            }   
        }

        public override int GetHashCode() {
            return TimeStamp().GetHashCode();
        }
    }
}
