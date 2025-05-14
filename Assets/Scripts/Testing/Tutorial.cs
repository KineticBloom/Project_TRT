using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private bool popUp = true;
    [SerializeField, HideIf("popUp")] private string tutorialFlag = "tutorial_bartered";
    [SerializeField, HideIf("popUp")] private FadeToBlack fadeToBlack;
    [SerializeField, HideIf("popUp")] private GameObject inputHints;
    [SerializeField, HideIf("popUp")] private TextAsset tutorialText;
    [SerializeField, HideIf("popUp")] private NpcInteractable tutorialNPC;
    [SerializeField, HideIf("popUp")] private WorldCameraController startCamera;
    [SerializeField, HideIf("popUp")] private WorldCameraController stopCamera;
    [SerializeField, HideIf("popUp")] private float playerSpeed = 8;
    [SerializeField, HideIf("popUp")] private float moveBackTime = 1;
    [SerializeField, HideIf("popUp")] private float moveForwardTime = 1;
    
    private bool _popUpClosed = false;
    
    private void Awake()
    {
        if (TimeLoopManager.Instance == null) {
            GameManager.Instance.FindPlayer();
            GameManager.Instance.FindMasterCanvas();
            GameManager.Inventory.Clear();
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartWait());
    }
    
    IEnumerator StartWait()
    {
        yield return null;
        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(true);
        }
        else GameManager.Player.Movement.SetCanMove(false);
        Time.timeScale = 0;
        if (inputHints != null) inputHints.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.PlayerInput == null) return;
        else if (!_popUpClosed && GameManager.PlayerInput.GetAffirmDown()) CloseTutorial();
        // else if (!popUp && GameManager.FlagTracker.CheckFlag(tutorialFlag)) Destroy(gameObject);
    }
    
    void OnDisable()
    {
        if (GameManager.DialogueManager.EndCallback != null) GameManager.DialogueManager.EndCallback -= EndDialogue;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (GameManager.FlagTracker.CheckFlag(tutorialFlag)) ContinueOn();
            else StopPlayer();
        }
    }
    
    void CloseTutorial()
    {
        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(false);
        }
        else GameManager.Player.Movement.SetCanMove(true);
        
        Time.timeScale = 1;
        _popUpClosed = true;
        if (popUp) Destroy(gameObject);
        else inputHints.SetActive(false);
    }
    
    void StopPlayer()
    {
        stopCamera.Activate();
        Vector3 NPCWorldPosition = tutorialNPC.transform.position + tutorialNPC.DialogueSourceLocalPosition;
        Vector3 PlayerWorldPosition = GameManager.Player.DialogueSource.position;
        GameManager.DialogueManager.StartDialogue(tutorialText, tutorialNPC.TriggerBarter, NPCWorldPosition, PlayerWorldPosition, "Wait_Up");
        GameManager.DialogueManager.EndCallback += EndDialogue;
    }
    
    void EndDialogue()
    {
        startCamera.Activate();
        GameManager.DialogueManager.EndCallback -= EndDialogue;
        StartCoroutine(MovePlayerBack());
    }
    
    IEnumerator MovePlayerBack()
    {
        GameManager.PlayerInput.ToggleControls();
        GameManager.Player.Movement.ForceMove(true, Vector3.back);
        yield return new WaitForSecondsRealtime(moveBackTime);
        GameManager.Player.Movement.ForceMove(false, Vector3.zero);
        GameManager.PlayerInput.ToggleControls(true);
    }
    
    IEnumerator MovePlayerForward()
    {
        Metrics.MarkTutorialCompleted();
        fadeToBlack.StartFadeIn(moveForwardTime);
        GameManager.PlayerInput.ToggleControls();
        GameManager.Player.Movement.ForceMove(true, Vector3.forward);
        yield return new WaitForSecondsRealtime(moveForwardTime);
        GameManager.Player.Movement.ForceMove(false, Vector3.zero);
        GameManager.PlayerInput.ToggleControls(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    void ContinueOn()
    {
        StartCoroutine(MovePlayerForward());
    }
}