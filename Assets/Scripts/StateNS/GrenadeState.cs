using UnityEngine;

namespace StateNS {
    public class GrenadeState {
        public readonly Vector3 Position;
        public readonly bool IsExploding;

        public GrenadeState(Vector3 position, bool isExploding) {
            Position = position;
            IsExploding = isExploding;
        }
        
        public GrenadeState UpdateState(float progression, GrenadeState target) {
            return new GrenadeState(
                Vector3.Lerp(Position, target.Position, progression),
                target.IsExploding
                );
        }
    }
}