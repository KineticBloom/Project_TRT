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
        if (_wonGame || GameManager.FlagTracker == null || flagID.Length <= 0) return;
        else if (GameManager.FlagTracker.CheckFlag(flagID)) 
        {
            _wonGame = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
    }
    
    // This should probably be a function in the game manager.
    public void ResetGame()
    {
        if (GameManager.Inventory != null) {
            GameManager.Inventory.Clear();
        }
        SceneManager.LoadScene(0);
    }
}