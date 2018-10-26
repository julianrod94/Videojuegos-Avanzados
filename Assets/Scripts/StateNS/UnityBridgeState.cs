namespace StateNS {
    public interface IUnityBridgeState<out T> {
        T GetCurrentState();
    }
}