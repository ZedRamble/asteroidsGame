using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UfoController : MonoBehaviour
{

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform gunUfo;
    
    [SerializeField] private float bulletSpeed;
    private float _bulletsPerSecond;


    [HideInInspector]
    public GameControll gameControll;
    [HideInInspector]
    public GameObject bulletParent;

    private Rigidbody2D _rigidbody2D;
    private List<GameObject> _bulletPool;
    private int _indexPool;
    private float _timeCount;

    private float _maxTimeBullet;
    private float _coefForInit;
    
    private Vector2 _velocitySave;
    private bool _afterPauseCheck;
    private void Awake()
    {
        _indexPool = 0;
        _coefForInit = 7;
        _maxTimeBullet = 0.5f;
        _bulletsPerSecond = Random.Range(0.2f, 0.5f);
        bulletParent = new GameObject();
        bulletParent.name = "Ufo Bullet Pool";
        _bulletPool = InitPooler.PoolInitFunc(bulletPrefab, bulletParent.transform, (int)(_maxTimeBullet * _coefForInit), gameControll, true);
        
        _rigidbody2D = GetComponent<Rigidbody2D>();

    }

    private void OnDisable()
    {
        gameControll.ufoActive = false;
    }

    private void Update()
    {
        if (gameControll.startButton && !gameControll.levelEnd)
        {
            _bulletsPerSecond = Random.Range(0.2f, 0.5f);
            _timeCount += Time.deltaTime;
            ChangeWallSide.ChangeWall(transform, gameControll.worldPosMax, gameControll.worldPosMin);
            AmountUfo();
            
            _rigidbody2D.constraints = RigidbodyConstraints2D.None;
            if (_afterPauseCheck)
            {
                _rigidbody2D.velocity = _velocitySave;
                _afterPauseCheck = false;
            }
            else
                _velocitySave = _rigidbody2D.velocity;
        }
        else
        {
            _afterPauseCheck = true;
            _rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        
       
    }
    

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 9 || other.gameObject.layer == 6)
        {
            gameControll.scoreCount += 200;
            this.gameObject.SetActive(false);
        }
    }
    
    private void AmountUfo()
    {
        if (_timeCount >= (1f / _bulletsPerSecond))
        {
            Vector2 dir = gameControll.spaceshipInited.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            _bulletPool[_indexPool].transform.position = gunUfo.position;
            _bulletPool[_indexPool].transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);
            _bulletPool[_indexPool].SetActive(true);
            _bulletPool[_indexPool].GetComponent<Rigidbody2D>().velocity = dir.normalized * bulletSpeed;
            _timeCount = 0;
            _indexPool++;
            if (_indexPool >= (int)(_maxTimeBullet * _coefForInit))
                _indexPool = 0;
        }
    }
}
