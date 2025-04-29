using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class Flag
{
    public enum FlagType {InventoryCard, Relay, Counter};
    
    public string ID;
    public FlagType Type;
    public bool Saved {get; private set;}
    [ShowIf("Type", FlagType.InventoryCard), AllowNesting] public InventoryCardData Card;
    [HideIf("Type", FlagType.Counter), AllowNesting]public bool DefaultValue = false;
    [HideIf("Type", FlagType.Counter), AllowNesting]public bool Value = false;
    [ShowIf("Type", FlagType.Counter), AllowNesting] public int DefaultCount = 0;
    [ShowIf("Type", FlagType.Counter), AllowNesting] public int Count = 0;
    
    /// <summary>
    /// Checks whether this flag is a default value
    /// </summary>
    /// <returns>If this flag is the default</returns>
    public bool IsDefault(){
        return ID == "default";
    }
    
    // === Constructors ===
    public Flag()
    {
        ID = "default";
        Type = FlagType.Relay;
        Saved = false;
        DefaultValue = false;
        Value = false;
    }
    
    public Flag(string id) : this()
    {
        ID = id;
        
        if (id.Length > 5 && id[..5] == "SAVE_") 
        {
            id = id[5..];
            Saved = true;
        }
        
        Type = GetFlagType(id);
    }
    
    /// <summary>
    /// Converts an id into a FlagType
    /// </summary>
    public static FlagType GetFlagType(string id)
    {
        if (id.Length > 5 && id[..5] == "SAVE_") id = id[5..];
        if (id.Length > 3 && id[..3] == "IC_") return FlagType.InventoryCard;
        else if (id.Length > 4 && id[..4] == "NUM_") return FlagType.Counter;
        return FlagType.Relay;
    }
    
    /// <summary>
    /// Resets the Flag
    /// </summary>
    public void Reset()
    {
        if (Type == FlagType.Counter) Count = DefaultCount;
        Value = DefaultValue;
    }
    
    /// <summary>
    /// Copies a given flag's values to this flag
    /// </summary>
    /// <param name="flag">The flag to copy</param>
    public void Copy(Flag flag)
    {
        ID = flag.ID;
        Type = flag.Type;
        Card = flag.Card;
        DefaultValue = flag.DefaultValue;
        Value = flag.Value;
        DefaultCount = flag.DefaultCount;
        Count = flag.Count;
    }
    
    // === System Modifiers ===
    public static implicit operator bool(Flag f){ return f.Value; }
    public static implicit operator int(Flag f){ return f.Type == FlagType.Counter ? f.Count : (f.Value ? 1 : 0); }
}

[Serializable]
public class FlagReference
{
    [SerializeField] string id;
    Flag flag;
    
    // === System Modifiers ===
    public static implicit operator bool(FlagReference f)
    { 
        if (f.flag == null || f.flag.IsDefault() ) {
            f.flag = GameManager.FlagTracker.GetFlag(f.id);
        }
        return f.flag;
    }
}