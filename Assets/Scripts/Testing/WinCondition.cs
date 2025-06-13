using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinCondition : MonoBehaviour
{
    [SerializeField] string flagID;
    
    bool _wonGame = false;
    
    // Update is called once per frame
    void Update()
    {
        if (_wonGame || flagID.Length <= 0 || GameManager.FlagTracker == null) return;
        else if (GameManager.FlagTracker.CheckFlag(flagID)) 
        {
            _wonGame = true;
            Metrics.MarkGameCompleted();
            SaveSystem.ResetSaveData();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
    }
    
    // This should probably be a function in the game manager.
    public void ResetGame()
    {
        SaveSystem.ResetSaveData();
        Metrics.Reset();
        SceneManager.LoadScene(1);
    }
}