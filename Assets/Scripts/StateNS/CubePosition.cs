using UnityEngine;

namespace StateNS {
    public class CubePosition : IInterpolatableState<CubePosition> {
        private readonly float _timeStamp;
        public Vector3 Position;

        public CubePosition(float timeStamp, Vector3 position) {
            _timeStamp = timeStamp;
            Position = position;
        }

        public CubePosition UpdateState(float progression, CubePosition target) {
            return new CubePosition(_timeStamp, Vector3.Lerp(Position, target.Position, progression));
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