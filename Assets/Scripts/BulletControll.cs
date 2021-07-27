using System;
using UnityEngine;

public class BulletControll : MonoBehaviour
{
    [SerializeField] private bool checkTypeOfBullet;
    private Rigidbody2D _rigidbody2D;
    
    [HideInInspector]
    public GameControll gameControll;


    private AudioSource _audioSource;
    private Vector2 _velocitySave;
    private bool _afterPauseCheck;
    private Vector2 _distance;
    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>(); 
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }


    private void OnEnable()
    {
        _audioSource.Play();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (checkTypeOfBullet)
        {
            if (other.gameObject.layer == 6
                || other.gameObject.layer == 8)
            {
                this.gameObject.SetActive(false);
            }
        }
        else
        {
            if (other.gameObject.layer == 6
                || other.gameObject.layer == 7)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
    
    private void Update()
    {
        if (gameControll.startButton && !gameControll.levelEnd)
        {
            ChangeWallSide.ChangeWall(transform, gameControll.worldPosMax, gameControll.worldPosMin);
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

    private void FixedUpdate()
    {
        _distance += _rigidbody2D.velocity * Time.fixedDeltaTime;
        if (_distance.sqrMagnitude > (gameControll.worldPosMax.x * gameControll.worldPosMax.x))
        {
            gameObject.SetActive(false);
            _distance = Vector2.zero;
        }
    }
    

    public void GiveVelocityToBullet(Vector2 dir)
    {
        _rigidbody2D.velocity = dir;
    }
}
