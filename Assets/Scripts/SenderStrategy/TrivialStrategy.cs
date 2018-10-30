using System;
using ChannelNS;
using UnityEngine;

namespace SenderStrategy {
    public class TrivialStrategy : ISenderStrategy {
        private Action<byte[]> _receiver;
        private Action<byte[]> _sender;

        public void SetupListener(Action<byte[]> receiver) {
            _receiver = receiver;
        }

        public void SetupSender(Action<byte[]> sender) {
            _sender = sender;
        }

        public void SendPackage(byte[] bytes) {
            _sender(bytes);
        }

        public void ReceivePackage(byte[] bytes) {
            _receiver(bytes);
        }
    }
}