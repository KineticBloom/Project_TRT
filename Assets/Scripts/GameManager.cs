using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

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

    public static UiManagerBase CurrentUIManager { get { return Instance.UiManagerInFocus; } }

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
    [SerializeField] private UiManagerBase UiManagerInFocus;
    [SerializeField] public AllNPCDatas AllNPCDatas;


    // Initializers ===============================================================================

    public void Start()
    {
        if (UiManagerInFocus != null)
        {
            UiManagerInFocus.CurrentFocus = true;
        }
    }

    public void FindPlayer()
    {
        print("FindPlayer() called");

        GameObject playerObj = GameObject.FindWithTag(playerTag);

        if (playerObj != null)
        {
            GameObject playerParent = playerObj.transform.root.gameObject;

            if (playerParent != null)
            {
                _player = playerParent.GetComponentInChildren<Player>();
            }
        }
    }

    public void FindMasterCanvas()
    {
        print("FindMasterCanvas() called");

        GameObject masterCanvasObj = GameObject.FindWithTag(masterCanvasTag);

        if (masterCanvasObj != null)
        {
            _masterCanvas = masterCanvasObj.GetComponentInChildren<Canvas>();
        }
    }

    public void SwapUiManager(UiManagerBase NewUIManager)
    {
        if (NewUIManager == null) return;

        if (UiManagerInFocus != null)
        {
            UiManagerInFocus.CurrentFocus = false;
        }
        NewUIManager.CurrentFocus = true;
        UiManagerInFocus = NewUIManager;
    }

    // Save and Load =============================================================================

    /// <summary>
    /// Saves the NPC Effect Cards and Saved Flags
    /// </summary>
    /// <param name="saveData">Save Data holding all of the effect cards and saved flags</param>
    public void Save(ref SaveSystem.SaveData saveData)
    {
        if (_player == null)
        {
            Debug.LogError("Cannot Save outside of game scene: Player not found.");
            return;
        }

        NPCSaveData npcSaveData = saveData.npcSaveData;

        foreach (NPCData data in AllNPCDatas.datas)
        {
            npcSaveData.effectCardData[data.FlagID] = new List<bool>();

            // Save reveal status for all Effect Cards
            for (int effectCardIndex = 0; effectCardIndex < data.EffectCards.Count; effectCardIndex++)
            {
                npcSaveData.effectCardData[data.FlagID].Add(data.EffectCards[effectCardIndex].IsRevealed);
            }
        }

        saveData.flagSaveData = flagTracker.SavedFlags();
    }

    /// <summary>
    /// Loads Effect Cards and Saved Flags from save data
    /// </summary>
    /// <param name="saveData">The save data</param>
    public void Load(SaveSystem.SaveData saveData)
    {
        if (_player == null)
        {
            Debug.LogError("Cannot Load outside of game scene: Player not found.");
            return;
        }

        NPCSaveData npcSaveData = saveData.npcSaveData;

        if (npcSaveData.effectCardData == null)
        {
            Debug.LogError("NPCInteractable: Cannot Load null data");
            return;
        }

        foreach (NPCData data in AllNPCDatas.datas)
        {
            // Cannot load data that does not exist
            if (!npcSaveData.effectCardData.ContainsKey(data.FlagID))
            {
                return;
            }

            List<bool> effectCardsList = npcSaveData.effectCardData[data.FlagID];

            // Set reveal status for all Effect Cards
            for (int effectCardIndex = 0; effectCardIndex < data.EffectCards.Count; effectCardIndex++)
            {
                data.EffectCards[effectCardIndex].IsRevealed = effectCardsList[effectCardIndex];
            }
        }

        if (saveData.flagSaveData == null)
        {
            Debug.LogError("FlagSaveData: Cannot Load null data");
            return;
        }

        flagTracker.LoadFlags(saveData.flagSaveData);
    }

    public void SaveLimbo(ref SaveSystem.SaveData saveData)
    {
        LimboDialogue limboDialogue = GameObject.FindGameObjectWithTag("Limbo").GetComponent<LimboDialogue>();
        if (!limboDialogue) return;

        saveData.currentLimboFile = limboDialogue.currentInkFile;
    }

    public void LoadLimbo(SaveSystem.SaveData saveData)
    {
        LimboDialogue limboDialogue = GameObject.FindGameObjectWithTag("Limbo").GetComponent<LimboDialogue>();
        if (!limboDialogue) return;

        limboDialogue.currentInkFile = saveData.currentLimboFile;
    }
}

