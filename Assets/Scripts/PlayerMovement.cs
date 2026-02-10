using SceneScript;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace cowsins2D
{
    [System.Serializable]
    public enum JumpMethod
    {
        Default, HoldToJumpHigher
    }

    [System.Serializable]
    public enum PlayerOrientationMethod
    {
        None,
        HorizontalInput,
        AimBased,
        Mixed
    }
    [System.Serializable]
    public enum TurnMethod
    {
        Simple,
        Smooth
    }

    [System.Serializable]
    public enum DashMethod
    {
        None,
        Default,
        AimBased,
        OrientationBased,
        HorizontalAimBased
    }
    [System.Serializable]
    public enum GlideDurationMethod
    {
        None,
        TimeBased
    }

    public class PlayerMovement : MonoBehaviour, IPlayerMovement
    {

        [System.Serializable]
        public class Sounds
        {
            public AudioClip jumpSFX, wallJumpSFX, dashSFX, startGlideSFX, startCrouchSFX;
        }

        [SerializeField, Tooltip("Reference to the player graphics.")] private Transform graphics;

        [Tooltip("Gravity strength."), SerializeField] private float gravityScale;
        [Tooltip("Gravity added. ( As a multiplier ).")][SerializeField] private float fallGravityMult;
        [Tooltip("Limit the maximum fall speed.")][SerializeField] private float maxFallSpeed;
        [Tooltip("Limit the maximum vertical speed ( going upwards )")][SerializeField] private float maxUpwardsSpeed;

        [Tooltip("If enabled, the player will not be able to walk, just to run.")]
        [SerializeField] private bool autoRun;
        [Tooltip("Speed while walking."), SerializeField] private float walkSpeed;
        [Tooltip("Speed while running."), SerializeField] private float runSpeed;
        [Tooltip("Speed while crouching."), SerializeField] private float crouchSpeed;
        [Tooltip("Speed horizontally while in a ladder."), SerializeField] private float horizontalLadderSpeed;
        [Tooltip("Speed to climb a ladder."), SerializeField] private float verticalLadderSpeed;
        [Tooltip("Automatically enter ladder state when a ladder is detected while airborne"), SerializeField] private bool autoEnterLadderInAir = false;
        [Tooltip("Capacity to gain speed and reach the maximum speed."), SerializeField] private float runAcceleration;
        [Tooltip("Capacity to lose speed and go idle."), SerializeField] private float runDecceleration;
        [Tooltip("Capacity to gain speed and reach the maximum speed mid-air.")][Range(0f, 1), SerializeField] private float airAcceleration;
        [Tooltip("Capacity to lose speed mid-air."), Range(0f, 1), SerializeField] private float airDeceleration;
        [Tooltip("Maximum floor angle to consider as a walkable area.")][SerializeField] private float maxFloorAngle;

        [Tooltip("Configures the way the jump works. Default: Press to jump. You are able to hold. Hold to Jump Higher: Press = Tiny jump. Hold = Higher jumps depending on the held time.")]
        [SerializeField] private JumpMethod jumpMethod;
        [Tooltip("How many jumps you can do before landing. 1 is the default value assigned.")][SerializeField, Min(0)] private int amountOfJumps;
        [Tooltip("How tall you will jump.")][SerializeField] private float jumpForce;

        [Header("JUMP CUSTOMIZATION")]
        [Tooltip("Adjust the stiffness of the jump. The higher this value is the sharper it will perform. " +
            "Usually, sharp jumps are more responsive than smooth jumps, but it all depends on the style you are looking for.")]
        [SerializeField] private float apexReachSharpness;
        [Tooltip("Jump Hang is the movement at the apex of the jump. This adjusts the gravity of the jump in that point."), Range(0f, 1), SerializeField] private float jumpHangGravityMult;
        [Tooltip("Enable jump hang depending on the current velocity, so it gets activated before reaching the apex.")][SerializeField] private float jumpHangTimeThreshold;
        [Tooltip("Acceleration multiplier on the apex of the jump.")]

        [SerializeField] private float jumpHangAccelerationMult;
        [Tooltip("Speed multiplier on the apex of the jump.")][SerializeField] private float jumpHangMaxSpeedMult;
        [Tooltip("When falling, you can press  S  to fall faster. Adjust the multiplier here.")][SerializeField] private float fastFallMultiplier;

        [SerializeField] private bool allowWallJump;

        [Header("WALL JUMP FORCES")]
        [Tooltip("Horizontal force applied away from the wall")][SerializeField] private float wallJumpHorizontalForce = 15f;
        [Tooltip("Vertical force applied upwards")][SerializeField] private float wallJumpVerticalForce = 18f;
        [Tooltip("Additional horizontal force when player holds direction away from wall ( Helps with movement forces and adds momentum)")][SerializeField] private float wallJumpDirectionalBonus;
        [Tooltip("Angle for wall jump (0 = horizontal, 90 = straight up). Usually 45-60 degrees.")][SerializeField, Range(0f, 90f)] private float wallJumpAngle;

        [Header("WALL JUMP CONTROL & TIMING")]
        [Tooltip("Duration where player has reduced control after wall jump. Gives a better sense of wall-jumping")][SerializeField, Range(0f, 0.5f)] private float wallJumpControlLockDuration;
        [Tooltip("(0 = no control, 1 = full control)")][SerializeField, Range(0f, 1f)] private float wallJumpControlStrength;
        [Tooltip("Time to gradually recover full control after lock duration")][SerializeField, Range(0f, 0.4f)] private float controlRecoveryDuration;
        [Tooltip("Grace period after leaving wall where wall jump is still allowed (kind of like coyote time)")][SerializeField, Range(0f, 0.15f)] private float wallJumpGraceTime;
        [Tooltip("Duration of the wall jump state before transitioning back to default")][SerializeField, Range(0f, 1.5f)] private float wallJumpStateDuration;

        [Header("WALL JUMP CHAINING")]
        [Tooltip("Enable faster jumping between walls")][SerializeField] private bool allowWallJumpChaining = true;
        [Tooltip("Velocity threshold to maintain momentum between wall jumps.")][SerializeField] private float wallJumpChainMomentumThreshold;

        [SerializeField] private bool allowSlide;

        [SerializeField, Tooltip("If disabled, the player will stick to the wall even if no Horizontal Input is applied.")] private bool requireHorizontalInputToSlide = false;

        [Tooltip("Enable to reset the amount of jumps when wall sliding.")]
        [SerializeField] private bool wallSlidingResetsJumps;
        [Tooltip("Velocity of wall sliding."), SerializeField] private float wallSlideSpeed;
        [Tooltip("Interval in seconds to display Wall slide visual effect...

< ...etc...>
