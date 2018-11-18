using UnityEngine;

namespace StateNS {
    public class CubePosition : IInterpolatableState<CubePosition> {
        private readonly float _timeStamp;
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly int LastInputApplied;

        public CubePosition(float timeStamp, Vector3 position, Quaternion rotation, int lastInputApplied) {
            _timeStamp = timeStamp;
            Position = position;
            Rotation = rotation;
            LastInputApplied = lastInputApplied;
        }

        public CubePosition UpdateState(float progression, CubePosition target) {
            return new CubePosition(_timeStamp, 
                Vector3.Lerp(Position, target.Position, progression),
                Rotation,
                target.LastInputApplied
                );
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