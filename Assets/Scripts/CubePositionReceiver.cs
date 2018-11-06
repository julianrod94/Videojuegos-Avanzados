using ChannelNS;
using ChannelNS.Implementations.StateChannels;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

public class CubePositionReceiver : MonoBehaviour, IUnityBridgeState<CubePosition> {
    private StateChannel<CubePosition> _cubeChannel;

    private CubePosition currentState;

    public CubePosition GetCurrentState() {
        return currentState;
    }

    // Use this for initialization
    private void Start() {
        _cubeChannel = new CubePositionStateChannel(this, new ReliableStrategy(0.1f,10), 0.1f);
        SetupEverything.instance.receiver.RegisterChannel(0, _cubeChannel);
    }

    // Update is called once per frame
    private void Update() {
        _cubeChannel.Interpolator.Update(Time.deltaTime);
        currentState = _cubeChannel.Interpolator.PresentState;
        if (currentState != null) {
            transform.position = currentState.Position;
        }
    }

    private void OnDestroy() {
        _cubeChannel.Dispose();
    }
}