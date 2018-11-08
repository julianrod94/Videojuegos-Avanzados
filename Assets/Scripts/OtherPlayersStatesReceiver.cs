using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChannelNS;
using ChannelNS.Implementations.StateChannels;
using SenderStrategyNS;
using StateNS;
using UnityEngine;

public class OtherPlayersStatesReceiver: MonoBehaviour {

    public GameObject otherPlayerPrefab;
    private static OtherPlayersStatesReceiver Instance;
    
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    private OtherPlayersChannel _channel;
    private void Awake() {
        Instance = this;
    }

    public void AddPlayer(int id) {
        GameObject go = Instantiate(otherPlayerPrefab, Vector3.zero, Quaternion.identity);
        players[id] = go;
    }
    
    private void Start() {
        _channel = new OtherPlayersChannel(null, new TrivialStrategy(), 0.1f);
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel(0, _channel);
        _channel.Interpolator.StartInterpolating();
    }


    private void FixedUpdate() {
        _channel.Interpolator.Update(Time.deltaTime);
        var currentState = _channel.Interpolator.PresentState;
        
        if (currentState != null) {
            foreach (var pState in currentState._states) {
                if (!players.ContainsKey(pState.Key)) {
                    AddPlayer(pState.Key);
                }
                var po = players[pState.Key];
                po.transform.position = pState.Value.Position;
                po.transform.rotation = pState.Value.Rotation;
            }
        }
    }

    private void OnDestroy() {
        _channel.Dispose();
    }
}
