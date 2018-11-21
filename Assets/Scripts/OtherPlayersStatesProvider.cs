﻿using System.Collections.Generic;
using System.Linq;
using ChannelNS;
using ChannelNS.Implementations.StateChannels;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

public class OtherPlayersStatesProvider: MonoBehaviour {
    
    public static OtherPlayersStatesProvider Instance;
    
    public OtherPlayersStates LastState;
    
    public Dictionary<int, PlayerEventServer> players = new Dictionary<int, PlayerEventServer>();
    public Dictionary<int, OtherPlayersChannel> playersChannel = new Dictionary<int, OtherPlayersChannel>();

    
    public class OtherPlayersStatesBridge: IUnityBridgeState<OtherPlayersStates> {

        private int _playerTarget;

        public OtherPlayersStatesBridge(int target) {
            _playerTarget = target;
        }
        
        public OtherPlayersStates GetCurrentState() {
            var newDict = new Dictionary<int, OtherPlayerState>();
            var lastState = Instance.LastState;
            foreach (var otherPlayerState in lastState._states) {
                if (otherPlayerState.Key != _playerTarget) {
                    newDict[otherPlayerState.Key] = otherPlayerState.Value;
                }
            }

            return new OtherPlayersStates(lastState.TimeStamp(), newDict);
        }
    }


    private void Awake() {
        Instance = this;
    }

    public int AddPlayer(PlayerEventServer go, ChannelManager cm) {
        lock (this) {
            //TODO Whathappens if many discconect and reconnect
            var id = players.Count;
            var channel = new OtherPlayersChannel(
                new OtherPlayersStatesBridge(id),
                new TrivialStrategy(),
                0.1f);

            playersChannel[id] = channel;
            players[id] = go;
            Debug.LogError("NOW THERE ARE  " + players.Count );
            cm.RegisterChannel((int)RegisteredChannels.OtherPlayersChannel, channel);
            
            channel.StartSending();
            return id;
        }
    }


    public void DamagePlayer(int id) {
        players[id].Damage();
    }
    
    private void FixedUpdate() {
        var newDict = new Dictionary<int, OtherPlayerState>();
        foreach (var keyValuePair in players) {
            var po = keyValuePair.Value;
            
            newDict[keyValuePair.Key] = new OtherPlayerState(po.transform.position, po.transform.rotation);
        }
        
        LastState = new OtherPlayersStates(Time.time, newDict);
    }

    private void OnDestroy() {
        foreach (var keyValuePair in playersChannel) {
            keyValuePair.Value.Dispose();
        }
    }
}
