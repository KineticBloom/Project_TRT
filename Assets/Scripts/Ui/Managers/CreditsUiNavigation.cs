using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsUiNavigation : MonoBehaviour {
    public void Update() {

        if (GameManager.PlayerInput.GetMenu1Down() || GameManager.PlayerInput.GetStartDown()) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
}
