using System;
using UnityEngine;

public class Tags {
    public const String ball = "Ball";
    public const String weapon = "Weapon";
    public const String holder = "Holder";
    public const String player = "Player";
    public const String enemy = "Enemy";
    public const String spawn = "Spawn";
}

public class AnimatorParameters {
    public const String enemyAttacking = "attacking";
    public const String enemyDead = "dead";


}

public class AnimatorStates {
    public const String enemyAttack = "attack";
    public const String dealDamage = "deal_damage";


}

public class Layers {
    private const String _floor = "Floor";

    public static int floor = LayerMask.NameToLayer(_floor);
}