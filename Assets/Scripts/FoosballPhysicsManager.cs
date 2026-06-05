using UnityEngine;

public class FoosballPhysicsManager : MonoBehaviour
{
    [Header("Core Objects")]
    [Tooltip("Drag the Ball Rigidbody here")]
    public Rigidbody ball;
    
    [Tooltip("Drag all of your top-level Rod objects here. (This script only reads their speed!)")]
    public RodController[] allRods;

    [Header("Real-World Physics Tuning")]
    [Tooltip("The mass of the player's foot. Higher mass = harder hits.")]
    public float playerMass = 5f;
    [Tooltip("How much the rod's spin speed multiplies the kick force.")]
    public float spinPowerMultiplier = 25f;
    [Tooltip("Hard speed limit to prevent the ball from glitching through the table.")]
    public float maxBallSpeed = 40f; 
    [Tooltip("How bouncy the ball is against walls and players (0 to 1).")]
    public float ballBounciness = 0.8f;

    void Start()
    {
        AutoConfigureBallPhysics();
        InjectCollisionSensors();
    }

    void AutoConfigureBallPhysics()
    {
        if (ball == null) return;

        // 1. Unbreakable Collision Settings for fast movement
        ball.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        ball.interpolation = RigidbodyInterpolation.Interpolate;
        
        // 2. Realistic weight and drag
        ball.mass = 1.5f; 
        ball.angularDamping = 1.0f; // Friction against spinning in the air
        
        // 3. Lock to the table height, but allow it to roll naturally (spin on X/Z)
        ball.constraints = RigidbodyConstraints.FreezePositionY; 

        // 4. AUTO-GENERATE PERFECT PHYSICS MATERIAL
        // This ensures the ball slides on the grass and bounces off walls perfectly
        PhysicsMaterial physicalMat = new PhysicsMaterial("BallPhysics");
        physicalMat.bounciness = ballBounciness;
        physicalMat.dynamicFriction = 0f; // 0 friction so it doesn't stick to the floor
        physicalMat.staticFriction = 0f;
        
        // Force Unity to prioritize these bouncy/slippery settings over the floor's settings
        physicalMat.frictionCombine = PhysicsMaterialCombine.Minimum;
        physicalMat.bounceCombine = PhysicsMaterialCombine.Maximum;

        ball.GetComponent<Collider>().material = physicalMat;
    }

    void InjectCollisionSensors()
    {
        // Go through every rod and silently attach a collision listener to the player figures
        foreach (RodController rod in allRods)
        {
            if (rod == null) continue;

            Collider[] players = rod.GetComponentsInChildren<Collider>();
            foreach (Collider player in players)
            {
                PhysicsHitListener listener = player.gameObject.AddComponent<PhysicsHitListener>();
                listener.engine = this;
                listener.parentRod = rod;
            }
        }
    }

    void FixedUpdate()
    {
        // Enforce the speed limit in the physics loop
        if (ball != null && ball.linearVelocity.magnitude > maxBallSpeed)
        {
            ball.linearVelocity = ball.linearVelocity.normalized * maxBallSpeed;
        }
    }

    // Called automatically by the sensors when a player foot physically hits the ball
    public void ProcessRealCollision(RodController hittingRod, Collision collision)
    {
        if (ball == null) return;

        // Read the spin speed directly from your existing input script
        float rodSpinSpeed = 0f;
        if (hittingRod != null)
        {
            rodSpinSpeed = Mathf.Abs(hittingRod.SpinVelocity);
        }
        
        // If the rod is barely moving, let Unity handle it as a standard passive bump
        if (rodSpinSpeed < 0.1f) return; 

        // Calculate kinetic energy transfer based on spin speed and mass
        float impactForce = rodSpinSpeed * playerMass * spinPowerMultiplier * Time.fixedDeltaTime;

        // Get the exact point where the plastic foot touched the ball's curve
        Vector3 contactPoint = collision.GetContact(0).point;
        
        // Find the direction from the foot to the center of the ball, kept flat
        Vector3 forceDirection = (ball.position - contactPoint).normalized;
        forceDirection.y = 0; 

        // APPLY TRUE PHYSICS:
        // AddForceAtPosition pushes the ball off-center, causing it to naturally roll and spin!
        ball.AddForceAtPosition(forceDirection * Mathf.Abs(impactForce), contactPoint, ForceMode.Impulse);
    }
}

// =====================================================================
// HIDDEN LISTENER CLASS 
// (Lives inside the same file. Just catches collisions and reports them)
// =====================================================================
public class PhysicsHitListener : MonoBehaviour
{
    [HideInInspector] public FoosballPhysicsManager engine;
    [HideInInspector] public RodController parentRod;

    private void OnCollisionEnter(Collision collision)
    {
        if (engine != null && collision.rigidbody == engine.ball)
        {
            engine.ProcessRealCollision(parentRod, collision);
        }
    }
}