using System;

namespace ChannelNS {
    public abstract class GenericChannel {
        protected ISenderStrategy Strategy;

        public void SetupSender(Action<byte[]> sender) {
            Strategy.SetupSender(sender);
        }

        public abstract void ReceivePackage(byte[] bytes);
    }

    public abstract class Channel<T> : GenericChannel {
        protected Bitbuffer buffer = new Bitbuffer();

        public override void ReceivePackage(byte[] bytes) {
            Strategy.ReceivePackage(bytes);
        }

        /// <summary>
        ///     Must be called at initialization to initiate the strategy
        /// </summary>
        /// <param name="strategy">SenderStrategy to be used</param>
        protected void setupStrategy(ISenderStrategy strategy) {
            Strategy = strategy;
            strategy.SetupListener(ProcessData);
        }

        protected abstract T DeserializeData(byte[] bytes);
        protected abstract byte[] SerializeData(T data);

        /// <summary>
        ///     Respond correctly to a new message
        /// </summary>
        /// <param name="bytes">Message that arrived by the sender strategy</param>
        protected abstract void ProcessData(byte[] bytes);
    }
}