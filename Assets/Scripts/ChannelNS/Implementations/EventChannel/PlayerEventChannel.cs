using System;
using System.Text;
using ChannelNS;
using SenderStrategyNS;
using UnityEngine;

public class PlayerEventChannel: EventChannel<PlayerEvent> {
    private readonly int _playerEvents = Enum.GetValues(typeof(PlayerEvent)).Length;
    
    private readonly float _minRot = 0;
    private readonly float _maxRot = 360;
    private readonly float _stepRot = 0.2f;
    
    private readonly int _minPlayers = 0;
    private readonly int _maxPlayers = 7;
    
    public PlayerEventChannel(Action<PlayerEvent> eventReceiver, SenderStrategy strategy) {
        SetupEventReceiver(eventReceiver);
        setupStrategy(strategy);
    }
        
    protected override PlayerEvent DeserializeData(byte[] bytes) {
        lock (this) {
            PlayerEvent pEvent;
            try {
                buffer.LoadBytes(bytes);
                buffer.ToRead();
                int player = 0;
                var eventType = (PlayerEvent.Type)buffer.ReadInt(0, _playerEvents);
                switch (eventType) {
                        case PlayerEvent.Type.ThrowGranade:
                            var rotX = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                            var rotY = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                            var rotZ = buffer.ReadFloat(_minRot, _maxRot, _stepRot);
                            return PlayerEvent.ThrowGranade(new Vector3(rotX, rotY, rotZ));
                        
                        case PlayerEvent.Type.Hit:
                            var target = buffer.ReadInt(_minPlayers, _maxPlayers);
                            return PlayerEvent.Hit(target);
                        
                        case PlayerEvent.Type.Die:
                            return PlayerEvent.Die();
                        
                        case PlayerEvent.Type.Shoot:
                            return PlayerEvent.Shoot();
                }
            } catch (Exception e) {
                Debug.LogError(e);
                throw;
            }
        }

        return null;
    }

    protected override byte[] SerializeData(PlayerEvent data) {
        lock (this) {
            try {
                buffer.ToWrite();
                buffer.WriteInt((int)data.type, 0, _playerEvents);

                switch (data.type) {
                    case PlayerEvent.Type.ThrowGranade:
                        buffer.WriteFloatRounded(data.direction.x, _minRot, _maxRot, _stepRot);
                        buffer.WriteFloatRounded(data.direction.y, _minRot, _maxRot, _stepRot);
                        buffer.WriteFloatRounded(data.direction.z, _minRot, _maxRot, _stepRot);
                        break;
                        
                    case PlayerEvent.Type.Hit:
                        buffer.WriteInt(data.player, _minPlayers, _maxPlayers);
                        break;
                }
            } catch (Exception e) {
                Debug.LogError(e);
                throw;
            }

            return buffer.GenerateBytes();
        }
    }
}
