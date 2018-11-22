using UnityEngine;

namespace SnapshotsNS.GrenadeStateNS {
    public class GrenadeState {
        public readonly Vector3 Position;
        public readonly Quaternion Rotation;
        public readonly bool IsExploding;

        public GrenadeState(Vector3 position, Quaternion rotation, bool isExploding) {
            Position = position;
            Rotation = rotation;
            IsExploding = isExploding;
        }
        
        public GrenadeState UpdateState(float progression, GrenadeState target) {
            return new GrenadeState(
                Vector3.Lerp(Position, target.Position, progression),
                Quaternion.SlerpUnclamped(Rotation, target.Rotation, progression),
                target.IsExploding
                );
        }
    }
}