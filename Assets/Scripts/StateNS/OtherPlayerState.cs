using UnityEngine;

namespace StateNS {
    public class OtherPlayerState {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;

        public OtherPlayerState(Vector3 position, Quaternion rotation) {
            Position = position;
            Rotation = rotation;
        }
        
        public OtherPlayerState UpdateState(float progression, OtherPlayerState target) {
            return new OtherPlayerState(
                Vector3.Lerp(Position, target.Position, progression),
                target.Rotation
                );
        }
    }
}