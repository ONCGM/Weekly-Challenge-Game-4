using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {
    [Header("Settings")]
    [SerializeField, Range(15, 150)] private int startBallAmount = 50;
    [SerializeField, Range(1000, 5000)] private int ballLimitAmount = 2500;
    [SerializeField, Range(0.01f, 1f)] private float spawnDelay = 0.15f;

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

    private List<Ball> spawnedBalls = new List<Ball>();
    private WaitForSecondsRealtime waitSpawnDelay;

    // Setup.
    private void Awake() {
        waitSpawnDelay = new WaitForSecondsRealtime(spawnDelay);
    }

    // Starting balls.
    private void Start() {
        StartCoroutine(SpawnBalls(startBallAmount, holderSpawnPoint.position));
    }

    /// <summary>
    /// Spawns any amount of ball with a small delay between them.
    /// </summary>
    private IEnumerator SpawnBalls(int amount, Vector3 position) {
        for(var i = 0; i < amount; i++) {
            if(spawnedBalls.Count >= ballLimitAmount) {
                AddScore(Math.Max(amount - i, 1));
                yield break;
            }
            
            var ball = SpawnBall(position, ballParent);
            ball.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere, ForceMode.Impulse);
            spawnedBalls.Add(ball.GetComponent<Ball>());
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
    /// Adds score on the counter.
    /// </summary>
    private void AddScore(int balls) {
        playerScore += balls * scorePerBall;
        scoreText.text = $"Score: {playerScore} \n Balls: {spawnedBalls.Count} / {ballLimitAmount}";
    }
}
