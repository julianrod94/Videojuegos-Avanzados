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
    private StateChannel<PlayerPosition> _positionChannel;
    private InputSequenceEventChannel _inputChannel;
    
    private PlayerPosition currentState;
    private PlayerPosition _lastUpdatedState;
    private Health health;

    public PlayerPosition GetCurrentState() {
        return currentState;
    }

    // Use this for initialization
    private void Start() {
        _lastUpdatedState = new PlayerPosition(Time.time, transform.position, -1, GetComponent<Health>().GetCurrentHealth());
        _positionChannel = new PlayerPositionStateChannel(this, new TrivialStrategy(), 0.1f);
        _inputChannel = new InputSequenceEventChannel((a) => { },new TrivialStrategy());
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel((int)RegisteredChannels.PlayerPositionChannel, _positionChannel);
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel((int)RegisteredChannels.PlayerInputChannel, _inputChannel);
        GetComponent <Rigidbody> ().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ; 
        health = GetComponent<Health>();
    }

    // Update is called once per frame
    private void Update() {
        if (health.GetCurrentHealth() > 0) {
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
                _inputChannel.SendEvent(_inputManager.Inputs());
            }
        }

        _positionChannel.Interpolator.Update(Time.deltaTime);
        currentState = _positionChannel.Interpolator.PastState;
        
        if (currentState != null) {
            transform.position = currentState.Position;
            if (health.GetCurrentHealth() != currentState.Health) {
                health.setHealth(currentState.Health);
            }
            _inputManager.EmptyUpTo(currentState.LastInputApplied);
            _lastUpdatedState = currentState;
            if (_inputManager.Count() > 200) {
                _inputManager.EmptyAll(currentState.LastInputApplied);
            }
        } else {
            transform.position = _lastUpdatedState.Position;
        }

        if (ClientConnectionManager.Instance.isPredicting) {
            Predict();
        }
    }

    private void Predict() {
        foreach (var playerAction in _inputManager.Inputs()) {
            PlayerInput.ApplyInput(gameObject, playerAction);
        }
    }

    private void OnDestroy() {
        _positionChannel.Dispose();
        _inputChannel.Dispose();
    }

}

