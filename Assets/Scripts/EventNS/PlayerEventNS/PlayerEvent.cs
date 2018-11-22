using UnityEngine;

namespace EventNS.PlayerEventNS {
    public class PlayerEvent {
        public enum Type {
            Shoot = 1,
            Hit,
            ThrowGranade,
            Die,
            Connect,
            Connected,
            Respawn,
        }

        public Type type;
        public int player;
        public Vector3 direction;


        private PlayerEvent(Type type, int player, Vector3 direction) {
            this.type = type;
            this.player = player;
            this.direction = direction;
        }
    
        public static PlayerEvent Shoot() {
            return new PlayerEvent(Type.Shoot, 0, Vector3.zero);
        }
    
        public static PlayerEvent Hit(int player) {
            return new PlayerEvent(Type.Hit, player, Vector3.zero);
        }
    
        public static PlayerEvent ThrowGranade(Vector3 direction) {
            return new PlayerEvent(Type.ThrowGranade, 0, direction);
        }

        public static PlayerEvent Die() {
            return new PlayerEvent(Type.Die, 0, Vector3.zero);
        } 
        
        public static PlayerEvent Connect() {
            return new PlayerEvent(Type.Connect, 0, Vector3.zero);
        } 
        
        public static PlayerEvent Connected() {
            return new PlayerEvent(Type.Connected, 0, Vector3.zero);
        } 
        
        public static PlayerEvent Respawn() {
            return new PlayerEvent(Type.Respawn, 0, Vector3.zero);
        } 
    }
}
