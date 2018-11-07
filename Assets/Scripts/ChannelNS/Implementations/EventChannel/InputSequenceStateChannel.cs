using System;
using System.Collections.Generic;
using System.Linq;
using InputManagerNS;
using SenderStrategyNS;
using UnityEngine;

namespace ChannelNS.Implementations.EventChannel {
    public class InputSequenceStateChannel: EventChannel<ICollection<PlayerAction>> {
        private readonly int _minInputs = 0;
        private readonly int _maxInputs = 255;
        
        private readonly int _minInputNumber = 0;
        private readonly int _maxInputNumber = 1024*8-1;
        
        private readonly float _minDt = 0;
        private readonly float _maxDT = 1/10f;
        private readonly float _stepDT = 1/100f;

        private readonly int _inputCommands = Enum.GetValues(typeof(InputEnum)).Length;

        public InputSequenceStateChannel(Action<ICollection<PlayerAction>> eventReceiver, SenderStrategy strategy) {
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
                        actions.Add(new PlayerAction(code, number, dT));
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
                    if (data.Count > _maxInputs) {
                        throw new ExecutionEngineException("Too many inputs on the queue");
                    }
                    
                    buffer.WriteInt(data.Count, _minInputs, _maxInputs);
                    foreach (var playerAction in data) {
                        buffer.WriteInt((int)playerAction.inputCommand, 0, _inputCommands);
                        buffer.WriteInt(playerAction.inputNumber, _minInputNumber, _maxInputNumber);
                        buffer.WriteFloatRounded(playerAction.deltaTime, _minDt, _maxDT, _stepDT);
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