﻿using System;
using StateNS;
using UnityEngine;

namespace ChannelNS.Implementations.StateChannels {
    public class CubePositionStateChannel : StateChannel<CubePosition> {
        private readonly Interpolator<CubePosition> _cubeInterpolator = new Interpolator<CubePosition>();

        private readonly IUnityBridgeState<CubePosition> _bridge;
        private readonly float _positionMax = 100;

        private readonly float _positionMin = -100;
        private readonly float _positionPrecision = 0.1f;
        private readonly float _timeStampMax = 60;

        private readonly float _timeStampMin = 0;
        private readonly float _timeStampPrecision = 1 / 60f;

        public CubePositionStateChannel(IUnityBridgeState<CubePosition> bridge, ISenderStrategy strategy, float refreshTime) {
            _bridge = bridge;
            setupStrategy(strategy);
            SetupPeriod((long) (refreshTime * 1000));
            SetupInterpolator(_cubeInterpolator);
            SetupStateProvider(_bridge.GetCurrentState);
        }

        protected override CubePosition DeserializeData(byte[] bytes) {
            float x = 0;
            float y = 0;
            float z = 0;
            float timeStamp = 0;
            try {
                buffer.ToRead();
                x = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                y = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                z = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                timeStamp = buffer.ReadFloat(_timeStampMin, _timeStampMax, _timeStampPrecision);
            } catch (Exception e) {
                Debug.LogError(e);
            }
            return new CubePosition(timeStamp, new Vector3(x, y, z));
        }

        protected override byte[] SerializeData(CubePosition data) {
            try {
                buffer.ToWrite();
                buffer.WriteFloat(data.Position.x, _positionMin, _positionMax, _positionPrecision);
                buffer.WriteFloat(data.Position.y, _positionMin, _positionMax, _positionPrecision);
                buffer.WriteFloat(data.Position.z, _positionMin, _positionMax, _positionPrecision);
                buffer.WriteFloat(data.Position.z, _timeStampMin, _timeStampMax, _timeStampPrecision);
            } catch (Exception e) {
                Debug.LogError(e);
            }
            return buffer.GenerateBytes();
        }
    }
}