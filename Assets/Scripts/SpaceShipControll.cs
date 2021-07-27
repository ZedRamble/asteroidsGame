using System.Collections.Generic;
using UnityEngine;

public class SpaceShipControll : MonoBehaviour
{
    [Space] [Header("SpaceShip Settings")]
    
    [Range(0.01f, 10)]
    [SerializeField] private float spaceshipSpeed;
    [Range(0.01f, 10)]
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float angleForRotate;

    [Space] [Header("Bullet Settings")]
    
    [Range(0.01f, 10)]
    [SerializeField] private float bulletSpeed = 0.1f;
    [SerializeField] private int bulletsPerSecond = 3;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnTransform;

    [Space] [Header("Invulnerability Settings")]
    [Header("Invulnerability Settings")] [Space]
    [Tooltip("Frequency per second")]
    [SerializeField] private int flickerFrequency;
    [SerializeField] private float invulnerabilityTime;
    [SerializeField] private int numberOfLives;

    private bool _invulnerabilityCheck;
    
    private Rigidbody2D _rb;
    private AudioSource _audioSource;
    
    private float _timeCountBullet;
    private float _timeCountFlickering;
    private float _timeCountInvulnerabilityt;

    [HideInInspector]
    public GameControll gameControll;
  
    
    private List<GameObject> _bulletPool;
    private int _indexPool;
    private int _initBulletsCoef;
    
    private bool _flick;

    private Vector2 _velocitySave;
    private bool _afterPauseCheck;
    
    [HideInInspector]
    public GameObject bulletParent;
    
    private void Awake()
    {
        _invulnerabilityCheck = true;
        gameControll.livesCount = numberOfLives;
        bulletParent = new GameObject();
        bulletParent.name = "spaceship Bullet Pool";

        _flick = false;
        _timeCountBullet = 0;
        _timeCountInvulnerabilityt = 0;
        _timeCountFlickering = 0;
        _indexPool = 0;
        _initBulletsCoef = 5;
        
        _rb = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _bulletPool = InitPooler.PoolInitFunc(bulletPrefab, bulletParent.transform, bulletsPerSecond * _initBulletsCoef, gameControll, true);
    }

    
    private void Update()
    {
        if (gameControll.startButton && !gameControll.levelEnd)
        {
            PauseControll();
        }
        else
        {
            _afterPauseCheck = true;
            _rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void PauseControll()
    {
        _timeCountBullet += Time.deltaTime;
        if (_invulnerabilityCheck)
        {
            _timeCountInvulnerabilityt += Time.deltaTime;
            _timeCountFlickering += Time.deltaTime;
            InvulnerabilityFunc();
        }

        ChangeWallSide.ChangeWall(transform, gameControll.worldPosMax, gameControll.worldPosMin);
        AmounationBullet();
        _rb.constraints = RigidbodyConstraints2D.None;
            
        if (_afterPauseCheck)
        {
            _rb.velocity = _velocitySave;
            _afterPauseCheck = false;
        }
        else
            _velocitySave = _rb.velocity;
    } 
    
    
    private void FixedUpdate()
    {
        if (!gameControll.levelEnd)
        {
            RotateMovement();
            SpashipMovementVelocity();
        }
    }

    private void InvulnerabilityFunc()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (_timeCountInvulnerabilityt < invulnerabilityTime)
        {
            if (_timeCountFlickering > (1f / flickerFrequency))
            {
                int a = _flick ? 1 : 0;
                Color color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, a);
                spriteRenderer.color = color;
                _timeCountFlickering = 0;
                _flick = !_flick;
            }
        }
        else
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
            _timeCountFlickering = 0;
            _invulnerabilityCheck = false;
            _timeCountInvulnerabilityt = 0;
        }
    }
    
    private void RotateMovement()
    {
        if (Input.GetKey(KeyCode.D))
        {
            _rb.MoveRotation(_rb.rotation - angleForRotate + rotateSpeed * Time.fixedDeltaTime);
        } 
        else if (Input.GetKey(KeyCode.A))
        {
            _rb.MoveRotation(_rb.rotation + angleForRotate + rotateSpeed * Time.fixedDeltaTime);
        }
    }
    
    private void SpashipMovementVelocity()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (!_audioSource.isPlaying)
                _audioSource.Play();
            _rb.velocity = transform.up * spaceshipSpeed;
        }
    }
    
    private void AmounationBullet()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_timeCountBullet >= (1f / bulletsPerSecond))
            {
                float angleBullet = Mathf.Atan2(transform.up.y, transform.up.x) * Mathf.Rad2Deg;
                _bulletPool[_indexPool].transform.position = bulletSpawnTransform.position;
                _bulletPool[_indexPool].transform.localRotation = Quaternion.AngleAxis(angleBullet, Vector3.forward);
                _bulletPool[_indexPool].SetActive(true);
                _bulletPool[_indexPool].GetComponent<BulletControll>().GiveVelocityToBullet(transform.up * bulletSpeed);
                _timeCountBullet = 0;
                _indexPool++;
                if (_indexPool >= ((bulletsPerSecond * _initBulletsCoef)-1))
                    _indexPool = 0;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if ((other.gameObject.layer == 10 || other.gameObject.layer == 6) && !_invulnerabilityCheck)
        {
            if (gameControll.livesCount == 0)
            {
                gameControll.levelEnd = true;
                this.gameObject.SetActive(false);
            }
            else
            {
                gameControll.livesCount--;
                gameControll.lifeIcons[gameControll.livesCount].SetActive(false);
                _invulnerabilityCheck = true;
            }
        }
    }

    
    
}

