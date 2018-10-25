namespace ChannelNS {
    public interface IStateProvider<T> {
        T PollState();
        void UpdateState(T newState);
    }
}