﻿using System.Collections;
using System.Collections.Generic;
using ChannelNS;
using ChannelNS.Implementations.EventChannel;
using ChannelNS.Implementations.StateChannels;
using InputManagerNS;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

public class PlayerMovementReceiver: MonoBehaviour, IUnityBridgeState<CubePosition> {
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
        _channel = new InputSequenceStateChannel((a) => { },new TrivialStrategy());
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel(0, _cubeChannel);
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel(2, _channel);
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
            PlayerInput.ApplyInput(gameObject, playerAction);
        }
    }

    private void OnDestroy() {
        _cubeChannel.Dispose();
    }

}

