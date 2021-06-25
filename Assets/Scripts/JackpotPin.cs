using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class JackpotPin : MonoBehaviour {
    
    [Header("Settings")]
    [SerializeField, Range(1, 75)] private int ballsForJackpot = 20;

    private GameController controller;

    // Setup.
    private void Awake() {
        controller = FindObjectOfType<GameController>();
    }

    
    // Collision Detection.
    private void OnTriggerEnter(Collider other) {
        if(!other.GetComponent<Ball>()) return;
        
        controller.RemoveBall(other.GetComponent<Ball>());
        controller.AddScore(ballsForJackpot);
        controller.AddBalls(ballsForJackpot);
    }
}
