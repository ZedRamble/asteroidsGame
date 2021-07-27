using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using TMPro;
using UnityEngine.UI;

public class GameControll : MonoBehaviour
{
    [Space] [Header("Prefabs for initizialization")]
    [SerializeField] private GameObject largeAsteroid;
    [SerializeField] private GameObject spaceshipPrefab;
    [SerializeField] private GameObject ufoPrefab;
    
    [Space] [Header("UI Settings")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text gameOverText;
    [SerializeField] private TMP_InputField levelInputText;
    [SerializeField] private Button resumeButton;
    public GameObject[] lifeIcons;
    
    [Space] [Header("Ufo Settings")]
    [SerializeField] private float ufoSpeed;
    
    // Game settings
    private int _levelOfGame;
    private int _levelCount;
    private SpaceShipControll _spaceShipControll;
    private int _maxNumbOfLives;
    
    //asteroid pool
    private List<GameObject> _astroPool;
    private int _indexPool;
    private int _astroInited;
    private GameObject _astroParent;
    
    //ufo settings
    private UfoController _ufoController;
    private Rigidbody2D _ufoRigidbody2D;
    private GameObject _ufoInited;
    
    // time counters
    private float _timeCount;
    private float _countTime;
    private float _timeUfo;
    
    // checkers
    private bool _doneText;
    private bool _initCheck;

    //world boundaries
    [HideInInspector] public Vector2 worldPosMax = new Vector2(9, 5);
    [HideInInspector] public Vector2 worldPosMin = new Vector2(-9, -5);
    
    [HideInInspector] public GameObject spaceshipInited;
    [HideInInspector] public AudioSource audioSource;

    [HideInInspector] public bool ufoActive;
    [HideInInspector] public bool levelEnd;
    [HideInInspector] public bool startButton;
    [HideInInspector] public bool newGameButton;
    
    [HideInInspector] public int livesCount;
    [HideInInspector] public int scoreCount;
    [HideInInspector] public int astroCount;

    private void Awake()
    {
        NewGame();
    }

    private void Update()
    {
        if (newGameButton)
            NewGame();
        if (startButton)
        {
            resumeButton.interactable = true;
            if (_initCheck)
            {
                InitInStart();
                _initCheck = false;
            }
            LevelController();
            TextLerpFunc();
        }
    }
    public void NewGame()
    {
        if (spaceshipInited)
        {
            Destroy(_spaceShipControll.bulletParent);
            Destroy(spaceshipInited);
        }
        if (_ufoInited)
        {
            Destroy(_ufoController.bulletParent);
            Destroy(_ufoInited);
        }
        if (_astroParent)
            Destroy(_astroParent);
        if (_levelCount < _maxNumbOfLives)
        {
            foreach (var gm in lifeIcons)
                gm.SetActive(true);
        }
        if (scoreCount > 0)
            scoreCount = 0;

        _timeUfo = 0;
        astroCount = 0;
        _levelCount = 2;
        _astroInited = 0;
        
        levelEnd = false;
        ufoActive = false;
        _initCheck = true;
        newGameButton = false;
        _doneText = false;
        _countTime = 0;
    }
    
    private void InitInStart()
    {
        
        _levelOfGame = Convert.ToInt32(levelInputText.text);
        _astroParent = new GameObject();
        _astroParent.name = "AsteroidsPool";
        _astroPool = InitPooler.PoolInitFunc(largeAsteroid, _astroParent.transform, _levelOfGame+1, GetComponent<GameControll>(), true);
        
        
        spaceshipPrefab.GetComponent<SpaceShipControll>().gameControll = GetComponent<GameControll>();
        spaceshipInited = Instantiate(spaceshipPrefab, Vector3.zero, Quaternion.identity);
        _spaceShipControll = spaceshipInited.GetComponent<SpaceShipControll>();
        _maxNumbOfLives = livesCount;
        
        ufoPrefab.GetComponent<UfoController>().gameControll = GetComponent<GameControll>();
        _ufoInited = Instantiate(ufoPrefab, Vector3.zero, Quaternion.identity);
        _ufoController = _ufoInited.GetComponent<UfoController>();
        _ufoRigidbody2D = _ufoInited.GetComponent<Rigidbody2D>();

        audioSource = GetComponent<AudioSource>();
        gameOverText.text = "1 LEVEL";
        _doneText = true;
    }

  

    
    private void TextLerpFunc()
    {
        if (_doneText)
        {
            Color32 startColor = new Color32(255, 255, 255, 255); 
            Color32 endColor = new Color32(255, 255, 255, 0); 
            if (_countTime <= 10)
                gameOverText.color = Color32.Lerp(startColor,endColor, _countTime);           
            _countTime += Time.deltaTime/2.3f;
        }
    }

    private void LevelController()
    {
        if (!levelEnd)
        {
            scoreText.text = $"Score: {scoreCount}";
            _timeUfo += Time.deltaTime;
            float timeToSpawn = Random.Range(20, 40);
            if (_timeUfo > timeToSpawn && !ufoActive)
            {
                UfoSpawn();
            }
            if  (_levelCount > _astroInited)
            {
                LevelConstuctor();
            }
            else if (astroCount == 0 && _levelCount < _levelOfGame)
            {
                gameOverText.text = $"{_levelCount} LEVEL";
                _timeCount += Time.deltaTime;
                if (_timeCount >= 2)
                {
                    _countTime = 0;
                    _levelCount++;
                    _astroInited = 0;
                    _timeCount = 0;
                }
            }
        }
        else
        {
            resumeButton.interactable = false;
            gameOverText.text = "Game Over";
            _countTime = 0;
        }
    }

    private void LevelConstuctor()
    {
        _astroPool[_indexPool].SetActive(true);
        Transform astroLarge = _astroPool[_indexPool].transform.GetChild(0);
        Vector2 moveDir = spaceshipInited.transform.position;
        
        int side = Random.Range(0, 2);
        float minDistanceToSpaceShip = 1.5f;
        int coefForMulty = side == 1 ? -1 : 1;
        
        Vector2 spawnPos = new Vector2(Random.Range(worldPosMin.x, 0), Random.Range(worldPosMin.y, worldPosMax.y));
        Vector2 spawnPosCur = new Vector2( spawnPos.x * (coefForMulty) + moveDir.x * (coefForMulty) + minDistanceToSpaceShip * (coefForMulty),
            spawnPos.y * (coefForMulty) + moveDir.y * (coefForMulty)  + minDistanceToSpaceShip * (coefForMulty));
        Vector2 currDir = new Vector2(moveDir.x - spawnPosCur.x, moveDir.y - spawnPosCur.y);

        float astroSpeed = Random.Range(0.05f, 0.1f);

        astroLarge.position = spawnPosCur;
        astroLarge.gameObject.SetActive(true);
        astroLarge.GetComponent<AsteroidController>().GiveVelocity(currDir, astroSpeed);
        
        _indexPool++;
        if (_indexPool == _levelOfGame)
            _indexPool = 0;
        _astroInited++;
    }

    private void UfoSpawn()
    {
        Vector2 spawnPos;
        int side = Random.Range(0, 2);
        float percent = HelpMathFunc.GetPercent(worldPosMax.y, 20);
        
        float currCoordinate = side == 0 ? worldPosMin.x :worldPosMax.x;
        spawnPos = new Vector2(currCoordinate, Random.Range(worldPosMin.y + percent , worldPosMax.y - percent));
        
        _ufoInited.transform.position = spawnPos;
        _ufoInited.SetActive(true);
        _ufoRigidbody2D.velocity = new Vector2(-spawnPos.x, spawnPos.y) * ufoSpeed;
        ufoActive = true;
        _timeUfo = 0;
    }

    
   
}
