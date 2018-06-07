using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    
}

public static class CommonState
{
    public const string IDLE = "Idle";
    public const string MOVING = "Moving";
    public const string JUMPING = "Jumping";
    public const string CROUCH = "Jumping";
    public const string DIE = "Die";

    public const string ONSIGHT = "OnSight";
    public const string PURSUIRT = "Pursuit";
    public const string OUT_OF_SIGHT = "OutOfSight";

    public const string ATTACKING = "Attacking";
}

public static class PHYSICAL_INPUT
{
    public const string HORIZONTAL = "Horizontal";
    public const string VERTICAL = "Vertical";
    public const string JUMP = "Jump";
    public const string FIRE1 = "Fire1";
    public const string CROUCH = "Crouch";
}

public static class SomeExtensions
{

}
