using System.Collections.Generic;
using ChannelNS;
using ChannelNS.Implementations.StateChannels;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

public class OtherPlayersStatesProviding: MonoBehaviour {
    
    private static OtherPlayersStatesProviding Instance;
    
    public OtherPlayersStates LastState;
    
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    public Dictionary<int, OtherPlayersChannel> playersChannel = new Dictionary<int, OtherPlayersChannel>();
    
    private class OtherPlayersStatesBridge : IUnityBridgeState<OtherPlayersStates> {

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

    public void AddPlayer(GameObject go, ChannelManager cm) {
        var id = players.Count;
        var channel = new OtherPlayersChannel(
            new OtherPlayersStatesBridge(id),
            new TrivialStrategy(),
            0.1f);
        playersChannel[id] = new;
        players[players.Count] = go;
        
        _cubeChannel = new CubePositionStateChannel(this, new TrivialStrategy(), 0.1f);

    }


    private void FixedUpdate() {
        
    }

    private void OnDestroy() {
        foreach (var keyValuePair in playersChannel) {
            keyValuePair.Value
        }
    }
}
