using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace ChannelNS {
    public abstract class Channel<T> {
        protected ISenderStrategy Strategy { get; private set; }
        protected Bitbuffer buffer;

        public void ReceivePackage(byte[] bytes) {
            Strategy.ReceivePackage(bytes);
        }

        /// <summary>
        /// Must be called at initialization to initiate the strategy
        /// </summary>
        /// <param name="strategy">SenderStrategy to be used</param>
        protected void setupStrategy(ISenderStrategy strategy) {
            Strategy = strategy;
            strategy.SetupListener(ProcessData);
        }

        public abstract T DeserializeData(byte[] bytes);
        public abstract byte[] SerializeData(T data);

        /// <summary>
        /// Respond correctly to a new message
        /// </summary>
        /// <param name="bytes">Message that arrived by the sender strategy</param>
        protected abstract void ProcessData(byte[] bytes);

    }
}

