using InputManagerNS;
using UnityEngine;

public class PlayerInput {
    public static int ApplyInput(GameObject gameObject, PlayerAction playerAction) {
        
        float velocity = 25;
        //Running was to easy to escape from the enemies
        float sprintFactor = 0f;
        float backwardsPercentage = 0.5f;
        float lateralPercentage = 0.8f;
    
        Vector3 movement = new Vector3();
        if (playerAction.inputCommand == InputEnum.Forward) {
            movement.z += velocity;
//                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
//                    movement.z *= (1 + sprintFactor);
//                }
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
