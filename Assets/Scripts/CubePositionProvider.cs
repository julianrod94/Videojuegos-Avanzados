using ChannelNS;
using ChannelNS.Implementations.EventChannel;
using ChannelNS.Implementations.StateChannels;
using InputManagerNS;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

public class CubePositionProvider : MonoBehaviour, IUnityBridgeState<CubePosition> {
    private readonly InputManager _inputManager = new InputManager();

    private StateChannel<CubePosition> _cubeChannel;
    private InputSequenceStateChannel _inputChannel;
    private int _lastAppliedInput = 0;
    
    public CubePosition LastState;

    public CubePosition GetCurrentState() {
        return LastState;
    }

    public void SetupChannels(ChannelManager cm) {
        _cubeChannel = new CubePositionStateChannel(this, new TrivialStrategy(), 0.1f);
       cm.RegisterChannel(0, _cubeChannel);
        _cubeChannel.StartSending();
        
        
        _inputChannel = new InputSequenceStateChannel((a) => {
            foreach (var playerAction in a) {
                _inputManager.ReceivePlayerAction(playerAction);
            }
        }, new TrivialStrategy());
        
        cm.RegisterChannel(2, _inputChannel);
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetKey(KeyCode.K)) {
            transform.Translate(Vector3.forward *  Time.deltaTime);
        }

        lock (_inputManager) {
            foreach (var playerAction in _inputManager.Inputs()) {
                _lastAppliedInput = playerAction.inputNumber;
                switch (playerAction.inputCommand) {
                    case InputEnum.Forward:
                        transform.Translate(Vector3.forward *  playerAction.deltaTime);
                        break;
                    case InputEnum.Back:
                        transform.Translate(Vector3.back * playerAction.deltaTime);
                        break;
                    case InputEnum.Left:
                        transform.Translate(Vector3.left * playerAction.deltaTime);
                        break;
                    case InputEnum.Right:
                        transform.Translate(Vector3.right * playerAction.deltaTime);
                        break;
                        
                }
            }
        }
        
        _inputManager.EmptyAll(_lastAppliedInput);
        LastState = new CubePosition(Time.time, transform.position, _lastAppliedInput);
    }

    private void OnDestroy() {
        _cubeChannel.Dispose();
    }
}