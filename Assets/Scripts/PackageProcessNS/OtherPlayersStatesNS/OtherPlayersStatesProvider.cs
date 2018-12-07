using System.Collections.Generic;
using System.Linq;
using ChannelNS;
using SenderStrategyNS;
using ServerNS;
using SnapshotsNS;
using SnapshotsNS.OtherPlayers;
using StateNS;
using UnityEngine;

public class OtherPlayersStatesProvider: MonoBehaviour {
    
    public static OtherPlayersStatesProvider Instance;
    
    public OtherPlayersStates LastState;
    
    public Dictionary<int, Health> Healths = new Dictionary<int, Health>();
    public Dictionary<int, OtherPlayersChannel> playersChannel = new Dictionary<int, OtherPlayersChannel>();

    public class OtherPlayersStatesBridge: IUnityBridgeState<OtherPlayersStates> {

        private readonly int _playerTarget;

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

    public void AddPlayer(int id, Health health, ChannelManager cm) {
        lock (this) {
            //TODO Whathappens if many discconect and reconnect
            var channel = new OtherPlayersChannel(
                new OtherPlayersStatesBridge(id),
                new TrivialStrategy(),
                0.1f);

            playersChannel[id] = channel;
            Healths[id] = health;
            Debug.Log("Players connected  " + Healths.Count);
            
            cm.RegisterChannel((int)RegisteredChannels.OtherPlayersChannel, channel);
            channel.StartSending();
        }
    }

    public void DamagePlayer(int id) {
        Health playerHealth = Healths[id];
        playerHealth.Damage();
    }
    
    private void FixedUpdate() {
        var newDict = new Dictionary<int, OtherPlayerState>();
        foreach (var keyValuePair in Healths) {
            var po = keyValuePair.Value;
            newDict[keyValuePair.Key] = new OtherPlayerState(po.transform.position, po.transform.rotation);
        }
        
        LastState = new OtherPlayersStates(Time.time, newDict);
    }

    public void DisconnectPlayer(int id) {
        playersChannel[id].Dispose();
        playersChannel.Remove(id);
        Healths.Remove(id);
    }

    private void OnDestroy() {
        foreach (var keyValuePair in playersChannel) {
            keyValuePair.Value.Dispose();
        }
    }
}
