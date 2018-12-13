using System;
using ChannelNS;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

namespace SnapshotsNS.PlayerMovementNS {
    public class PlayerPositionStateChannel : StateChannel<PlayerPosition> {
        private readonly Interpolator<PlayerPosition> _cubeInterpolator = new Interpolator<PlayerPosition>();

        private readonly IUnityBridgeState<PlayerPosition> _bridge;
        
        private readonly float _positionMax = 120;
        private readonly float _positionMin = -120;
        private readonly float _positionPrecision = 0.001f;

        private readonly float _timeStampMin = 0;
        private readonly float _timeStampMax = 600;
        private readonly float _timeStampPrecision = 1 / 60f;
        
        private readonly int _minInputNumber = 0;
        private readonly int _maxInputNumber = 10000;
        
        private readonly int _minHealthNumber = 0;
        private readonly int _maxHealthNumber = 3;

        public PlayerPositionStateChannel(
            IUnityBridgeState<PlayerPosition> bridge,
            SenderStrategy strategy,
            float refreshTime) {
            _bridge = bridge;
            setupStrategy(strategy);
            SetupPeriod((long) (refreshTime * 1000));
            SetupInterpolator(_cubeInterpolator);
            SetupStateProvider(_bridge.GetCurrentState);
        }

        protected override PlayerPosition DeserializeData(byte[] bytes) {
            lock (this) {
                float x = 0;
                float y = 0;
                float z = 0;
                int lastInput = 0;
                float timeStamp = 0;
                int health = 0;
                try {
                    buffer.LoadBytes(bytes);
                    buffer.ToRead();
                    x = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                    y = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                    z = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                    timeStamp = buffer.ReadFloat(_timeStampMin, _timeStampMax, _timeStampPrecision);
                    lastInput = buffer.ReadInt(_minInputNumber, _maxInputNumber);
                    health = buffer.ReadInt(_minHealthNumber, _maxHealthNumber);
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw;
                }

                return new PlayerPosition(timeStamp, new Vector3(x, y, z), lastInput, health);
            }
        }

        protected override byte[] SerializeData(PlayerPosition data) {
            lock (this) {
                try {
                    buffer.ToWrite();
                    buffer.WriteFloatRounded(data.Position.x, _positionMin, _positionMax, _positionPrecision);
                    buffer.WriteFloatRounded(data.Position.y, _positionMin, _positionMax, _positionPrecision);
                    buffer.WriteFloatRounded(data.Position.z, _positionMin, _positionMax, _positionPrecision);
                    buffer.WriteFloatRounded(data.TimeStamp(), _timeStampMin, _timeStampMax, _timeStampPrecision);
                    buffer.WriteInt(data.LastInputApplied, _minInputNumber, _maxInputNumber);
                    buffer.WriteInt(data.Health, _minHealthNumber, _maxHealthNumber);
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw;
                }

                return buffer.GenerateBytes();
            }
        }
    }
}