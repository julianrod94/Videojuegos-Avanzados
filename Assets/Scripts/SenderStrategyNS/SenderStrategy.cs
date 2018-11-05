using System;

namespace SenderStrategyNS {
    public abstract class SenderStrategy {
        
        protected Action<byte[]> _receiver;
        protected Action<byte[]> _sender;

        public void SetupReceiver(Action<byte[]> receiver) {
            _receiver = receiver;
        }

        public void SetupSender(Action<byte[]> sender) {
            _sender = sender;
        }
        
        public abstract void SendPackage(byte[] bytes);
        public abstract void ReceivePackage(byte[] bytes);
        public abstract void Dispose();
    }
}