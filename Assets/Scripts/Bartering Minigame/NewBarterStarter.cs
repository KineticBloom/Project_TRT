using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBarterStarter : MonoBehaviour {

    #region ======== [ INTERNAL PROPERTIES ] ========
    private InGameUi _inGameUi;
    #endregion

    #region ======== [ INIT METHOD ] ========
    public void Start() {
        if (GameManager.MasterCanvas != null) {
            _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();
        }
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoadTrigger;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoadTrigger;
    }
    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Globally callable event to start a barter.
    /// </summary>
    /// <param name="npcData">Data to start the barter with.</param>
    public void StartBarter(NPCData npcData, NpcInteractable npcInstance) {

        if (_inGameUi == null) return;

        /// HERE MOVE TO ITEM SELECTION WITH NPC DATA THEN TO BARTERING!
        _inGameUi.MoveToChooseItem(npcData, npcInstance);
    }

    /// <summary>
    /// Get BarterController that will be used in bartering logic.
    /// </summary>
    /// <returns>BarteringController being used, if any.</returns>
    public BarteringController GetBarteringController() {

        if (_inGameUi == null) return null;

        return _inGameUi.BarteringController;
    }
    #endregion

    #region ======== [ PRIVATE METHODS ] ========
    void OnSceneLoadTrigger(Scene scene, LoadSceneMode mode) {
        StartCoroutine("OnSceneLoad");
    }

    IEnumerator OnSceneLoad() {
        yield return 0;
        if (_inGameUi == null && GameManager.MasterCanvas != null) {
            _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();
        }
    }
    #endregion
}
