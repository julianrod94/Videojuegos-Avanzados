using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChannelNS;
using ChannelNS.Implementations.StateChannels;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

public class GrenadesStatesReceiver: MonoBehaviour {

    public GameObject grenadePrefab;
    private static GrenadesStatesReceiver Instance;

    public int activeGrenades = 0;
    
    public Dictionary<int, GameObject> grenades = new Dictionary<int, GameObject>();
    private GrenadesChannel _channel;
    private void Awake() {
        Instance = this;
    }

    public void AddGrenade(int id, Vector3 position) {
        GameObject go = Instantiate(grenadePrefab);
        activeGrenades++;
        go.GetComponent<OtherPlayer>().id = id;
        go.transform.position = position;
        grenades[id] = go;
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
            foreach (var pState in currentState._states) {
                if (!grenades.ContainsKey(pState.Key)) {
                    AddGrenade(pState.Key, pState.Value.Position);
                }
                var po = grenades[pState.Key];
                po.transform.position = pState.Value.Position;
            }

            if (activeGrenades < currentState._states.Count) {
                foreach (var keyValuePair in grenades) {
                    if (!currentState._states.ContainsKey(keyValuePair.Key)) {
                        Destroy(grenades[keyValuePair.Key]);
                        grenades.Remove(keyValuePair.Key);
                        activeGrenades--;
                    }
                }
            }
        }
    }

    private void OnDestroy() {
        _channel.Dispose();
    }
}
