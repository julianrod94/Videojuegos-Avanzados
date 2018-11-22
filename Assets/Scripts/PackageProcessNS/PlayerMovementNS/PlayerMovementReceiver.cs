﻿using System;
 using System.Collections;
using System.Collections.Generic;
using ChannelNS;
 using EventNS.InputSequenceNS;
 using GameScripts.Player;
 using SenderStrategyNS;
 using SnapshotsNS;
 using SnapshotsNS.PlayerMovementNS;
 using StateNS;
using UnityEngine;
 using Random = UnityEngine.Random;

public class PlayerMovementReceiver: MonoBehaviour, IUnityBridgeState<PlayerPosition> {
    private InputManager _inputManager = new InputManager();
    private StateChannel<PlayerPosition> _cubeChannel;
    private InputSequenceEventChannel _channel;
    
    private PlayerPosition currentState;
    private PlayerPosition _lastUpdatedState;

    public PlayerPosition GetCurrentState() {
        return currentState;
    }

    // Use this for initialization
    private void Start() {
        _lastUpdatedState = new PlayerPosition(Time.time, transform.position, -1, GetComponent<Health>().GetCurrentHealth());
        _cubeChannel = new PlayerPositionStateChannel(this, new TrivialStrategy(), 0.1f);
        _channel = new InputSequenceEventChannel((a) => { },new TrivialStrategy());
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel((int)RegisteredChannels.PlayerPositionChannel, _cubeChannel);
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel((int)RegisteredChannels.PlayerInputChannel, _channel);
        GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
    }

    // Update is called once per frame
    private void Update() {
        if (Input.GetKey(KeyCode.A)) {
            _inputManager.SubmitInput(InputEnum.Left, transform.rotation);
        }
		
		
        if (Input.GetKey(KeyCode.W)) {
            _inputManager.SubmitInput(InputEnum.Forward, transform.rotation);
        }
		
		
        if (Input.GetKey(KeyCode.D)) {
            _inputManager.SubmitInput(InputEnum.Right, transform.rotation);
        }
		
		
        if (Input.GetKey(KeyCode.S)) {
            _inputManager.SubmitInput(InputEnum.Back, transform.rotation);
        }

        if (_inputManager.Count() > 0) {
            _channel.SendEvent(_inputManager.Inputs());
        }

       _cubeChannel.Interpolator.Update(Time.deltaTime);
        currentState = _cubeChannel.Interpolator.PastState;
        
        if (currentState != null)
        {
            Health health = GetComponent<Health>();
            transform.position = currentState.Position;
            if (health.GetCurrentHealth() != currentState.Health)
            {
                health.Damage(currentState.Health);
            }
            _inputManager.EmptyUpTo(currentState.LastInputApplied);
            _lastUpdatedState = currentState;
        } else {
            transform.position = _lastUpdatedState.Position;
        }
        Predict();
    }

    private void Predict() {
        foreach (var playerAction in _inputManager.Inputs()) {
            PlayerInput.ApplyInput(gameObject, playerAction);
        }
    }

    private void OnDestroy() {
        _cubeChannel.Dispose();
    }

}

