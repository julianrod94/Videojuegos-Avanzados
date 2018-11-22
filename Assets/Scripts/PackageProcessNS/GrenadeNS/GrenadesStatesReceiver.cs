using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChannelNS;
using GameScripts;
using SenderStrategyNS;
using SnapshotsNS.GrenadeStateNS;
using StateNS;
using UnityEngine;

public class GrenadesStatesReceiver: MonoBehaviour {

    public GameObject grenadePrefab;
    public int activeGrenades = 0;
    public Dictionary<int, GrenadeBehaviour> grenades = new Dictionary<int, GrenadeBehaviour>();
    private GrenadesChannel _channel;

    public void AddGrenade(int id, Vector3 position) {
        Debug.Log("GRANADE");
        GameObject go = Instantiate(grenadePrefab);
        activeGrenades++;
        go.transform.position = position;
        grenades[id] = go.GetComponent<GrenadeBehaviour>();
    }
    
    private void Start() {
        _channel = new GrenadesChannel(null, new TrivialStrategy(), 1/30f);
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel((int)RegisteredChannels.GrenadeStatesChannel, _channel);
        _channel.Interpolator.StartInterpolating();
    }

    private void FixedUpdate() {
        _channel.Interpolator.Update(Time.deltaTime);
        var currentState = _channel.Interpolator.PresentState;
        
        if (currentState != null) {
            if (activeGrenades > currentState._states.Count) {
                var destroyed = new List<int>();
                foreach (var keyValuePair in grenades) {
                    if (!currentState._states.ContainsKey(keyValuePair.Key)) {
                        destroyed.Add(keyValuePair.Key);
                    }
                }
                
                foreach (var i in destroyed) {
                    Destroy(grenades[i].gameObject);
                    grenades.Remove(i);
                    activeGrenades--;
                }
            }
            
            foreach (var pState in currentState._states) {
                if (!grenades.ContainsKey(pState.Key)) {
                    AddGrenade(pState.Key, pState.Value.Position);
                }
                var po = grenades[pState.Key];
                po.transform.position = pState.Value.Position;
                po.transform.rotation = pState.Value.Rotation;
                po.isExploding = pState.Value.IsExploding;
            }

            
        }
    }

    private void OnDestroy() {
        _channel.Dispose();
    }
}
