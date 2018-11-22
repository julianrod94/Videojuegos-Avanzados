namespace StateNS {
    public interface IInterpolatableState<T> {
        T UpdateState(float progression, T target);
        float TimeStamp();
    }
}