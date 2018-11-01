using ChannelNS;
using ChannelNS.Implementations.StateChannels;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

public class CubePositionProvider : MonoBehaviour, IUnityBridgeState<CubePosition> {
    private StateChannel<CubePosition> _cubeChannel;

    public CubePosition lastState;

    public CubePosition GetCurrentState() {
        return lastState;
    }

    // Use this for initialization
    private void Start() {
        _cubeChannel = new CubePositionStateChannel(this, new ReliableStrategy(0.2f,10), 0.1f);
        SetupEverything.instance.sender.RegisterChannel(_cubeChannel);
        _cubeChannel.StartSending();
    }

    // Update is called once per frame
    private void Update() {
        if (Random.value < 0.1) {
            var ra = Random.value * 6;
            transform.position = new Vector3(Mathf.Sin(ra) * 5, Mathf.Cos(ra) * 5, 0);
        }

        lastState = new CubePosition(Time.time, transform.position);
    }

    private void OnDestroy() {
        _cubeChannel.Dispose();
    }
}