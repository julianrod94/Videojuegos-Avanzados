﻿﻿using StateNS;
 using UnityEngine;

namespace SnapshotsNS.PlayerMovementNS {
    public class PlayerPosition : IInterpolatableState<PlayerPosition> {
        private readonly float _timeStamp;
        public readonly Vector3 Position;
        public readonly int LastInputApplied;
        public readonly int Health;

        public PlayerPosition(float timeStamp, Vector3 position, int lastInputApplied, int health) {
            _timeStamp = timeStamp;
            Position = position;
            LastInputApplied = lastInputApplied;
            Health = health;
        }

        public PlayerPosition UpdateState(float progression, PlayerPosition target) {
            return new PlayerPosition(_timeStamp, Vector3.Lerp(Position, target.Position, progression), target.LastInputApplied, target.Health);
        }

        public float TimeStamp() {
            return _timeStamp;
        }

        public override bool Equals(object obj) {
            //Check for null and compare run-time types.
            if (obj == null || GetType() != obj.GetType()) return false;

            var other = (PlayerPosition) obj;
            return other.TimeStamp().Equals(_timeStamp);
        }

        public override int GetHashCode() {
            return _timeStamp.GetHashCode();
        }
    }
}