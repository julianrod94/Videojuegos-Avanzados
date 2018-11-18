using System;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

namespace ChannelNS.Implementations.StateChannels {
    public class CubePositionStateChannel : StateChannel<CubePosition> {
        private readonly Interpolator<CubePosition> _cubeInterpolator = new Interpolator<CubePosition>();

        private readonly IUnityBridgeState<CubePosition> _bridge;
        
        private readonly float _positionMax = 100;
        private readonly float _positionMin = -100;
        private readonly float _positionPrecision = 0.001f;

        private readonly float _timeStampMin = 0;
        private readonly float _timeStampMax = 600;
        private readonly float _timeStampPrecision = 1 / 60f;
        
        private readonly int _minInputNumber = 0;
        private readonly int _maxInputNumber = 10000;
        
        private readonly float _minRot = 0;
        private readonly float _maxRot = 360;
        private readonly float _stepRot = 0.2f;

        public CubePositionStateChannel(
            IUnityBridgeState<CubePosition> bridge,
            SenderStrategy strategy,
            float refreshTime) {
            _bridge = bridge;
            setupStrategy(strategy);
            SetupPeriod((long) (refreshTime * 1000));
            SetupInterpolator(_cubeInterpolator);
            SetupStateProvider(_bridge.GetCurrentState);
        }

        protected override CubePosition DeserializeData(byte[] bytes) {
            lock (this) {
                float x = 0;
                float y = 0;
                float z = 0;
                float rotx = 0;
                float roty = 0;
                float rotz = 0;
                int lastInput = 0;
                float timeStamp = 0;
                try {
                    buffer.LoadBytes(bytes);
                    buffer.ToRead();
                    x = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                    y = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                    z = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                    
                    rotx = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                    roty = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                    rotz = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                    timeStamp = buffer.ReadFloat(_timeStampMin, _timeStampMax, _timeStampPrecision);
                    lastInput = buffer.ReadInt(_minInputNumber, _maxInputNumber);
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw;
                }

                return new CubePosition(timeStamp, new Vector3(x, y, z), Quaternion.Euler(rotx,roty,rotz), lastInput);
            }
        }

        protected override byte[] SerializeData(CubePosition data) {
            lock (this) {
                try {
                    buffer.ToWrite();
                    buffer.WriteFloatRounded(data.Position.x, _positionMin, _positionMax, _positionPrecision);
                    buffer.WriteFloatRounded(data.Position.y, _positionMin, _positionMax, _positionPrecision);
                    buffer.WriteFloatRounded(data.Position.z, _positionMin, _positionMax, _positionPrecision);
                    buffer.WriteFloatRounded(data.Rotation.eulerAngles.x%360, _minRot, _maxRot, _stepRot);
                    buffer.WriteFloatRounded(data.Rotation.eulerAngles.y%360, _minRot, _maxRot, _stepRot);
                    buffer.WriteFloatRounded(data.Rotation.eulerAngles.z%360, _minRot, _maxRot, _stepRot);
                    buffer.WriteFloatRounded(data.TimeStamp(), _timeStampMin, _timeStampMax, _timeStampPrecision);
                    buffer.WriteInt(data.LastInputApplied, _minInputNumber, _maxInputNumber);
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw;
                }

                return buffer.GenerateBytes();
            }
        }
    }
}