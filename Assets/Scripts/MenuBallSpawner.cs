using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class MenuBallSpawner : MonoBehaviour {
    [Header("Settings")]
    [SerializeField, Range(50, 500)] private int ballLimit = 250;
    [SerializeField, Range(0.01f, 5f)] private float spawnFrequency = 0.3f;
    [SerializeField, Range(0.5f, 50f)] private float maxImpulseForce = 15f;
    [SerializeField, Range(-10f, -500f)] private float distanceToTeleportBalls = -200f;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject ballPrefab;

    private List<Ball> spawnedBalls = new List<Ball>();
    private WaitForSecondsRealtime waitSpawnDelay;
    

    // Setup.
    private void Awake() {
        waitSpawnDelay = new WaitForSecondsRealtime(spawnFrequency);
    }

    private void Start() {
        StartCoroutine(SpawnBalls(ballLimit));
    }

    private void FixedUpdate() {
        foreach(var ball in spawnedBalls.Where(ball => ball.transform.position.y < distanceToTeleportBalls)) {
            ball.transform.position = transform.position;
            var rb = ball.GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.AddForce(Random.onUnitSphere * maxImpulseForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Spawns any amount of ball with a small delay between them.
    /// </summary>
    private IEnumerator SpawnBalls(int amount) {
        for(var i = 0; i < amount; i++) {
            var ball = Instantiate(ballPrefab, transform.position, Quaternion.identity, transform);
            ball.GetComponent<Rigidbody>().AddForce(Random.onUnitSphere * maxImpulseForce, ForceMode.Impulse);
            spawnedBalls.Add(ball.GetComponent<Ball>());
            yield return waitSpawnDelay;
        }
    }
}
