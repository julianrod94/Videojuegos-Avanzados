namespace ChannelNS {
    public abstract class EventChannel<T> : Channel<T> {
        private IEventReceiver<T> _eventReceiver;

        public EventChannel<T> SetupEventReceiver(IEventReceiver<T> eventReceiver) {
            _eventReceiver = eventReceiver;
            return this;
        }

        public void SendEvent(T newEvent) {
            var data = SerializeData(newEvent);
            Strategy.SendPackage(data);
        }

        protected override void ProcessData(byte[] bytes) {
            var newEvent = DeserializeData(bytes);
            _eventReceiver.ReceiveEvent(newEvent);
        }
    }
}