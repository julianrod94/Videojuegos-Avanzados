using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ChannelNS;
using UnityEngine;
using Utils;

namespace SenderStrategyNS {
    public class ReliableStrategy: SenderStrategy {
        private Dictionary<int, float> alreadyAcked = new Dictionary<int, float>();
        
        private struct MessageState {
            public int timesSent;
            public Timer timer;
            public byte[] message;

            public MessageState(Timer timer, byte[] message) {
                this.timer = timer;
                timesSent = 0;
                this.message = message;
            }

            public void addTry() {
                timesSent += 1;
            }
        }
        
        private long _timeout;
        private int _tries;

        /// <summary>
        /// This strategy can only wait for up to 256 messages at the same time
        /// </summary>
        private byte _messageIdCount = 0;
        private Dictionary<byte, MessageState> waiting = new Dictionary<byte, MessageState>();

        private const byte NORMAL_MESSAGE_BYTE = 222;
        private const byte ACK_MESSAGE_BYTE = 111;
        
        
        public ReliableStrategy(float timeout, int tries) {
            _timeout = (long)(timeout*1000);
            _tries = tries;
        }

        public override void SendPackage(byte[] bytes) {
            lock (this) {
                if (waiting.Keys.Count == 256) {
                    throw new ExecutionEngineException("This Strategy can´t wait for more messages");
                }

                while (waiting.ContainsKey(_messageIdCount)) {
                    _messageIdCount = (byte) ((_messageIdCount + 1) % 256);
                }
                
                var messages = new[] {_messageIdCount, NORMAL_MESSAGE_BYTE};
                var newMessage = ArrayUtils.AddBytesToArray(bytes, messages);

                TimerCallback tc = (state) => ReSendPackage((byte) state);
                waiting.Add(_messageIdCount, new MessageState(new Timer(tc, _messageIdCount, 0, _timeout), newMessage));
                _messageIdCount++;
            }
        }

        private void ReSendPackage(byte messageId) {
            if (!waiting.ContainsKey(messageId)) {
                throw new ExecutionEngineException("There was a problem where a timer was leaked");
            }

            var state = waiting[messageId];
            state.addTry();
            if (_tries < 0 || state.timesSent <= _tries) {
                _sender(state.message);
            } else {
                lock (this) {
                    waiting.Remove(messageId);
                    state.timer.Dispose();
                }
            }
        }

        public override void ReceivePackage(byte[] bytes) {
            var now = CurrentTime.Time;
            
            var command = bytes[bytes.Length - 1];
            var messageId = bytes[bytes.Length - 2];
            switch (command) {
                case NORMAL_MESSAGE_BYTE:
                    if(!alreadyAcked.ContainsKey(messageId) || 
                        alreadyAcked[messageId] - now >= 1) {
                        _receiver(ArrayUtils.RemoveBytes(bytes, 2));
                        alreadyAcked[messageId] = now;
                    }
                    SendAck(messageId);
                    break;
                    
                    case ACK_MESSAGE_BYTE:
                        lock (this) {
                            if (waiting.ContainsKey(messageId)) {
                                var state = waiting[messageId];
                                state.timer.Dispose();
                                waiting.Remove(messageId);
                            }
                        }
                        break;
                    
                    default:
                        Debug.LogError("WTF " + command);
                        break;
            }
        }

        private void SendAck(byte messageId) {
             var message = new[] {messageId, ACK_MESSAGE_BYTE};
            _sender(message);
        }
        
        public override void Dispose() {
            foreach (var keyValuePair in waiting) {
                keyValuePair.Value.timer.Dispose();
            }
        }
    }
}