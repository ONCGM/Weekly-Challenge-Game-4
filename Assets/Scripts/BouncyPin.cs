using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyPin : MonoBehaviour {
    [Header("Settings")]
    [SerializeField, Range(1f, 10f)] private float bounceMultiplier = 3f;

    private AudioSource aSource;

    // Setup.
    private void Awake() => aSource = GetComponent<AudioSource>();
    
    private void OnCollisionEnter(Collision other) {
        if(!other.gameObject.GetComponent<Ball>()) return;
        var rb = other.gameObject.GetComponent<Rigidbody>();
        
        if(!rb) return;
        rb.AddForce(-other.GetContact(0).normal.normalized * bounceMultiplier, ForceMode.Impulse);
        aSource.Play();
    }
}
