using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour {
    [Header("Settings")]
    [SerializeField] private int gameSceneIndex = 1;
    
    /// <summary>
    /// Loads the game scene. Also works for a cheap restart.
    /// </summary>
    public void LoadGameScene() {
        SceneManager.LoadSceneAsync(gameSceneIndex);
    }
}
