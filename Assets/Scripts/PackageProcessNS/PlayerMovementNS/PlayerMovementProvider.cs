using ChannelNS;
using EventNS.InputSequenceNS;
using GameScripts.Player;
using SenderStrategyNS;
using ServerNS;
using SnapshotsNS;
using SnapshotsNS.PlayerMovementNS;
using StateNS;
using UnityEngine;

public class PlayerMovementProvider: MonoBehaviour, IUnityBridgeState<PlayerPosition> {
	// Update is called once per frame

	private readonly InputManager _inputManager = new InputManager();

    private StateChannel<PlayerPosition> _positionChannel;
    private InputSequenceEventChannel _inputChannel;
    private int _lastAppliedInput = 0;
    
    public PlayerPosition LastState;

    public PlayerPosition GetCurrentState() {
        return LastState;
    }

    // Use this for initialization
    private void Start() {
	    // Obtenemos el RigidBody para hacer el salto posteriormente;
        GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
    }

    public void SetupChannels(ChannelManager cm) {
        _positionChannel = new PlayerPositionStateChannel(this, new TrivialStrategy(), 0.1f);
       cm.RegisterChannel((int)RegisteredChannels.PlayerPositionChannel, _positionChannel);
        _positionChannel.StartSending();
        
        
        _inputChannel = new InputSequenceEventChannel((a) => {
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
                if(!ServerGameManager.Instance.IsPlayerDead(GetComponent<OtherPlayer>().id))_lastAppliedInput = PlayerInput.ApplyInput(gameObject, playerAction);
            }
        }
        
        _inputManager.EmptyAll(_lastAppliedInput);
        LastState = new PlayerPosition(Time.time, transform.position, _lastAppliedInput, GetComponent <Health> ().GetCurrentHealth());
    }

    private void OnDestroy() {
        _positionChannel.Dispose();
        _inputChannel.Dispose();
    }
}

