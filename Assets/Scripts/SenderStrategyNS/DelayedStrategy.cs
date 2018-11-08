using System.Threading;
using UnityEngine;

namespace SenderStrategyNS {
    public class DelayedStrategy : SenderStrategy {

        private int _delay;

        public DelayedStrategy(int delay) {
            _delay = delay;
        }

        public override void SendPackage(byte[] bytes) {
            WaitCallback wc = (state) => {
                Thread.Sleep(_delay);
                _sender(bytes);
            };
            ThreadPool.QueueUserWorkItem(wc);
        }

        public override void ReceivePackage(byte[] bytes) {
            _receiver(bytes);
        }

        public override void Dispose() {
        }
    }
}