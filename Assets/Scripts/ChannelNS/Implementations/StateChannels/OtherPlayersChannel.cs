﻿using System;
using System.Collections.Generic;
using InputManagerNS;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

namespace ChannelNS.Implementations.StateChannels {
    public class OtherPlayersChannel : StateChannel<OtherPlayersStates> {
        private readonly Interpolator<OtherPlayersStates> _cubeInterpolator = new Interpolator<OtherPlayersStates>();

        private readonly IUnityBridgeState<OtherPlayersStates> _bridge;
        
        private readonly float _positionMax = 100;
        private readonly float _positionMin = -100;
        private readonly float _positionPrecision = 0.001f;

        private readonly float _timeStampMin = 0;
        private readonly float _timeStampMax = 600;
        private readonly float _timeStampPrecision = 1 / 60f;

        private readonly int _minPlayers = 0;
        private readonly int _maxPlayers = 7;
        
        private readonly float _minRot = 0;
        private readonly float _maxRot = 360;
        private readonly float _stepRot = 0.2f;

        public OtherPlayersChannel(IUnityBridgeState<OtherPlayersStates> bridge, SenderStrategy strategy, float refreshTime) {
            _bridge = bridge;
            setupStrategy(strategy);
            SetupInterpolator(_cubeInterpolator);
            if (bridge != null) {
                SetupPeriod((long) (refreshTime * 1000));
                SetupStateProvider(_bridge.GetCurrentState);
            }
        }

        protected override OtherPlayersStates DeserializeData(byte[] bytes) {
            lock (this) {
                Dictionary<int, OtherPlayerState> actions;
                float timeStamp;
                try {
                    buffer.LoadBytes(bytes);
                    buffer.ToRead();

                    timeStamp = buffer.ReadFloat(_timeStampMin, _timeStampMax, _timeStampPrecision);
                    var amount = buffer.ReadInt(_minPlayers, _maxPlayers);
                    
                    actions = new Dictionary<int, OtherPlayerState>(amount);
                    for (int i = 0; i < amount; i++) {
                        var player = buffer.ReadInt(_minPlayers, _maxPlayers);

                        var posX = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                        var posY = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                        var posZ = buffer.ReadFloat(_positionMin, _positionMax, _positionPrecision);
                        
                        var rotX = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                        var rotY = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                        var rotZ = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                        
                        
                        actions[player] =
                            new OtherPlayerState(
                                new Vector3(posX, posY, posZ),
                                Quaternion.Euler(rotX, rotY, rotZ)
                            );
                    }
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw;
                }

                return new OtherPlayersStates(timeStamp, actions);
            }
        }

        protected override byte[] SerializeData(OtherPlayersStates data) {
            Debug.LogError("ewqewqewqewq");
            lock (this) {
                try {
                    
                    buffer.ToWrite();
                    if (data._states.Count > 7) {
                        throw new ExecutionEngineException("NO MORE THAN 7 PLayers allowed");
                    }
                    
                    buffer.WriteFloatRounded(data.TimeStamp(), _timeStampMin, _timeStampMax, _timeStampPrecision);
                    buffer.WriteInt(data._states.Count, _minPlayers, _maxPlayers);

                    foreach (var otherPlayerState in data._states) {
                        buffer.WriteInt(otherPlayerState.Key, _minPlayers, _maxPlayers);

                        var pData = otherPlayerState.Value;
                        buffer.WriteFloatRounded(pData.Position.x, _positionMin, _positionMax, _positionPrecision);
                        buffer.WriteFloatRounded(pData.Position.y, _positionMin, _positionMax, _positionPrecision);
                        buffer.WriteFloatRounded(pData.Position.z, _positionMin, _positionMax, _positionPrecision);
                        buffer.WriteFloatRounded(Mathf.Abs(pData.Rotation.x%360), _minRot, _maxRot, _stepRot);
                        buffer.WriteFloatRounded(Mathf.Abs(pData.Rotation.y%360), _minRot, _maxRot, _stepRot);
                        buffer.WriteFloatRounded(Mathf.Abs(pData.Rotation.z%360), _minRot, _maxRot, _stepRot);
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