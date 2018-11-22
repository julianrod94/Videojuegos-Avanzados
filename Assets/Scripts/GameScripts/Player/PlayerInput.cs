using EventNS.InputSequenceNS;
using UnityEngine;

namespace GameScripts.Player {
    public class PlayerInput {
        public static int ApplyInput(GameObject gameObject, PlayerAction playerAction) {
        
            float velocity = 25;
            //Running was to easy to escape from the enemies
            float backwardsPercentage = 0.5f;
            float lateralPercentage = 0.8f;
    
            Vector3 movement = new Vector3();
            gameObject.transform.rotation = playerAction.rotation;
            if (playerAction.inputCommand == InputEnum.Forward) {
                movement.z += velocity;
            }

            if (playerAction.inputCommand == InputEnum.Left) {
                movement.x -= velocity * lateralPercentage;
            }

            if (playerAction.inputCommand == InputEnum.Right) {
                movement.x += velocity * lateralPercentage;
            }

            if (playerAction.inputCommand == InputEnum.Back) {
                movement.z -= velocity * backwardsPercentage;
            }

    
            gameObject.transform.Translate(movement * playerAction.deltaTime);
            return playerAction.inputNumber;
        }
    }
}
