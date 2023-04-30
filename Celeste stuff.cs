using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Celestestuff : MonoBehaviour
{
    #region constants

    //public const float maxRun = 2.0f;
    //public const float runAccel = 5.0f;
    //private const float runReduce = 2.0f;

    //private const float holdingMaxRun = 1.4f;
    //private const float holdingMinTime = 0.35f;

    //private const float autoAbilityTime = 0.1f;

    //private const float duckFriction = 500f;
    //private const int duckCorrectCheck = 4;
    //private const float duckCorrectSlide = 50f;

    //private const float blockGraceTime = 0.1f;
    //private const int cornerCorrection = 4;

    //private const int abilityCheckDistance = 3;

    //private const float dodgeSpeed = 240f;
    //private const float endDodgeSpeed = 160f;
    //private const float dashTime = 0.15f;
    //private const float dashCooldown = 0.2f;
    //private const float dashRefillCooldown = 0.1f;
    //private const int dashThroughNudge = 6;
    //private const int dashCornerCorrection = 4;
    //private const float dashAttackTime = 0.3f;

    //private const float boostMoveSpeed = 80f;
    //public const float boostTime = 0.25f;

    //private const float duckWindMult = 0f;
    //private const int windWallDist = 3;

    //private const float reboundSpeedX = 120f;
    //private const float reboundSpeedY = 120f;
    //private const float reboundVarDashTime = 0.15f;

    //private const float reflectBoundSpeed = 220f;

    //public const float climbMaxStamina = 110;
    //private const float climbUpCost = 100 / 2.2f;
    //private const float climbStillCost = 100 / 10f;
    //private const float climbJumpCost = 110 / 4f;
    //private const int climbCheckDist = 2;
    //private const int climbUpCheckDist = 2;
    //private const float climbNoMoveTime = .1f;
    //public const float climbTiredThreshold = 20f;
    //private const float climbUpSpeed = -45f;
    //private const float climbDownSpeed = 80f;
    //private const float climbSlipSpeed = 30f;
    //private const float climbAccel = 900f;
    //private const float climbGrabYMult = .2f;
    //private const float climbHopY = -120f;
    //private const float climbHopX = 100f;
    //private const float climbHopForceTime = .2f;
    //private const float climbJumpBoostTime = .2f;
    //private const float climbHopNoWindTime = .3f;

    //private const float launchSpeed = 280f;
    //private const float launchCancelThreshold = 220f;

    //private const float throwRecoil = 80f;
    //private static readonly Vector2 carryOffsetTarget = new Vector2(0, -12);

    //private const float ChaserStateMaxTime = 4f;

    //public const float WalkSpeed = 64f;

    //public const int StNormal = 0;
    //public const int StClimb = 1;
    //public const int StDash = 2;
    //public const int StSwim = 3;
    //public const int StBoost = 4;
    //public const int StRedDash = 5;
    //public const int StHitSquash = 6;
    //public const int StLaunch = 7;
    //public const int StPickup = 8;
    //public const int StDreamDash = 9;
    //public const int StSummitLaunch = 10;
    //public const int StDummy = 11;
    //public const int StIntroWalk = 12;
    //public const int StIntroJump = 13;
    //public const int StIntroRespawn = 14;
    //public const int StIntroWakeUp = 15;
    //public const int StBirdDashTutorial = 16;
    //public const int StFrozen = 17;
    //public const int StAttract = 22;

    //public const string TalkSfx = "player_talk";

    #endregion

    #region vars

    //public Vector2 Speed;
    // public Facings Facing;
    // public PlayerSprite Sprite;
    // public PlayerHair Hair;
    // public StateMachine StateMachine;
    //public Vector2 CameraAnchorLerp;
    //public bool ForceCameraUpdate;
    // public Leader Leader;
    // public VertexLight Light;
    //    public int Dashes;
    // public float Stamina = ClimbMaxStamina;
    //public Vector2 PreviousPosition;
    //public bool DummyAutoAnimate = true;
    //public Vector2? OverrideDashDirection;
    //public bool FlipInReflection = false;
    //public bool JustRespawned;  // True if the player hasn't moved since respawning
    // public bool Dead { get; private set; }

    //// private Level level;
    //private bool onGround;
    //private bool wasOnGround;
    //private bool flash;
    //private bool wasDucking;

    //private float idleTimer;
    // private static Chooser<string> idleOptions = new Chooser<string>().Add("idleA", 5f).Add("idleB", 3f).Add("idleC", 1f);

    //private Hitbox hurtbox;
    //private float jumpGraceTimer;
    //public bool AutoJump;
    //public float AutoJumpTimer;
    //private float varJumpSpeed;
    //private float varJumpTimer;
    //private int forceMoveX;
    //private float forceMoveXTimer;
    //private int hopWaitX;   // If you climb hop onto a moving solid, snap to beside it until you get above it
    //private float hopWaitXSpeed;
    //private Vector2 lastAim;
    //private float dashCooldownTimer;
    //private float dashRefillCooldownTimer;
    //    public Vector2 DashDir;
    // private float wallSlideTimer = WallSlideTime;
    //private int wallSlideDir;
    //private float climbNoMoveTimer;
    //private Vector2 carryOffset;
    //private Vector2 deadOffset;
    //private float introEase;
    //private float wallSpeedRetentionTimer; // If you hit a wall, start this timer. If coast is clear within this timer, retain h-speed
    //private float wallSpeedRetained;
    //private int wallBoostDir;
    //private float wallBoostTimer;   // If you climb jump and then do a sideways input within this timer, switch to wall jump
    //private float maxFall;
    //    private float dashAttackTimer;
    // private List<ChaserState> chaserStates;
    //private bool wasTired;
    // private HashSet<Trigger> triggersInside;
    //private float highestAirY;
    //private bool dashStartedOnGround;
    //private bool fastJump;
    //private int lastClimbMove;
    //private float noWindTimer;
    //private float dreamDashCanEndTimer;
    // private Solid climbHopSolid;
    //private Vector2 climbHopSolidPosition;
    // private SoundSource wallSlideSfx;
    // private SoundSource swimSurfaceLoopSfx;
    //private float playFootstepOnLand;
    //private float minHoldTimer;
    // public Booster CurrentBooster;
    // private Booster lastBooster;
    //private bool calledDashEvents;
    //private int lastDashes;
    //private Sprite sweatSprite;
    //private int startHairCount;
    // private bool launched;
    // private float launchedTimer;
    // private float dashTrailTimer;
    // private List<ChaserStateSound> activeSounds = new List<ChaserStateSound>();
    // private FMOD.Studio.EventInstance idleSfx;

    //private readonly Hitbox normalHitbox = new Hitbox(8, 11, -4, -11);
    // private readonly Hitbox duckHitbox = new Hitbox(8, 6, -4, -6);
    //private readonly Hitbox normalHurtbox = new Hitbox(8, 9, -4, -11);
    // private readonly Hitbox duckHurtbox = new Hitbox(8, 4, -4, -6);
    // private readonly Hitbox starFlyHitbox = new Hitbox(8, 8, -4, -10);
    // private readonly Hitbox starFlyHurtbox = new Hitbox(6, 6, -3, -9);

    // private Vector2 normalLightOffset = new Vector2(0, -8);
    // private Vector2 duckingLightOffset = new Vector2(0, -3);

    // private List<Entity> temp = new List<Entity>();

    // public static readonly Color NormalHairColor = Calc.HexToColor("AC3232");
    // public static readonly Color FlyPowerHairColor = Calc.HexToColor("F2EB6D");
    // public static readonly Color UsedHairColor = Calc.HexToColor("44B7FF");
    // public static readonly Color FlashHairColor = Color.White;
    // public static readonly Color TwoDashesHairColor = Calc.HexToColor("ff6def");
    // private float hairFlashTimer;
    // public Color? OverrideHairColor;

    // private Vector2 windDirection;
    // private float windTimeout;
    // private float windHairTimer;

    // public enum IntroTypes { Transition, Respawn, WalkInRight, WalkInLeft, Jump, WakeUp, Fall, TempleMirrorVoid, None }
    // public IntroTypes IntroType;

    // private MirrorReflection reflection;

    #endregion

    #region constructor / added / removed

    #endregion
}
