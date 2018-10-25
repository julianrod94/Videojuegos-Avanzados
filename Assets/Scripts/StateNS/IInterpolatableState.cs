namespace StateNS {
    public interface IInterpolatableState {
        void UpdateState(float progression, IInterpolatableState target);
        float TimeStamp();
    }
}
