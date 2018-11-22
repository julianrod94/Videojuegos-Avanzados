using System;
using System.Collections.Generic;
using ChannelNS;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

namespace SnapshotsNS.GrenadeStateNS {
    public class GrenadesChannel : StateChannel<GrenadesState> {
        private readonly Interpolator<GrenadesState> _cubeInterpolator = new Interpolator<GrenadesState>();

        private readonly IUnityBridgeState<GrenadesState> _bridge;
        
        private readonly float _positionMax = 100;
        private readonly float _positionMin = -100;
        private readonly float _positionPrecision = 0.001f;

        private readonly float _timeStampMin = 0;
        private readonly float _timeStampMax = 600;
        private readonly float _timeStampPrecision = 1 / 60f;

        private readonly int _mingranades = 0;
        private readonly int _maxGranades = 511;
        
                
        private readonly float _minRot = -1;
        private readonly float _maxRot = 1;
        private readonly float _stepRot = 0.2f/180f;

        
        public GrenadesChannel(IUnityBridgeState<GrenadesState> bridge, SenderStrategy strategy, float refreshTime) {
            _bridge = bridge;
            setupStrategy(strategy);
            SetupInterpolator(_cubeInterpolator);
            if (bridge != null) {
                SetupPeriod((long) (refreshTime * 1000));
                SetupStateProvider(_bridge.GetCurrentState);
            }
        }

        protected override GrenadesState DeserializeData(byte[] bytes) {
            lock (this) {
                Dictionary<int, GrenadeState> actions;
                float timeStamp;
                try {
                    buffer.LoadBytes(bytes);
                    buffer.ToRead();

                    timeStamp = buffer.ReadFloat(_timeStampMin, _timeStampMax, _timeStampPrecision);
                    var amount = buffer.ReadInt(_mingranades, _maxGranades);
                    
                    actions = new Dictionary<int, GrenadeState>(amount);
                    for (int i = 0; i < amount; i++) {
                        var grenade = buffer.ReadInt(_mingranades, _maxGranades);

                        var posX = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                        var posY = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                        var posZ = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                        
                        var rotX = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                        var rotY = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                        var rotZ = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                        var rotW = buffer.ReadFloat(_minRot, _maxRot, _stepRot);

                        var isExploding = buffer.ReadBit();
                        
                        actions[grenade] =
                            new GrenadeState(
                                new Vector3(posX, posY, posZ), 
                                new Quaternion(rotX, rotY, rotZ, rotW), 
                                isExploding
                            );
                    }
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw;
                }

                return new GrenadesState(timeStamp, actions);
            }
        }

        protected override byte[] SerializeData(GrenadesState data) {
            lock (this) {
                try {
                    
                    buffer.ToWrite();
                    
                    buffer.WriteFloatRounded(data.TimeStamp(), _timeStampMin, _timeStampMax, _timeStampPrecision);
                    buffer.WriteInt(data._states.Count, _mingranades, _maxGranades);

                    foreach (var grenadeState in data._states) {
                        buffer.WriteInt(grenadeState.Key, _mingranades, _maxGranades);

                        var pData = grenadeState.Value;
                        buffer.WriteFloatRounded(pData.Position.x, _positionMin, _positionMax, _positionPrecision);
                        buffer.WriteFloatRounded(pData.Position.y, _positionMin, _positionMax, _positionPrecision);
                        buffer.WriteFloatRounded(pData.Position.z, _positionMin, _positionMax, _positionPrecision);
                        buffer.WriteFloatRounded(pData.Rotation.x, _minRot, _maxRot, _stepRot);
                        buffer.WriteFloatRounded(pData.Rotation.y, _minRot, _maxRot, _stepRot);
                        buffer.WriteFloatRounded(pData.Rotation.z, _minRot, _maxRot, _stepRot);
                        buffer.WriteFloatRounded(pData.Rotation.w, _minRot, _maxRot, _stepRot);
                        buffer.WriteBool(pData.IsExploding);
                    }
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw;
                }

                return buffer.GenerateBytes();
            }
        }
    }
}