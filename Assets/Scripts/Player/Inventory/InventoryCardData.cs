using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using NaughtyAttributes.Editor;
// using MackySoft.SerializeReferenceExtensions;   // Check if this is required in the build to function properly
#endif


[CreateAssetMenu(fileName = "InventoryCard", menuName = "ScriptableObjects/InventoryCard", order = 1)]
public class InventoryCardData : ScriptableObject
{
    public string CardName;

    public string ID;
    public string Description;
    public Sprite Sprite;
    public int BaseValue;

    public List<string> Tags = new List<string>();

    private int _currentValue = 0;

    public int CurrentValue => _currentValue;
    public void SetCurrentValue(int value) => _currentValue = value;
    public void ResetCurrentValue() => _currentValue = BaseValue;

    private void OnEnable()
    {
        _currentValue = BaseValue;
    }


    /// <summary>
    /// Checks if the ID matches and returns the result
    /// </summary>
    /// <param name="other">The other InventoryCardData item</param>
    public bool IsSame(InventoryCardData other)
    {
        return ID == other.ID;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryCardData))]
public class InventoryCardDataEditor : NaughtyInspector
{
    private bool onSelectionChange = false;
    private string _tagField = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Draw TagField Manually
        // This is to allow it to be editable, but non-serializable
        InventoryCardData icd = (target as InventoryCardData);

        EditorGUILayout.HelpBox("You can quickly modify the tags by filling in the field below with your tags. " +
            "Seperate with either commas or spaces. Press \"Update Tags\" when you're done and remember to save!", MessageType.Info);

        if (!onSelectionChange)
        {
            onSelectionChange = true;
            UpdateTagField(icd);
        }

        _tagField = EditorGUILayout.TextField("Tags: ", _tagField);
        if (GUILayout.Button("Update Tags"))
        {
            UpdateTags(icd);
        }
    }

    private void UpdateTags(InventoryCardData icd)
    {
        icd.Tags.Clear();

        // Used to avoid duplicates
        HashSet<string> addedTags = new HashSet<string>();

        string[] splitTags = _tagField.ToLower().Trim().Split(' ', ',');
        foreach (var extractedTag in splitTags)
        {
            var tag = extractedTag.ToLower().Trim();
            if (tag.Length < 1) continue;
            if (addedTags.Contains(tag)) continue;

            icd.Tags.Add(tag);
            addedTags.Add(tag);
        }

        EditorUtility.SetDirty(icd);
    }

    private void UpdateTagField(InventoryCardData icd)
    {
        if (_tagField != "") return;

        string tagField = "";
        foreach (string tag in icd.Tags)
        {
            tagField += tag + " ";
        }
        _tagField = tagField.Trim();
    }
}
#endif