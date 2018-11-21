using ChannelNS;
using ChannelNS.Implementations.EventChannel;
using ChannelNS.Implementations.StateChannels;
using InputManagerNS;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

public class PlayerMovementProvider: MonoBehaviour, IUnityBridgeState<CubePosition> {
	// Update is called once per frame

	private readonly InputManager _inputManager = new InputManager();

    private StateChannel<CubePosition> _cubeChannel;
    private InputSequenceStateChannel _inputChannel;
    private int _lastAppliedInput = 0;
    
    public CubePosition LastState;

    public CubePosition GetCurrentState() {
        return LastState;
    }

    // Use this for initialization
    private void Start() {
	    // Obtenemos el RigidBody para hacer el salto posteriormente;
        GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
    }

    public void SetupChannels(ChannelManager cm) {
        _cubeChannel = new CubePositionStateChannel(this, new TrivialStrategy(), 0.1f);
       cm.RegisterChannel((int)RegisteredChannels.PlayerPositionChannel, _cubeChannel);
        _cubeChannel.StartSending();
        
        
        _inputChannel = new InputSequenceStateChannel((a) => {
            foreach (var playerAction in a) {
                _inputManager.ReceivePlayerAction(playerAction);
            }
        }, new TrivialStrategy());
        
        cm.RegisterChannel((int)RegisteredChannels.PlayerInputChannel, _inputChannel);
    }

    // Update is called once per frame
    private void FixedUpdate() {
        lock (_inputManager) {
            foreach (var playerAction in _inputManager.Inputs()) {
                _lastAppliedInput = PlayerInput.ApplyInput(gameObject, playerAction);
            }
        }
        
        _inputManager.EmptyAll(_lastAppliedInput);
        LastState = new CubePosition(Time.time, transform.position, _lastAppliedInput, GetComponent <Health> ().GetCurrentHealth());
    }

    private void OnDestroy() {
        _cubeChannel.Dispose();
    }
}

