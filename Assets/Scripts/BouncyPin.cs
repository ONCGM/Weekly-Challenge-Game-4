using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyPin : MonoBehaviour {
    [Header("Settings")]
    [SerializeField, Range(1f, 10f)] private float bounceMultiplier = 3f;
    
    private void OnCollisionEnter(Collision other) {
        if(!other.gameObject.GetComponent<Ball>()) return;
        var rb = other.gameObject.GetComponent<Rigidbody>();
        
        if(!rb) return;
        rb.AddForce(Vector3.Reflect(rb.velocity.normalized, other.GetContact(0).normal) * bounceMultiplier, ForceMode.Impulse);
    }
}
