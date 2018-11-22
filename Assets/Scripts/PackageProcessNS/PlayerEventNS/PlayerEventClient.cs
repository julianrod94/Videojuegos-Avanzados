using System.Diagnostics;
using ChannelNS;
using EventNS.PlayerEventNS;
using SenderStrategyNS;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;

public class PlayerEventClient: MonoBehaviour {
    public PlayerEventChannel PlayerEventChannel;

    public static PlayerEventClient Instance;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        PlayerEventChannel = new PlayerEventChannel(Process, new ReliableStrategy(0.1f,-1));
        ClientConnectionManager.Instance.ChannelManager.RegisterChannel((int)RegisteredChannels.PlayerEventChannel, PlayerEventChannel);
    }

    private void Process(PlayerEvent pEvent) {
        switch (pEvent.type) {
                case PlayerEvent.Type.Hit:
                    Debug.Log("HIT");
                    break;
                
                case PlayerEvent.Type.Die:
                    Debug.Log("DEAD");
                    break;
                
            case PlayerEvent.Type.Connected:
                Debug.Log("Connected");
                GameManager.Instance.Connected();
                break;
        }
    }

    public void Connect() {
        Debug.Log("Connecting");
        PlayerEventChannel.SendEvent(PlayerEvent.Connect());
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
                        PlayerEventChannel.SendEvent(PlayerEvent.Hit(id));
                        return;
                }
                
                PlayerEventChannel.SendEvent(PlayerEvent.Shoot());
            }
    }

    public void ThrowGranade(Vector3 direction) {
        PlayerEventChannel.SendEvent(PlayerEvent.ThrowGranade(direction));
    }

    private void OnDestroy() {
        PlayerEventChannel.Dispose();
    }
}
