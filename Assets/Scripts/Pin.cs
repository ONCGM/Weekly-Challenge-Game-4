using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Pin : MonoBehaviour {
    [Header("Clips")]
    [SerializeField] private List<AudioClip> aClips = new List<AudioClip>();
    
    [Header("Settings")]
    [SerializeField, Range(0f, 0.3f)] private float pitchRandomization = 0.2f;
    
    private AudioSource aSource;

    // Setup.
    private void Awake() => aSource = GetComponent<AudioSource>();

    private void OnCollisionEnter(Collision other) {
        if(!other.gameObject.GetComponent<Ball>()) return;
        
        aSource.clip = aClips[Random.Range(0, aClips.Count)];
        aSource.pitch = Random.Range(1f - pitchRandomization, 1f + pitchRandomization);
        
        if(other.rigidbody) {
            var speed = other.rigidbody.velocity.magnitude;
            print(speed);
            aSource.volume = Mathf.Max(Mathf.InverseLerp(0f, 15f, speed), 0.42f);
        }
        
        aSource.Play();
    }
}
