using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreMenu : MonoBehaviour {
    // Update text.
    private void Awake() {
        GetComponent<TMP_Text>().text = $"High Score: {PlayerPrefs.GetInt("HighScore")}";
    }
}
