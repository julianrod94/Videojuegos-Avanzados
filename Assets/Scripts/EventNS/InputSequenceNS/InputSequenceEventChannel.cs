﻿using System;
using System.Collections.Generic;
using ChannelNS;
using SenderStrategyNS;
using UnityEngine;

namespace EventNS.InputSequenceNS {
    public class InputSequenceEventChannel: EventChannel<ICollection<PlayerAction>> {
        private readonly int _minInputs = 0;
        private readonly int _maxInputs = 255;
        
        private readonly int _minInputNumber = 0;
        private readonly int _maxInputNumber = 1024*8-1;
        
        private readonly float _minDt = 0;
        private readonly float _maxDT = 1/10f;
        private readonly float _stepDT = 1/100f;

        private readonly float _minRot = 0;
        private readonly float _maxRot = 360;
        private readonly float _stepRot = 0.2f;
        

        private readonly int _inputCommands = Enum.GetValues(typeof(InputEnum)).Length;

        public InputSequenceEventChannel(Action<ICollection<PlayerAction>> eventReceiver, SenderStrategy strategy) {
            SetupEventReceiver(eventReceiver);
            setupStrategy(strategy);
        }
        
        protected override ICollection<PlayerAction> DeserializeData(byte[] bytes) {
            lock (this) {
                List<PlayerAction> actions;
                try {
                    buffer.LoadBytes(bytes);
                    buffer.ToRead();

                    var amount = buffer.ReadInt(_minInputs, _maxInputs);
                    actions = new List<PlayerAction>(amount);
                    for (int i = 0; i < amount; i++) {
                        var code = (InputEnum)buffer.ReadInt(0, _inputCommands);
                        var number = buffer.ReadInt(_minInputNumber, _maxInputNumber);
                        var dT = buffer.ReadFloat(_minDt, _maxDT, _stepDT);
                        var rotX = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                        var rotY = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                        var rotZ = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                        actions.Add(new PlayerAction(code, number, dT, Quaternion.Euler(rotX, rotY, rotZ)));
                    }
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw;
                }

                return actions;
            }
        }

        protected override byte[] SerializeData(ICollection<PlayerAction> data) {
            lock (this) {
                try {
                    buffer.ToWrite();
                    int inputToSend = data.Count;
                    if (data.Count > _maxInputs) {
                        inputToSend = _maxInputs;
                    }
                    
                    buffer.WriteInt(inputToSend, _minInputs, _maxInputs);
                    int i = 0;
                    foreach (var playerAction in data) {
                        if (i > _maxInputs) {
                            break;
                        }

                        i++;
                        buffer.WriteInt((int)playerAction.inputCommand, 0, _inputCommands);
                        buffer.WriteInt(playerAction.inputNumber, _minInputNumber, _maxInputNumber);
                        buffer.WriteFloatRounded(playerAction.deltaTime, _minDt, _maxDT, _stepDT);
                        var euler = playerAction.rotation.eulerAngles;
                        buffer.WriteFloatRounded(euler.x, _minRot, _maxRot, _stepRot);
                        buffer.WriteFloatRounded(euler.y, _minRot, _maxRot, _stepRot);
                        buffer.WriteFloatRounded(euler.z, _minRot, _maxRot, _stepRot);
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