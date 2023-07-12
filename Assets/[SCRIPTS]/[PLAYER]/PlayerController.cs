using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region VARIABLES

    [Header("MOVEMENT")]

    // редактируемые
    [Tooltip("Нормальная скорость персонажа")]
    [SerializeField] float _speed;
    [Tooltip("Скорость персонажа в воздухе (коэффициент)")]
    [Range(0.1f, 1f)][SerializeField] float _speedAirCoef;
    [Tooltip("Сила прыжка")]
    [SerializeField] float _jumpForce;
    [Tooltip("Задержка удержания прыжка")]
    [Range(0.001f, 5f)][SerializeField] float _jumpDelayTime;
    [Tooltip("Задержка нахождения на земле")]
    [Range(0.001f, 5f)][SerializeField] float _groundDelayTime;
    [Tooltip("Длина луча, проверяющего землю")]
    [Range(0.001f, 1f)][SerializeField] float _groundRayDist;
    [Tooltip("Длина луча, проверяющего платформу")]
    [Range(0.001f, 1f)][SerializeField] float _platformRayDist;

    [Header("DAMAGE CONTROL")]

    // редактируемые
    [Tooltip("Наносимый урон противнику")]
    [SerializeField] public int damage;
    [Tooltip("Время получения урона")]
    [Range(0, 1f)][SerializeField] private float _takingHitTime;
    [SerializeField] private Vector2 _hitForceVector;

    // не редактируемые
    private bool _isTakingHit;

    [Header("LAYER MASKS")]

    [SerializeField] LayerMask _groundLayerMask;
    [SerializeField] LayerMask _platformLayerMask;

    [Header("DEBUG")]

    // редактируемые BOOL
    [SerializeField] private bool _showRays;

    // не редактируемые BOOL
    private bool _isFacingRight;
    private bool _canJump;
    private bool _isHoldGround;

    // проверяются физикой BOOL
    private bool _isGrounded;
    private bool _isPlatformAbove;

    // проверяются с нажатием BOOL
    private bool _wantJump;

    // ссылки на компоненты
    private Rigidbody2D RB;
    private Animator _animator;
    private PlayerHP _playerHP;
    private Transform _groundCheck;
    private Transform _platformCheck;

    // input
    private Vector2 _horizontal;
    private bool _jumpButton;
    private bool _downJumpButton;
    private bool _downJumpButtonHold;

    // остальное
    private int playerLayer, platformLayer;


    #endregion

    #region UNITY METHODS

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        PlayerInput();
        GroundCheck();
        JumpControl();
        ShowRays();
    }

    private void FixedUpdate()
    {
        MoveControl();
    }

    #endregion

    #region CUSTOM METHODS

    private void Initialize()
    {
        // компоненты
        RB = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _playerHP = GetComponent<PlayerHP>();

        _groundCheck = transform.Find("GroundCheck").transform;
        _platformCheck = transform.Find("PlatformCheck").transform;

        // переменные
        _isFacingRight = true;
        _canJump = true;
        _wantJump = false;
        _isHoldGround = false;

        // назначение слоев
        playerLayer = LayerMask.NameToLayer("10 PlayerCollider");
        platformLayer = LayerMask.NameToLayer("24 Platforms");
    }

    private void PlayerInput()
    {
        if (_isTakingHit) return;

        _horizontal.x = Input.GetAxisRaw("Horizontal");
        _jumpButton = Input.GetButtonDown("Jump");
        _downJumpButton = Input.GetButtonDown("Down Jump");
        _downJumpButtonHold = Input.GetButton("Down Jump");
    }

    private void MoveControl()
    {
        // flip control
        if (_horizontal.x > 0 && !_isFacingRight) FlipX();
        else if (_horizontal.x < 0 && _isFacingRight) FlipX();

        // обычное перемещение
        if (_isGrounded && _horizontal.x != 0f && !_isTakingHit)
        {
            RB.velocity = new Vector2(_horizontal.x * _speed, RB.velocity.y);
            _animator.SetBool("Run", true);
        }
        // когда персонаж двигается в воздухе
        else if (!_isGrounded && _horizontal.x != 0f && !_isTakingHit)
        {
            RB.velocity = new Vector2(_horizontal.x * _speed * _speedAirCoef, RB.velocity.y);
        }
        // когда персонаж не двигается
        else if (_isGrounded && _horizontal.x == 0f && !_isTakingHit)
        {
            _animator.SetBool("Run", false);
            RB.velocity = new Vector2(0f, RB.velocity.y);
        }
        // когда персонаж получает урон
        else if (_isTakingHit)
        {
            RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y);
        }

        // контроль анимаций
        if (_isGrounded) _animator.SetBool("Falling", false);
        else _animator.SetBool("Falling", true);
    }

    private void FlipX()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void GroundCheck()
    {
        _isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundRayDist, _groundLayerMask);

        if (_isGrounded)
        {
            if (_isHoldGround) return;
            StartCoroutine(HoldGround());
        }
    }

    private void JumpControl()
    {
        // прыжок
        if ((_jumpButton ^ _wantJump) && (_isGrounded || _isHoldGround) && _canJump && !_isTakingHit) StartCoroutine(Jump());
        // запоминание прыжка
        if (_jumpButton && !_isGrounded && !_isTakingHit) StartCoroutine(HoldJump());
       
        // контроль перепрыгивания платформы
        if (_downJumpButtonHold) _isPlatformAbove = Physics2D.OverlapCircle(_platformCheck.position, _platformRayDist + 0.1f, _platformLayerMask);
        else _isPlatformAbove = Physics2D.OverlapCircle(_platformCheck.position, _platformRayDist, _platformLayerMask);

        if (_downJumpButton || _isPlatformAbove)
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, true);
        }
        else
        {
            Physics2D.IgnoreLayerCollision(playerLayer, platformLayer, false);
        }
    }

    private void ShowRays()
    {
        if (_showRays)
        {
            Debug.DrawRay(_groundCheck.position, Vector3.down * _groundRayDist, Color.yellow);

            Debug.DrawRay(_platformCheck.position, Vector3.up * _platformRayDist, Color.red);
            Debug.DrawRay(_platformCheck.position, Vector3.down * _platformRayDist, Color.red);
            Debug.DrawRay(_platformCheck.position, Vector3.right * _platformRayDist, Color.red);
            Debug.DrawRay(_platformCheck.position, Vector3.left * _platformRayDist, Color.red);
        }
    }

    public void TakeDamage(int damage, float force, int side)
    {
        if (side == 1 && _isTakingHit == false)
        {
            StartCoroutine(LoseControl(damage,
                new Vector3(-transform.localScale.x * _hitForceVector.x, _hitForceVector.y, 0f),
                force));
        }
        else if(side == 2 && _isTakingHit == false)
        {
            StartCoroutine(LoseControl(damage,
                new Vector3(transform.localScale.x * _hitForceVector.x, _hitForceVector.y, 0f),
                force));
        }
        else if (side == 3 && _isTakingHit == false)
        {
            StartCoroutine(LoseControl(damage,
                new Vector3(0f, -_hitForceVector.y, 0f),
                force));
        }
    }
    #endregion

    #region COROUTINES

    private IEnumerator LoseControl(int damage, Vector3 dir, float force)
    {
        
        _playerHP.TakeDamage(-damage);
        _isTakingHit = true;
        _animator.SetTrigger("Hit");
        RB.AddRelativeForce(dir * force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(_takingHitTime);
        _isTakingHit = false;

    }

    public IEnumerator Jump()
    {
        _canJump = false;
        _animator.SetTrigger("Jump");
        RB.AddRelativeForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(_jumpDelayTime);
        _canJump = true;
    }

    private IEnumerator HoldJump()
    {   // корутина включает переменную wantJump
        // для возможности прыжка сразу после приземления

        _wantJump = true;
        yield return new WaitForSeconds(_jumpDelayTime);
        _wantJump = false;
    }

    private IEnumerator HoldGround()
    {   // корутина включает переменную isHoldGround
        // для возможности прыжка когда isGrounded = false

        _isHoldGround = true;
        yield return new WaitForSeconds(_groundDelayTime);
        _isHoldGround = false;
    }

    #endregion
}
