using ChannelNS;
using ChannelNS.Implementations.EventChannel;
using ChannelNS.Implementations.StateChannels;
using InputManagerNS;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

public class CubePositionReceiver : MonoBehaviour, IUnityBridgeState<CubePosition> {
    private InputManager _inputManager = new InputManager();
    private StateChannel<CubePosition> _cubeChannel;
    private InputSequenceStateChannel _channel;
    
    private CubePosition currentState;
    private CubePosition _lastUpdatedState;

    public CubePosition GetCurrentState() {
        return currentState;
    }

    // Use this for initialization
    private void Start() {
        _lastUpdatedState = new CubePosition(Time.time, transform.position, -1);
        _cubeChannel = new CubePositionStateChannel(this, new TrivialStrategy(), 0.1f);
        _channel = new InputSequenceStateChannel((a) => { }, new DelayedStrategy(70));
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel(0, _cubeChannel);
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel(2, _channel);
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetKey(KeyCode.A)) {
            _inputManager.SubmitInput(InputEnum.Left);
        }
		
		
        if (Input.GetKey(KeyCode.W)) {
            _inputManager.SubmitInput(InputEnum.Forward);
        }
		
		
        if (Input.GetKey(KeyCode.D)) {
            _inputManager.SubmitInput(InputEnum.Right);
        }
		
		
        if (Input.GetKey(KeyCode.S)) {
            _inputManager.SubmitInput(InputEnum.Back);
        }

        if (_inputManager.Count() > 0) {
            _channel.SendEvent(_inputManager.Inputs());
        }

       _cubeChannel.Interpolator.Update(Time.deltaTime);
        currentState = _cubeChannel.Interpolator.PastState;
        Debug.LogWarning(_lastUpdatedState);
        
        if (currentState != null) {
            transform.position = currentState.Position;
            _inputManager.EmptyUpTo(currentState.LastInputApplied);
            _lastUpdatedState = currentState;
        } else {
            transform.position = _lastUpdatedState.Position;
        }
        Predict();
    }

    private void Predict() {
        foreach (var playerAction in _inputManager.Inputs()) {
            switch (playerAction.inputCommand) {
                case InputEnum.Forward:
                    transform.Translate(Vector3.forward * playerAction.deltaTime);
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

    private void OnDestroy() {
        _cubeChannel.Dispose();
    }
}