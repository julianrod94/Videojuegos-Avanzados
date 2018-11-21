using System.Collections.Generic;
using ChannelNS.Implementations.StateChannels;
using GameScripts;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

public class GrenadeStatesProvider: MonoBehaviour, IUnityBridgeState<GrenadesState> {
    
    public GameObject grenadePrefab;
    public int grenadeCount = 0;
    public static GrenadeStatesProvider Instance;
    public GrenadesState LastState;
    private Queue<initialConditions> _toInstantiate = new Queue<initialConditions>();
        
    private struct initialConditions {
        public initialConditions(Vector3 position, Quaternion rotation) {
            this.position = position;
            this.rotation = rotation;
        }
        
        public Vector3 position;
        public Quaternion rotation;
    }

    
    public Dictionary<int, GrenadeBehaviour> grenades = new Dictionary<int, GrenadeBehaviour>();
    private GrenadesChannel _channel;
    private void Awake() {
        Instance = this;
    }
   
    public GrenadesState GetCurrentState() {
        return LastState;
    }

    public void SetupChannels(ChannelManager cm) {
        _channel = new GrenadesChannel(this, new TrivialStrategy(), 1/30f);
        cm.RegisterChannel((int)RegisteredChannels.GrenadeStatesChannel, _channel);
    }

    private void FixedUpdate() {
        while (_toInstantiate.Count > 0) {
            var conditions = _toInstantiate.Dequeue();
            var newGrenade = Instantiate(grenadePrefab, conditions.position, conditions.rotation);
            newGrenade.GetComponent<Rigidbody>().AddForce(Vector3.forward * 5, ForceMode.Impulse);
            grenades[grenadeCount++] = newGrenade.GetComponent<GrenadeBehaviour>();
        }
        
        var newDict = new Dictionary<int, GrenadeState>();
        foreach (var keyValuePair in grenades) {
            var po = keyValuePair.Value;
            
            newDict[keyValuePair.Key] = new GrenadeState(po.gameObject.transform.position, po.isExploding);
        }
        
        LastState = new GrenadesState(Time.time, newDict);
    }

    public void GenerateGrenade(Vector3 position, Vector3 rotation) {
        _toInstantiate.Enqueue(new initialConditions(position, Quaternion.Euler(rotation))); 
    }
    
}
