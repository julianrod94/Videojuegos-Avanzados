using System;
using System.Text;
using SenderStrategyNS;
using UnityEngine;

namespace ChannelNS.Implementations.EventChannel {
    public class TextEventChannel: EventChannel<TextMessage> {
        private readonly int _numberMin = 0;
        private readonly int _numberMax = 1000;

        public TextEventChannel(Action<TextMessage> eventReceiver, SenderStrategy strategy) {
            SetupEventReceiver(eventReceiver);
            setupStrategy(strategy);
        }
        
        protected override TextMessage DeserializeData(byte[] bytes) {
            lock (this) {
                int number;
                string message;
                try {
                    buffer.LoadBytes(bytes);
                    buffer.ToRead();
                    number = buffer.ReadInt(_numberMin, _numberMax);
                    var builder = new StringBuilder();
                    byte currentByte = buffer.Readbyte();
                    while (currentByte != 0) {
                        builder.Append((char)currentByte);
                        currentByte = buffer.Readbyte();
                    }

                    message = builder.ToString();
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw;
                }


                return new TextMessage(number, message);
            }
        }

        protected override byte[] SerializeData(TextMessage data) {
            lock (this) {
                try {
                    buffer.ToWrite();
                    buffer.WriteInt(data.numberMessage, _numberMin, _numberMax);
                    foreach (char c in data.message) {
                        buffer.WriteByte((byte) c);
                    }
                    buffer.WriteByte(0);
                } catch (Exception e) {
                    Debug.LogError(e);
                    throw;
                }

                return buffer.GenerateBytes();
            }
        }
    }
}