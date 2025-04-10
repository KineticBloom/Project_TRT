#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using TMPro;

public class PlaytestMenuManager : MonoBehaviour {
    
    public GameObject SceneSelectButtonPrefab;
    public GameObject ButtonParent;
    public float DistanceBetweenButtons = 200;
    public List<string> ScenesToCreate;

    List<GameObject> _playtestButtons = new List<GameObject>();

    protected void Awake() 
    {
        int totalScenes = ScenesToCreate.Count;

        for (int i = 0; i < totalScenes; i++) {
            var scenePath = ScenesToCreate[i];

            string sceneName = Path.GetFileNameWithoutExtension(scenePath); // ObjectNames.NicifyVariableName(Path.GetFileNameWithoutExtension(scenePath));

            _playtestButtons.Add(CreateButton(sceneName, scenePath));
        }
    }

    /// <summary>
    /// Create a new scene Navigation Button 
    /// </summary>
    /// <param name="sceneName"> The name to display on Button. </param>
    /// <param name="scenePath"> Path to scene in Asset Database.</param>
    GameObject CreateButton(string sceneName, string scenePath) 
    {
        // Create button
        var buttonObject = Instantiate(SceneSelectButtonPrefab, Vector3.zero, Quaternion.identity, transform);
        var buttonText = buttonObject.GetComponentInChildren<TMP_Text>();
        var buttonComponent = buttonObject.GetComponent<Button>();

        if (buttonText == null || buttonComponent == null) return buttonObject;

        // Set name
        buttonText.SetText(sceneName);

        // Link Button to scene
        buttonComponent.onClick.AddListener(() => SceneManager.LoadScene(scenePath));

        return buttonObject;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(PlaytestMenuManager))]

public class PlaytestMenuEditor : Editor 
{
    /// <summary>
    /// Init button to load all scenes in build path.
    /// </summary>
    public override void OnInspectorGUI() {

        base.OnInspectorGUI();

        // Add a custom button in the Inspector

        if (GUILayout.Button("Add Scenes to Playtest Menu.")) {

            PlaytestMenuManager playtestMenu = (PlaytestMenuManager)target;

            int oldCount = playtestMenu.ScenesToCreate.Count;

            playtestMenu.ScenesToCreate = new List<string>();

            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
                playtestMenu.ScenesToCreate.Add(scene.path);
            }

            Debug.Log("Added " + (playtestMenu.ScenesToCreate.Count - oldCount) + " to scene!");
        }
    }
}
#endif