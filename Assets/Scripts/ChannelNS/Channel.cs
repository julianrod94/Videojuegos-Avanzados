using System;
using SenderStrategyNS;

namespace ChannelNS {
    public abstract class GenericChannel {
        protected Bitbuffer buffer = new Bitbuffer();
        protected SenderStrategy Strategy;

        public void SetupSender(Action<byte[]> sender) {
            Strategy.SetupSender(sender);
        }

        public void ReceivePackage(byte[] bytes) {
            Strategy.ReceivePackage(bytes);
        }

        public virtual void Dispose() {
            Strategy.Dispose();
        }
        
        /// <summary>
        ///     Must be called at initialization to initiate the strategy
        /// </summary>
        /// <param name="strategy">SenderStrategy to be used</param>
        protected void setupStrategy(SenderStrategy strategy) {
            Strategy = strategy;
            strategy.SetupReceiver(ProcessData);
        }
        
        /// <summary>
        ///     Respond correctly to a new message
        /// </summary>
        /// <param name="bytes">Message that arrived by the sender strategy</param>
        protected abstract void ProcessData(byte[] bytes);
    }

    public abstract class Channel<T> : GenericChannel {
        protected abstract T DeserializeData(byte[] bytes);
        protected abstract byte[] SerializeData(T data);
    }
}