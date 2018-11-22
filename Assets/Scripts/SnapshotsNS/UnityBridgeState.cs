namespace SnapshotsNS {
    public interface IUnityBridgeState<out T> {
        T GetCurrentState();
    }
}