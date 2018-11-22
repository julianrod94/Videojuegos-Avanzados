using System;
using ChannelNS;
using EventNS.PlayerEventNS;
using SenderStrategyNS;

namespace EventNS.keepAliveNS {
    public class KeepAliveChannel: EventChannel<bool> {
        
        public KeepAliveChannel(Action<bool> eventReceiver, SenderStrategy strategy) {
            SetupEventReceiver(eventReceiver);
            setupStrategy(strategy);
        }        
        
        protected override bool DeserializeData(byte[] bytes) {
            return true;
        }

        protected override byte[] SerializeData(bool data) {
            return new byte[] {1};
        }
    }
}