using System.Collections.Generic;
using ChannelNS;
using GameScripts;
using SenderStrategyNS;
using ServerNS;
using SnapshotsNS;
using SnapshotsNS.GrenadeStateNS;
using StateNS;
using UnityEngine;

public class GrenadeStatesProvider: MonoBehaviour, IUnityBridgeState<GrenadesState> {
    
    public static GrenadeStatesProvider Instance;
    
    public GameObject grenadePrefab;
    public int grenadeCount = 0;
    
    public GrenadesState LastState;
    private Queue<InitialConditions> _toInstantiate = new Queue<InitialConditions>();
    private Dictionary<int, GrenadesChannel> _channels = new Dictionary<int, GrenadesChannel>();    
    public Dictionary<int, ServerGrenadeBehaviour> grenades = new Dictionary<int, ServerGrenadeBehaviour>();

    private struct InitialConditions {
        public InitialConditions(int playerId, Quaternion rotation) {
            this.PlayerId = playerId;
            this.rotation = rotation;
        }
        
        public int PlayerId;
        public Quaternion rotation;
    }
    
    private void Awake() {
        Instance = this;
    }
   
    public GrenadesState GetCurrentState() {
        return LastState;
    }

    public void SetupChannel(int playerID, ChannelManager cm) {
        var channel = new GrenadesChannel(this, new TrivialStrategy(), 1/30f);
        _channels[playerID] = channel;
        cm.RegisterChannel((int)RegisteredChannels.GrenadeStatesChannel, channel);
        channel.StartSending();
    }

    private void FixedUpdate() {
        while (_toInstantiate.Count > 0) {
            var conditions = _toInstantiate.Dequeue();
            var newGrenade = Instantiate(grenadePrefab, 
                OtherPlayersStatesProvider.Instance.players[conditions.PlayerId].gameObject.transform.position,
                conditions.rotation);
            newGrenade.GetComponent<Rigidbody>().AddForce(Vector3.forward * 5, ForceMode.Impulse);
            grenades[grenadeCount++] = newGrenade.GetComponent<ServerGrenadeBehaviour>();
        }
        
        var newDict = new Dictionary<int, GrenadeState>();
        var destroyed = new List<int>();
        foreach (var keyValuePair in grenades) {
            var po = keyValuePair.Value;

            if (po == null) {
                destroyed.Add(keyValuePair.Key);
            } else {

                newDict[keyValuePair.Key] = new GrenadeState(
                    po.gameObject.transform.position,
                    po.gameObject.transform.rotation,
                    po.isExploding
                );
            }
        }
        
        destroyed.ForEach((i) => grenades.Remove(i));
        
        LastState = new GrenadesState(Time.time, newDict);
    }

    public void GenerateGrenade(int playerId, Vector3 rotation) {
        _toInstantiate.Enqueue(new InitialConditions(playerId, Quaternion.Euler(rotation))); 
    }

    private void OnDestroy() {
        foreach (var keyValuePair in _channels) {
            keyValuePair.Value.Dispose();
        }
    }
}
