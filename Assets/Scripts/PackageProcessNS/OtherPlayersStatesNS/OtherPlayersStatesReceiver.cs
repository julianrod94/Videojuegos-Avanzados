using System.Collections.Generic;
using System.Runtime.InteropServices;
using ChannelNS;
using SenderStrategyNS;
using SnapshotsNS.OtherPlayers;
using StateNS;
using UnityEngine;

public class OtherPlayersStatesReceiver: MonoBehaviour {

    public GameObject otherPlayerPrefab;
    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();
    private OtherPlayersChannel _channel;

    public void AddPlayer(int id, Vector3 position) {
        GameObject go = Instantiate(otherPlayerPrefab);
        go.GetComponent<OtherPlayer>().id = id;
        go.transform.position = position;
        players[id] = go;
    }
    
    private void Start() {
        _channel = new OtherPlayersChannel(null, new TrivialStrategy(), 0.1f);
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel((int)RegisteredChannels.OtherPlayersChannel, _channel);
        _channel.Interpolator.StartInterpolating();
    }

    private void FixedUpdate() {
        _channel.Interpolator.Update(Time.deltaTime);
        var currentState = _channel.Interpolator.PresentState;
        
        if (currentState != null) {
            foreach (var pState in currentState._states) {
                if (!players.ContainsKey(pState.Key)) {
                    AddPlayer(pState.Key, pState.Value.Position);
                }
                
                var po = players[pState.Key];
                po.transform.position = pState.Value.Position;
                po.transform.rotation = pState.Value.Rotation;
            }
            
            foreach (var keyValuePair in players) {
                if (!currentState._states.ContainsKey(keyValuePair.Key)) {
                    Destroy(players[keyValuePair.Key]);
                    players.Remove(keyValuePair.Key);
                }
            }
        }
    }

    private void OnDestroy() {
        _channel.Dispose();
    }
}
