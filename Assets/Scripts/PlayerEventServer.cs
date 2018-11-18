using System.Diagnostics;
using SenderStrategyNS;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using Debug = UnityEngine.Debug;

public class PlayerEventServer: MonoBehaviour {
    private PlayerEventChannel _playerEventChannel;
    private OtherPlayer _otherPlayer;
    
    private void Start() {
        _otherPlayer = GetComponent<OtherPlayer>();
    }

    private void Process(PlayerEvent pEvent) {
        switch (pEvent.type) {
                case PlayerEvent.Type.Hit:
                    Debug.Log("player " + _otherPlayer.id + " hitted " + pEvent.player);
                    OtherPlayersStatesProvider.Instance.DamagePlayer(pEvent.player);
                    break;
                
            case PlayerEvent.Type.Shoot:
                Debug.Log("player " + _otherPlayer.id + " shoted and missed");
                OtherPlayersStatesProvider.Instance.DamagePlayer(pEvent.player);
                break;
                
                case PlayerEvent.Type.ThrowGranade:
                    Debug.Log("player " + _otherPlayer.id + "Threw a grenade");
                    break;
        }
    }

    public void Damage() {
        _playerEventChannel.SendEvent(PlayerEvent.Hit(_otherPlayer.id));
    }
    
    public void SetupChannels(ChannelManager cm) {
        _playerEventChannel = new PlayerEventChannel((e) => Process(e), new TrivialStrategy());
        cm.RegisterChannel((int)RegisteredChannels.PlayerEventChannel, _playerEventChannel);
    }
}
