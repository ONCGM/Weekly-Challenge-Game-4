using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
    [Header("Settings")]
    [SerializeField, Range(15, 150)] private int startBallAmount = 50;
    [SerializeField, Range(150, 2500)] private int ballLimit = 700;
    [SerializeField, Range(0.01f, 1f)] private float spawnDelay = 0.15f;
    [SerializeField, Range(0.1f, 2f)] private float maxHoldTime = 1f;
    [SerializeField, Range(1f, 125f)] private float maxBallImpulse = 25f;
    [SerializeField, Range(5, 60)] private int bonusTimeOnJackpot = 20;
    [SerializeField, Range(2, 12)] private int bonusTimeCooldown = 5;

    [Header("Scoring")]
    [SerializeField, Range(1, 15)] private int scorePerBall = 5;
    [SerializeField] private int playerScore;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject ballPrefab;
    
    [Header("Positions")] 
    [SerializeField] private Transform ballParent;
    [SerializeField] private Transform holderSpawnPoint;
    [SerializeField] private Transform gameSpawnPoint;

    [Header("Components")] 
    [SerializeField] private TMP_Text scoreText;

    [Header("Scene Settings")]
    [SerializeField, Range(0, 2)] private int mainMenuSceneIndex = 0;

    private readonly List<Ball> spawnedBalls = new List<Ball>();
    private readonly List<Ball> inGameBalls = new List<Ball>();
    private WaitForSecondsRealtime waitSpawnDelay;

    private float keyDownTime;
    private float bonusTime;
    private float nextBonusTime;
    private bool bonusTimeActive;
    private readonly List<JackpotPin> jackpotPins = new List<JackpotPin>();

    // Setup.
    private void Awake() {
        waitSpawnDelay = new WaitForSecondsRealtime(spawnDelay);

        foreach(var pin in FindObjectsOfType<JackpotPin>())
            jackpotPins.Add(pin);
    }

    // Starting balls.
    private void Start() {
        StartCoroutine(SpawnBalls(startBallAmount, holderSpawnPoint.position));
    }
    
    #region Jackpot

    /// <summary>
    /// Opens up the bonus jackpot pins.
    /// </summary>
    public void JackpotBonusTime() {
        if(Time.time < nextBonusTime) return;
        if(bonusTimeActive) return;
        
        bonusTimeActive = true;
        bonusTime = bonusTimeOnJackpot;
        foreach(var jackpotPin in jackpotPins) {
            jackpotPin.BonusTime();
        }
    }
    
    #endregion
    
    #region Input

    // Input detection.
    private void Update() {
        if(bonusTimeActive) {
            if(bonusTime > 0) bonusTime -= Time.deltaTime;
            else {
                bonusTimeActive = false;
                nextBonusTime = Time.time + bonusTimeCooldown;
                foreach(var jackpotPin in jackpotPins) {
                    jackpotPin.EndBonus();
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Space)) keyDownTime = Time.time;
        
        if(!Input.GetKeyUp(KeyCode.Space)) return;
        var impulseForce = Mathf.Lerp(0.5f, maxBallImpulse, Mathf.InverseLerp(0f, maxHoldTime, Time.time - keyDownTime));
        
        if(spawnedBalls.Count < 1) return;
        var ball = spawnedBalls[0];
        spawnedBalls.Remove(ball);
        spawnedBalls.TrimExcess();
        inGameBalls.Add(ball);
        
        ball.transform.position = gameSpawnPoint.position;
        ball.GetComponent<Rigidbody>().AddForce(Vector3.left * impulseForce, ForceMode.Impulse);
    }

    #endregion

    #region Ball Spawn

    /// <summary>
    /// Adds new ball on jackpot or other events.
    /// </summary>
    public void AddBalls(int balls) {
        StartCoroutine(SpawnBalls(balls, holderSpawnPoint.position));
    }
    
    /// <summary>
    /// Spawns any amount of ball with a small delay between them.
    /// </summary>
    private IEnumerator SpawnBalls(int amount, Vector3 position) {
        for(var i = 0; i < amount; i++) {
            if(spawnedBalls.Count + inGameBalls.Count >= ballLimit) {
                AddScore(Math.Max(amount - i, 1));
                yield break;
            }
            
            var ball = SpawnBall(position, ballParent);
            ball.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere, ForceMode.Impulse);
            spawnedBalls.Add(ball.GetComponent<Ball>());
            UpdateScore();
            yield return waitSpawnDelay;
        }
    }

    /// <summary>
    /// Spawns a ball and returns it.
    /// </summary>
    private GameObject SpawnBall(Vector3 position, Transform parent = null) {
        return parent ? Instantiate(ballPrefab, position, quaternion.identity, parent) : 
            Instantiate(ballPrefab, position, quaternion.identity);
    }

    /// <summary>
    /// Removes a ball.
    /// </summary>
    public void RemoveBall(Ball ball) {
        if(!inGameBalls.Contains(ball)) return;
        
        inGameBalls.Remove(ball);
        inGameBalls.TrimExcess();
        Destroy(ball.gameObject);
        UpdateScore();
    }
    #endregion
    
    #region Score & UI

    /// <summary>
    /// Adds score on the counter.
    /// </summary>
    public void AddScore(int balls) {
        playerScore += balls * scorePerBall;
        UpdateScore();
    }

    /// <summary>
    /// Updates the score text.
    /// </summary>
    private void UpdateScore() {
        scoreText.text = $"High Score: {PlayerPrefs.GetInt("HighScore")}\nScore: {playerScore}\nBalls: {spawnedBalls.Count + inGameBalls.Count} / {ballLimit}";

        if(playerScore > PlayerPrefs.GetInt("HighScore")) {
            PlayerPrefs.SetInt("HighScore", playerScore);
            PlayerPrefs.Save();
        }

        if(spawnedBalls.Count + inGameBalls.Count < 1) SceneManager.LoadSceneAsync(mainMenuSceneIndex);
    }

    #endregion
}
