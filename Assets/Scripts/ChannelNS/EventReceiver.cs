namespace ChannelNS {
    public interface IEventReceiver<T> {
        void ReceiveEvent(T newEvent);
    }
}