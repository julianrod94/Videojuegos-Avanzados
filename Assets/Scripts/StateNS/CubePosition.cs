using UnityEngine;

namespace StateNS {
    public class CubePosition: IInterpolatableState {
        private readonly float _timeStamp;
        public Vector3 Position;
        public GameObject cube;

        public CubePosition(GameObject cube, float timeStamp, Vector3 position) {
            this.cube = cube;
            _timeStamp = timeStamp;
            this.Position = position;
        }

        public void UpdateState(float progression, IInterpolatableState target) {
            var nextPosition = (CubePosition) target;
            cube.transform.position = Vector3.Lerp(Position, nextPosition.Position, progression);
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
                return other.TimeStamp().Equals(_timeStamp);
            }   
        }

        public override int GetHashCode() {
            return _timeStamp.GetHashCode();
        }
    }
}
