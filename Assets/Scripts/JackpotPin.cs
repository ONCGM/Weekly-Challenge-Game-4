using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class JackpotPin : MonoBehaviour {
    
    [Header("Settings")]
    [SerializeField, Range(1, 75)] private int ballsForJackpot = 20;
    [SerializeField] public bool isBonus;
    [SerializeField] private bool canEnableBonus;
    [SerializeField, Range(0f, 50f)] private float retractDistance = 2f;
    [SerializeField, Range(0f, 5f)] private float boardZPosition = 1.2f;
    [SerializeField, Range(0.05f, 2f)] private float animationTime = 0.5f;

    private GameController controller;
    private Collider trigger;
    private AudioSource aSource;

    // Setup.
    private void Awake() {
        controller = FindObjectOfType<GameController>();
        trigger = GetComponent<Collider>();
        aSource = GetComponent<AudioSource>();

        if(!isBonus) return;
        trigger.enabled = false;
        transform.DOLocalMoveZ(transform.localPosition.z - retractDistance, animationTime);
    }

    /// <summary>
    /// Enables bonus jackpots.
    /// </summary>
    public void BonusTime() {
        if(!isBonus) return;
        
        trigger.enabled = true;
        transform.DOLocalMoveZ(boardZPosition, animationTime);
    }

    /// <summary>
    /// Ends the bonus time.
    /// </summary>
    public void EndBonus() {
        if(!isBonus) return;
        
        trigger.enabled = false;
        transform.DOLocalMoveZ(transform.localPosition.z - retractDistance, animationTime);        
    }
    
    // Collision Detection.
    private void OnTriggerEnter(Collider other) {
        if(!other.GetComponent<Ball>()) return;

        controller.RemoveBall(other.GetComponent<Ball>());
        controller.AddScore(ballsForJackpot);
        controller.AddBalls(ballsForJackpot);
        
        if(!canEnableBonus) return;
        
        controller.JackpotBonusTime();
        aSource.Play();
    }
}
