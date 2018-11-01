using System;
using ChannelNS;
using UnityEngine;

namespace SenderStrategyNS {
    public class TrivialStrategy : SenderStrategy {

        public override void SendPackage(byte[] bytes) {
            _sender(bytes);
        }

        public override void ReceivePackage(byte[] bytes) {
            _receiver(bytes);
        }

        public override void Dispose() {
        }
    }
}