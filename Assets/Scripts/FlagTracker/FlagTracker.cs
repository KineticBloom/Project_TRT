using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlagTracker : MonoBehaviour
{
    [SerializeField] FlagIndex flagIndex;
    
    Inventory _inventory => GameManager.Inventory;

    #region Unity Methods
    void Awake()
    {
        if (!flagIndex) flagIndex = (FlagIndex) Resources.Load("Assets/Resources/FlagIndex.asset");
        flagIndex.CreateFlags();
    }

    private void OnDisable() 
    {
        ResetSavedFlags();
    }
    #endregion
    
    #region Public Methods
    /// <summary>
    /// Check the value of flag with id
    /// </summary>
    /// <returns>The value of the flag. False if null</returns>
    public bool CheckFlag(string id) => flagIndex[id] ?? false;
    
    /// <summary>
    /// Returns flag with id
    /// </summary>
    /// <returns>The flag with id. Null if it doesn't exist.</returns>
    public Flag GetFlag(string id) => flagIndex[id];
    
    /// <summary>
    /// Returns the object form of a flag with id
    /// </summary>
    /// <returns>The object form of flag with id. Default flag if null.</returns>
    public object ExtractFlag(string id)
    {
        Flag flag = flagIndex[id] ?? new Flag();
        return flag.Type switch {Flag.FlagType.Counter => (int)flag, _ => (bool)flag};
    }
    
    /// <summary>
    /// Sets the flag with id 
    /// </summary>
    /// <param name="id">The id</param>
    /// <param name="value">Value to set it to</param>
    /// <param name="editInventory">Whether or not to edit the inventory</param>
    /// <returns>Whether the flag existed or not</returns>
    public bool SetFlag(string id, bool value = true, bool editInventory = true)
    {
        bool code = true;
        Flag flag = flagIndex[id];
        
        if (flag == null)
        {
            code = false;
            flag = new Flag(id);
            flagIndex[id] = flag;
        }
        flag.Value = value;
        
        if (flag.Type == Flag.FlagType.InventoryCard)
        {
            if (!flag.Card) flag.Card = _inventory.AllCardDatas.datas.FirstOrDefault(x => x.ID == id[3..]);
            if (editInventory)
            {
                if (value) _inventory.AddCard(flag.Card);
                else _inventory.RemoveCard(flag.Card);
            }
        }
        
        if (flag.Saved) SaveSystem.Save();
        return code;
    }
    
    /// <summary>
    /// Sets the counter flag with id
    /// </summary>
    /// <param name="id">The id</param>
    /// <param name="count">Number to set it to</param>
    /// <returns>Whether the flag existed or not</returns>
    public bool SetFlag(string id, int count)
    {
        bool code = true;
        Flag flag = flagIndex[id] ?? new Flag(id);
        if (flag.Type != Flag.FlagType.Counter) return false;
        
        if (flag.IsDefault())
        {
            code = false;
            flagIndex[id] = flag;
        }
        flag.Count = count;
        if (flag.Saved) SaveSystem.Save();
        
        return code;
    }
    
    /// <summary>
    /// Given an object, sets flag with id with correct object value
    /// </summary>
    /// <param name="id">The id</param>
    /// <param name="value">The object to set it to</param>
    /// <returns>Whether the flag existed or not</returns>
    public bool SetFlag(string id, object value) => Flag.GetFlagType(id) switch 
    {
        Flag.FlagType.Counter => SetFlag(id, (int)value),
        _ => SetFlag(id, (bool)value)
    };
    
    /// <summary>
    /// Given an inventory card, sets the corresponding flag to value
    /// </summary>
    /// <returns>Whether the flag existed or not</returns>
    public bool SetFlag(InventoryCardData card, bool value) => SetFlag("IC_"+card.ID, value, false);

    /// <summary>
    /// Update all Inventory Card flags to match the inventory
    /// </summary>
    public void UpdateICFlags()
    {
        foreach (InventoryCardData card in _inventory.Get())
        {
            SetFlag(card, true);
        }
    }

    /// <summary>
    /// Resets all flags to their default value.
    /// </summary>
    public void ResetFlags()
    {
        flagIndex.ResetFlags();
        UpdateICFlags();
    }
    
    /// <summary>
    /// Returns a copy of the saved flags.
    /// </summary>
    public List<Flag> SavedFlags() => new(flagIndex.SavedFlags);
    
    /// <summary>
    /// Loads the values of a list of flags into saved flags
    /// </summary>
    /// <param name="flags">Flags to load</param>
    public void LoadFlags(List<Flag> flags)
    {
        foreach (Flag flag in flags)
        {
            Flag flag1 = flagIndex[flag.ID];
            if (flag1 == null)
            {
                flag1 = new Flag();
                flagIndex[flag.ID] = flag1;
            }
            flag1.Copy(flag);
        }
    }
    
    /// <summary>
    /// Resets Saved Flags and regular flags
    /// </summary>
    public void ResetSavedFlags() => flagIndex.HardResetFlags();
    #endregion
}