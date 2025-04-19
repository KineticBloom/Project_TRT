using UnityEngine;
using NaughtyAttributes;

public class GameManager : Singleton<GameManager>
{
    // Public accessors ===========================================================================

    public static DialogueManager DialogueManager { get { return Instance.dialogueManager; } }
    public static PlayerInputHandler PlayerInput { get { return Instance.playerInput; } }
    public static Inventory Inventory { get { return Instance.inventory; } }
    public static Player Player { get { return Instance._player; } }
    public static Canvas MasterCanvas { get { return Instance._masterCanvas; } }
    public static FlagTracker FlagTracker { get { return Instance.flagTracker; } }
    public static NewBarterStarter NewBarterStarter { get { return Instance.newBarterStarter; } }

    // Backing fields =============================================================================

    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private PlayerInputHandler playerInput;
    [SerializeField] private Inventory inventory;
    [SerializeField, Tag] private string playerTag;
    [SerializeField, ReadOnly] private Player _player;
    [SerializeField, Tag] private string masterCanvasTag;
    [SerializeField, ReadOnly] private Canvas _masterCanvas;
    [SerializeField] private FlagTracker flagTracker;
    [SerializeField] private NewBarterStarter newBarterStarter;

    // Initializers ===============================================================================

    public void FindPlayer()
    {
        print("FindPlayer() called");

        GameObject playerObj = GameObject.FindWithTag(playerTag);

        if (playerObj != null) {
            GameObject playerParent = playerObj.transform.root.gameObject;

            if (playerParent != null) {
                _player = playerParent.GetComponentInChildren<Player>();
            }
        }
    }

    public void FindMasterCanvas()
    {
        print("FindMasterCanvas() called");

        GameObject masterCanvasObj = GameObject.FindWithTag(masterCanvasTag);

        if (masterCanvasObj != null) {
            _masterCanvas = masterCanvasObj.GetComponentInChildren<Canvas>();
        }
    }

    // Save and Load =============================================================================

    /// <summary>
    /// Saves the NPC Effect Cards
    /// </summary>
    /// <param name="npcSaveData">Save Data holding all of the effect cards</param>
    public void Save(ref NPCSaveData npcSaveData)
    {
        if (_player == null)
        {
            Debug.LogError("Cannot Save outside of game scene: Player not found.");
            return;
        }

        foreach (GameObject npcObject in GameObject.FindGameObjectsWithTag("NPC"))
        {
            
            if (!npcObject.TryGetComponent<NpcInteractable>(out var interactable))
            {
                Debug.LogError("Save: NPC " + npcObject.name + " does not have NPCInteractable component");
                continue;
            }

            interactable.Save(ref npcSaveData);
        }
    }

    /// <summary>
    /// Loads Effect Cards from save data
    /// </summary>
    /// <param name="npcSaveData">The save data</param>
    public void Load(NPCSaveData npcSaveData)
    {
        if (_player == null)
        {
            Debug.LogError("Cannot Load outside of game scene: Player not found.");
            return;
        }

        foreach (GameObject npcObject in GameObject.FindGameObjectsWithTag("NPC"))
        {

            if (!npcObject.TryGetComponent<NpcInteractable>(out var interactable))
            {
                Debug.LogError("Save: NPC " + npcObject.name + " does not have NPCInteractable component");
                continue;
            }

            interactable.Load(npcSaveData);
        }
    }
}
