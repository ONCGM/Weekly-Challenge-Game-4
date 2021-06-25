using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeSpawner : MonoBehaviour {
    private GameController controller;

    // Setup.
    private void Awake() {
        controller = FindObjectOfType<GameController>();
    }

    // Collision detection.
    private void OnCollisionEnter(Collision other) {
        if(!other.gameObject.GetComponent<Ball>()) return; 
        
        controller.RemoveBall(other.gameObject.GetComponent<Ball>());
    }
}
