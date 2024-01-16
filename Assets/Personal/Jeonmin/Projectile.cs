using UnityEngine;
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using static MoreMountains.Tools.MMObjectBounds;
using Structs;

/// </summary>
public class Projectile : MonoBehaviour
{
    [Header("Bounciness Tech")]
    /// the length of the raycast used to detect bounces, should be proportionate to the size and speed of your projectile
    [Tooltip("the length of the raycast used to detect bounces, should be proportionate to the size and speed of your projectile")]
    public float BounceRaycastLength = 1f;
    /// the layers you want this projectile to bounce on
    [Tooltip("the layers you want this projectile to bounce on")]
    public LayerMask BounceLayers;
    /// a feedback to trigger at every bounce
    [Tooltip("a feedback to trigger at every bounce")]
    public MMFeedbacks BounceFeedback;

    [Header("Bounciness")]
    /// the min and max amount of bounces (a value will be picked at random between both bounds)
    [Tooltip("the min and max amount of bounces (a value will be picked at random between both bounds)")]
    [MMVector("Min", "Max")]
    public Vector2Int AmountOfBounces = new Vector2Int(10, 10);
    /// the min and max speed multiplier to apply at every bounce (a value will be picked at random between both bounds)
    [Tooltip("the min and max speed multiplier to apply at every bounce (a value will be picked at random between both bounds)")]
    [MMVector("Min", "Max")]
    public Vector2 SpeedModifier = Vector2.one;

    [Header("BounceEvent")]
    public UnityEvent ExecuteOnBounce;
    public int testb = 1;

    protected Vector3 _positionLastFrame;
    protected Vector3 _raycastDirection;
    protected Vector3 _reflectedDirection;
    protected int _randomAmountOfBounces;
    protected int _bouncesLeft;
    protected float _randomSpeedModifier;


    public enum MovementVectors { Forward, Right, Up }

    [Header("Movement")]
    /// if true, the projectile will rotate at initialization towards its rotation
    [Tooltip("if true, the projectile will rotate at initialization towards its rotation")]
    public bool FaceDirection = true;
    /// if true, the projectile will rotate towards movement
    [Tooltip("if true, the projectile will rotate towards movement")]
    public bool FaceMovement = false;
    /// if FaceMovement is true, the projectile's vector specified below will be aligned to the movement vector, usually you'll want to go with Forward in 3D, Right in 2D
    [Tooltip("if FaceMovement is true, the projectile's vector specified below will be aligned to the movement vector, usually you'll want to go with Forward in 3D, Right in 2D")]
    [MMCondition("FaceMovement", true)]
    public MovementVectors MovementVector = MovementVectors.Forward;

    /// the speed of the object (relative to the level's speed)
    [Tooltip("the speed of the object (relative to the level's speed)")]
    public float Speed = 0;
    /// the acceleration of the object over time. Starts accelerating on enable.
    [Tooltip("the acceleration of the object over time. Starts accelerating on enable.")]
    public float Acceleration = 0;
    /// the current direction of the object
    [Tooltip("the current direction of the object")]
    public Vector3 Direction = Vector3.left;
    /// if set to true, the spawner can change the direction of the object. If not the one set in its inspector will be used.
    [Tooltip("if set to true, the spawner can change the direction of the object. If not the one set in its inspector will be used.")]
    public bool DirectionCanBeChangedBySpawner = true;
    /// the flip factor to apply if and when the projectile is mirrored
    [Tooltip("the flip factor to apply if and when the projectile is mirrored")]
    public Vector3 FlipValue = new Vector3(-1, 1, 1);
    /// set this to true if your projectile's model (or sprite) is facing right, false otherwise
    [Tooltip("set this to true if your projectile's model (or sprite) is facing right, false otherwise")]
    public bool ProjectileIsFacingRight = true;

    /// should the projectile damage its owner?
    [Tooltip("should the projectile damage its owner?")]
    public bool DamageOwner = false;

    [Header("Distance Limit")]
    public bool DistanceLimit = false;
    public float DistanceLimitValue;
    private Vector2 initialPosition;
    private float traveledDistance;

    protected Weapon _weapon;
    protected GameObject _owner;
    protected Vector3 _movement;
    protected float _initialSpeed;
    protected SpriteRenderer _spriteRenderer;
    protected Collider2D _collider2D;
    protected Rigidbody2D _rigidBody2D;
    protected bool _facingRightInitially;
    protected bool _initialFlipX;
    protected Vector3 _initialLocalScale;
    protected bool _shouldMove = true;
    protected bool _spawnerIsFacingRight;

    public BulletStat defaultStat;

    private void Awake()
    {
        _facingRightInitially = ProjectileIsFacingRight;
        _initialSpeed = Speed;
        _collider2D = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidBody2D = GetComponent<Rigidbody2D>();
        if (_spriteRenderer != null) { _initialFlipX = _spriteRenderer.flipX; }
        _initialLocalScale = transform.localScale;
    }

