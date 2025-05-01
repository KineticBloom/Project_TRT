using System.Collections.Generic;
using System.Linq;
using Ink.Runtime;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "FlagIndex", menuName = "ScriptableObjects/FlagIndex")]
public class FlagIndex : ScriptableObject
{
    public List<Flag> SavedFlags = new List<Flag>();
    [SerializeField] List<Flag> flags = new List<Flag>();
    [SerializeField] List<TextAsset> inkFiles = new List<TextAsset>();
    [SerializeField] AllInventoryCardDatas inventory;

    public Flag this[string id] 
    {
        get => IsSaved(id) ? SavedFlags.SingleOrDefault(f => f.ID == id) : flags.SingleOrDefault(f => f.ID == id);
        set
        {
            if (value.Saved) SavedFlags.Add(value);
            else flags.Add(value);
        }
    }
    
    #region Public Methods
    /// <summary>
    /// Creates flags from the variables in the ink files
    /// </summary>
    public void CreateFlags()
    {
        foreach (TextAsset inkJson in inkFiles)
        {
            Story temp = new Story(inkJson.text);
            
            foreach (string id in temp.variablesState)
            {
                CreateFlag(id);
            }
        }
        foreach (InventoryCardData data in inventory.datas)
        {
            CreateFlag($"IC_{data.ID}");
        }
    }

    /// <summary>
    /// Resets all flags to their default value.
    /// </summary>
    public void ResetFlags()
    {
        foreach (Flag flag in flags){
            flag.Reset();
        }
    }
    
    /// <summary>
    /// Resets all flags and saved flags to their default value.
    /// </summary>
    public void HardResetFlags()
    {
        ResetFlags();
        foreach (Flag flag in SavedFlags){
            flag.Reset();
        }
    }
    
    /// <summary>
    /// Clears the list of flags.
    /// </summary>
    public void ClearFlags()
    {
        SavedFlags.Clear();
        flags.Clear();
    }
    
    public bool IsSaved(string id) => id.Length > 5 && id[..5] == "SAVE_";
    #endregion
    
    #region Private Methods
    /// <summary>
    /// Creates a flag with id if necessary
    /// </summary>
    /// <param name="id"></param>
    void CreateFlag(string id)
    {
        Flag flag = this[id];
        
        if (flag == null)
        {
            flag = new Flag(id);
            if (flag.Saved) SavedFlags.Add(flag);
            else flags.Add(flag);
        }
        flag.Reset();
        
        if (flag.Type == Flag.FlagType.InventoryCard)
        {
            if (!flag.Card && inventory) flag.Card = inventory.datas.FirstOrDefault(x => x.ID == id[3..]);
        }
    }
    #endregion
    
    #region Unity Methods
    private void OnDisable() {
        ResetFlags();
    }
    #endregion
}

#if UNITY_EDITOR 
[CustomEditor(typeof(FlagIndex))]
public class FlagIndexEditor : Editor
{
    public override void OnInspectorGUI() 
    {
        base.OnInspectorGUI();

        // Add a custom button in the Inspector
        if (GUILayout.Button("Create Flags")) 
        {
            ((FlagIndex)target).CreateFlags();
        }
        
        if (GUILayout.Button("Reset Flags")) 
        {
            ((FlagIndex)target).ResetFlags();
        }
        
        if (GUILayout.Button("Hard Reset Flags")) 
        {
            ((FlagIndex)target).HardResetFlags();
        }
        
        if (GUILayout.Button("Clear Flags")) 
        {
            ((FlagIndex)target).ClearFlags();
        }
    }
}
#endif