using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidController : MonoBehaviour
{
    [SerializeField] private bool checkForBigAsteroid;
    [SerializeField] private bool checkForMidleAsteroid;
    [SerializeField] private GameObject[] midleAsteroidForInit;
    [SerializeField] private GameObject[] smallAsteroidForInit;
    [SerializeField] private AudioClip audioClip;
    
    private Rigidbody2D _rigidbody2D;
    private AudioSource _audioSource;
    private Vector2 _velocitySave;
    private bool _afterPauseCheck;
    private bool _collisionEnter;
    
    [HideInInspector]
    public GameControll gameControll;
        
    private void Awake()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _velocitySave = _rigidbody2D.velocity;
        _afterPauseCheck = false;
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 10 || other.gameObject.layer == 9 || other.gameObject.layer == 7 || other.gameObject.layer == 8)
        {
            gameControll.audioSource.clip = audioClip;
            gameControll.audioSource.Play();
            AsteroidStats(other.gameObject.layer);
        }
    }
    
    private void AsteroidStats(int layer)
    {
        gameObject.SetActive(false);
        gameControll.astroCount--;
        if (layer != 10)
            ScoreCountAstro();
        if (checkForBigAsteroid && layer != 7 && layer != 8)
        {
            InitAstro(midleAsteroidForInit);
        }
        else if (checkForMidleAsteroid && layer != 7 && layer != 8)
        {
            InitAstro(smallAsteroidForInit);
        } 
    }
    
    private void ScoreCountAstro()
    {
        if (checkForBigAsteroid)
            gameControll.scoreCount += 20;
        else if (checkForMidleAsteroid)
            gameControll.scoreCount += 50;
        else
            gameControll.scoreCount += 100;
    }
    private void InitAstro(GameObject[] astroInit)
    {
        astroInit[0].transform.position = transform.position;
        astroInit[1].transform.position = transform.position;

        Vector2 asteroidDir = -(transform.position - Camera.main.transform.position).normalized;
        
        float angl = Mathf.Sqrt(2)/2;
        float rx = asteroidDir.x * angl - asteroidDir.y * angl;
        float ry = asteroidDir.x * angl + asteroidDir.y * angl;
        
        Vector2 dirWithAngle = new Vector2(rx, ry).normalized;
        Vector2 sm1Dir = asteroidDir - dirWithAngle;
        Vector2 sm2Dir = asteroidDir + dirWithAngle;
        
        astroInit[0].SetActive(true);
        astroInit[1].SetActive(true);
        
        float asteroidSpeed = Random.Range(0.5f, 1.5f);
        astroInit[0].GetComponent<AsteroidController>().GiveVelocity(sm1Dir, asteroidSpeed);
        astroInit[1].GetComponent<AsteroidController>().GiveVelocity(sm2Dir, asteroidSpeed);
    }

    public void GiveVelocity(Vector2 dir, float speed)
    {
        _rigidbody2D.velocity = dir * speed;
        gameControll.astroCount++;
    }
    
}