    private void Initialization()
    {
        _randomAmountOfBounces = Random.Range(AmountOfBounces.x, AmountOfBounces.y);
        _randomSpeedModifier = Random.Range(SpeedModifier.x, SpeedModifier.y);
        _bouncesLeft = _randomAmountOfBounces;
        if (_collider2D != null)
        {
            _collider2D.enabled = false;
            _collider2D.enabled = true;
        }
    }

    /// <summary>
    /// On trigger enter 2D, we call our colliding endpoint
    /// </summary>
    /// <param name="collider"></param>S
    public virtual void OnTriggerEnter2D(Collider2D collider)
    {
        Colliding(collider.gameObject);
    }

    /// <summary>
    /// Colliding endpoint
    /// </summary>
    /// <param name="collider"></param>
    protected virtual void Colliding(GameObject collider)
    {
        if (!BounceLayers.MMContains(collider.layer))
        {
            return;
        }

        _raycastDirection = (this.transform.position - _positionLastFrame);

        RaycastHit2D hit = MMDebug.RayCast(this.transform.position, Direction.normalized, BounceRaycastLength, BounceLayers, MMColors.DarkOrange, true);
        EvaluateHit2D(hit);
    }

    protected virtual void EvaluateHit2D(RaycastHit2D hit)
    {
        if (hit)
        {
            CObj cObj = hit.transform.gameObject.GetComponent<CObj>();
            if (cObj)
            {
                Vector2 normal = hit.point;
                Vector2 normalDir = new Vector2(transform.position.x, transform.position.y) - normal;

                HitInfo hitinfo = cObj.Hit(defaultStat.dmg, normalDir);

                GameObject particle = PoolingManager.Instance.LentalObj("Effect_Smoke_04");
                particle.transform.localPosition = this.transform.position;
                PoolingManager.Instance.ReturnObj(this.gameObject);
            }

            if (_bouncesLeft > 0)
            {
                Bounce2D(hit);
                ExecuteOnBounce?.Invoke();
            }
            else
            {
                GameObject particle = PoolingManager.Instance.LentalObj("Effect_Smoke_04");
                particle.transform.localPosition = this.transform.position;
                PoolingManager.Instance.ReturnObj(this.gameObject);
            }
        }
    }


    public void PreventedCollision2D(RaycastHit2D hit)
    {
        _raycastDirection = transform.position - _positionLastFrame;
        EvaluateHit2D(hit);
    }

    /// <summary>
    /// Applies a bounce in 2D
    /// </summary>
    /// <param name="hit"></param>
    protected virtual void Bounce2D(RaycastHit2D hit)
    {
        BounceFeedback?.PlayFeedbacks();
        _reflectedDirection = Vector3.Reflect(_raycastDirection, hit.normal);
        float angle = Vector3.Angle(_raycastDirection, _reflectedDirection);
        Direction = _reflectedDirection.normalized;
        this.transform.right = _spawnerIsFacingRight ? _reflectedDirection.normalized : -_reflectedDirection.normalized;
        Speed *= _randomSpeedModifier;
        _bouncesLeft--;
    }

    public void Movement()
    {
        _movement = Direction * (Speed / 10) * Time.deltaTime;
        if (_rigidBody2D != null)
        {
            _rigidBody2D.MovePosition(this.transform.position + _movement);
        }
        // We apply the acceleration to increase the speed
        Speed += Acceleration * Time.deltaTime;
    }

    public virtual void SetDirection(Vector3 newDirection, Quaternion newRotation, bool spawnerIsFacingRight = true)
    {
        _spawnerIsFacingRight = spawnerIsFacingRight;

        if (DirectionCanBeChangedBySpawner)
        {
            Direction = newDirection;
        }
        if (ProjectileIsFacingRight != spawnerIsFacingRight)
        {
            Flip();
        }
        if (FaceDirection)
        {
            transform.rotation = newRotation;
        }

        if (FaceMovement)
        {
            switch (MovementVector)
            {
                case MovementVectors.Forward:
                    transform.forward = newDirection;
                    break;
                case MovementVectors.Right:
                    transform.right = newDirection;
                    break;
                case MovementVectors.Up:
                    transform.up = newDirection;
                    break;
            }
        }
    }

    protected virtual void Flip()
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = !_spriteRenderer.flipX;
        }
        else
        {
            this.transform.localScale = Vector3.Scale(this.transform.localScale, FlipValue);
        }
    }

    protected virtual void Flip(bool state)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.flipX = state;
        }
        else
        {
            this.transform.localScale = Vector3.Scale(this.transform.localScale, FlipValue);
        }
    }

    protected virtual void FixedUpdate()
    {
        if (_shouldMove)
        {
            Movement();
            CalcDistance();
        }
    }

    private void CalcDistance()
    {
        traveledDistance = Vector2.Distance(initialPosition, transform.position);
        if (DistanceLimit)
        {
            if (traveledDistance > DistanceLimitValue) this.gameObject.SetActive(false);
        }
    }

    protected virtual void LateUpdate()
    {
        _positionLastFrame = this.transform.position;
    }

    protected void OnEnable()
    {
        Initialization();
    }

    /// <summary>
    /// On disable, we plug our OnDeath method to the health component
    /// </summary>
    protected void OnDisable()
    {

    }
}