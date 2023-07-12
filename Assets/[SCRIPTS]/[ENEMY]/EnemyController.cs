using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region VARIABLES

    [Header("BEHAVIOUR")]
    [Tooltip("Использовать таймер для разворота")]
    [SerializeField] private bool _useTimer;
    [Tooltip("Агрессивен ли противник")]
    [SerializeField] private bool _isAgressvie;

    [Header("MOVEMENT")]

    // редактируемые

    [Tooltip("Нормальная скорость")]
    [SerializeField] private float _walkSpeed;
    [Tooltip("Скорость атаки")]
    [SerializeField] private float _attackSpeed;
    [Tooltip("Время смены направдления")]
    [SerializeField] private float _changeDirTime;
    [Tooltip("Расстояние, на котором противник может обнаружить игрока")]
    [SerializeField] private float _attackRayDist;
    [Tooltip("Время атаки")]
    [SerializeField] private float _attackTime;

    [Tooltip("Длина луча, проверяющего землю")]
    [Range(0.001f, 1f)][SerializeField] float _groundRayDist;
    [Tooltip("Длина луча, проверяющего стены")]
    [Range(0.001f, 1f)][SerializeField] float _wallRayDist;

    [Header("DAMAGE CONTROL")]

    [Tooltip("Наносимый урон игроку")]
    [SerializeField] public int damage;
    [Tooltip("Сила удара по игроку")]
    [SerializeField] public float attackForce;
    [Tooltip("Время получения урона от игрока")]
    [Range(0, 1f)][SerializeField] private float _takingHitTime;

    [Header("LAYER MASKS")]

    [SerializeField] LayerMask _playerLayerMask;
    [SerializeField] LayerMask _wallGroundLayerMask;
    [SerializeField] LayerMask _groundLayerMask;

    [Header("DEBUG")]

    // редактируемые BOOL
    [SerializeField] private bool _showRays;

    // не редактируемые BOOL
    [SerializeField] public bool _isTakingHit;

    // проверяются физикой BOOL
    [SerializeField] private bool _isGroundedMain;
    [SerializeField] private bool _isGroundedForward;
    [SerializeField] private bool _isWallAhead;
    [SerializeField] private bool _seePlayer;


    // ссылки на компоненты
    private Rigidbody2D RB;
    [HideInInspector] public Animator _animator;
    private Transform _mainGroundCheck;
    private Transform _forwardGroundCheck;
    private Transform _wallCheck;
    private EnemyHP _enemyHP;
    [HideInInspector] public Collider2D collider2D;
    [HideInInspector] public Transform[] childObjects;


    // не редактируемые
    private float _timer;
    private int _direction;
    private bool _isFacingRight;
    private bool _runFinished;
    #endregion

    #region UNITY METHODS

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        if (_useTimer) ChangeDirection();
        if (_isAgressvie) CheckPlayer();
        ShowRays();
    }

    private void FixedUpdate()
    {
        if (_isAgressvie) AgressiveMove();
        else NormalMove();
    }

    #endregion

    #region CUSTOM METHODS

    private void Initialize()
    {
        // компоненты
        RB = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _enemyHP = GetComponent<EnemyHP>();
        collider2D = GetComponent<Collider2D>();
        childObjects = GetComponentsInChildren<Transform>();
        _mainGroundCheck = transform.Find("MainGroundCheck").transform;
        _forwardGroundCheck = transform.Find("ForwardGroundCheck").transform;
        _wallCheck = transform.Find("WallCheck").transform;

        // переменные
        _isFacingRight = true;
        _timer = _changeDirTime;
        _isTakingHit = false;
        _direction = 1;
        _isGroundedMain = false;
        _isGroundedForward = false;
        _runFinished = true;
        _seePlayer = false;

        // вызов методов
        InvokeRepeating(nameof(MainGroundCheck), 0f, 0.1f);
        InvokeRepeating(nameof(ForwardGroundCheck), 0f, 0.1f);
        InvokeRepeating(nameof(WallCheck), 0f, 0.1f);
        InvokeRepeating(nameof(CallFlip), 0f, 0.1f);
    }

    private void ChangeDirection()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            _direction = -_direction;
            _timer = _changeDirTime;
            FlipX();
        }

    }

    private void NormalMove()
    {
        if (_isTakingHit) return;

        if (_isGroundedMain)
        {
            RB.velocity = new Vector2(_walkSpeed * _direction, RB.velocity.y);
            _animator.SetBool("Run", true);
        }
        else
        {
            RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y);
            _animator.SetBool("Run", false);
        }
    }

    private void AgressiveMove()
    {
        if (_isTakingHit) return;

        if (_isGroundedMain && _runFinished)
        {
            RB.velocity = new Vector2(_walkSpeed * _direction, RB.velocity.y);
            _animator.SetBool("Walk", true);
            _animator.SetBool("Run", false);
        }
        else if (_isGroundedMain && !_runFinished)
        {
            RB.velocity = new Vector2(_attackSpeed * _direction, RB.velocity.y);
            _animator.SetBool("Run", true);
            _animator.SetBool("Walk", false);
        }
        else
        {
            RB.velocity = new Vector2(RB.velocity.x, RB.velocity.y);
            _animator.SetBool("Walk", false);
        }
    }

    private void CheckPlayer()
    {

    }

    private void CallFlip()
    {
        if (!_isGroundedForward || _isWallAhead) FlipX();
    }

    private void FlipX()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        if (!_useTimer) _direction = -_direction;
    }
    
    private void MainGroundCheck()
    {
        _isGroundedMain = Physics2D.OverlapCircle(_mainGroundCheck.position, _groundRayDist, _groundLayerMask);
    }

    private void ForwardGroundCheck()
    {
        _isGroundedForward = Physics2D.OverlapCircle(_forwardGroundCheck.position, _groundRayDist, _groundLayerMask);
    }

    private void WallCheck()
    {
        _isWallAhead = Physics2D.OverlapCircle(_wallCheck.position, _wallRayDist, _wallGroundLayerMask);
    }

    private void ShowRays()
    {
        if (_showRays)
        {
            Debug.DrawRay(_mainGroundCheck.position, Vector3.down * _groundRayDist, Color.yellow);

            Debug.DrawRay(_wallCheck.position, Vector3.right * _wallRayDist * transform.localScale.x, Color.blue);

            Debug.DrawRay(transform.position, Vector3.right * _attackRayDist * transform.localScale.x, Color.red);
        }
    }

    #endregion

    #region COURUTINES

    private IEnumerator RunToPlayer()
    {
        _runFinished = false;
        yield return new WaitForSeconds(_attackTime);
        _runFinished = true;
        FlipX();
    }

    public IEnumerator TakeHit(int damage)
    {
        _isTakingHit = true;
        _enemyHP.TakeDamage(damage);
        yield return new WaitForSeconds(_takingHitTime);
        _isTakingHit = false;
    }

    #endregion
}