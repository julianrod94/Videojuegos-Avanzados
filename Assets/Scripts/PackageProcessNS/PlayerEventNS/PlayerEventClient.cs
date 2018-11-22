using System.Diagnostics;
using ChannelNS;
using EventNS.PlayerEventNS;
using SenderStrategyNS;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Debug = UnityEngine.Debug;

public class PlayerEventClient: MonoBehaviour {
    private PlayerEventChannel _playerEventChannel;
    
    private void Start() {
        _playerEventChannel = new PlayerEventChannel((e) => Process(e), new TrivialStrategy());
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel((int)RegisteredChannels.PlayerEventChannel, _playerEventChannel);
    }

    private void Process(PlayerEvent pEvent) {
        switch (pEvent.type) {
                case PlayerEvent.Type.Hit:
                    Debug.Log("HIT");
                    break;
                
                case PlayerEvent.Type.Die:
                    Debug.Log("DEAD");
                    break;
        }
    }

    public void Shoot(float weaponRange, Transform origin) {
            Vector3 fwd = origin.transform.TransformDirection(Vector3.forward);

            Debug.DrawRay(origin.transform.position, fwd * weaponRange, Color.green,2);

            RaycastHit hit;
            if (Physics.Raycast(origin.transform.position, fwd, out hit, weaponRange)) {
                GameObject obj = hit.transform.gameObject;
                Debug.Log(obj.name);
                switch (obj.tag) {
                    case "Enemy":
                        var id = obj.GetComponent<OtherPlayer>().id;
                        _playerEventChannel.SendEvent(PlayerEvent.Hit(id));
                        return;
                }
                
                _playerEventChannel.SendEvent(PlayerEvent.Shoot());
            }
    }

    public void ThrowGranade(Vector3 direction) {
        _playerEventChannel.SendEvent(PlayerEvent.ThrowGranade(direction));
    }
}
