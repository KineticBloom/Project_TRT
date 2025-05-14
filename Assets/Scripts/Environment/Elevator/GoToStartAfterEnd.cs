using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToStartAfterEnd : MonoBehaviour
{
    [SerializeField] private ElevatorController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller.finishedMoving += GoToStart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GoToStart()
    {
        controller.finishedMoving -= GoToStart;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
