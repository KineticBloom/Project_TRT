using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DevMenu : Singleton<DevMenu> {
    [SerializeField] GameObject devMenu;
    [SerializeField] TMP_InputField textInput;
    [SerializeField] TMP_InputField output;

    private float _initalTimescale = 0;
    private bool _devOn = false;
    private GameObject _lastSelected;
    
    private void Start()
    {
        devMenu.SetActive(false);
        output.text = helpMessage;
        textInput.onSubmit.AddListener(ConsoleCommand);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.PlayerInput == null) return;
        else if (GameManager.PlayerInput.GetDebug0Down()) ToggleDevMenu();
        else if (GameManager.PlayerInput.GetDebug1Down()) ResetGame();
    }
    
    void ToggleDevMenu() 
    {
        _devOn = !_devOn;

        GameManager.PlayerInput.ToggleControls(!_devOn);
        devMenu.SetActive(_devOn);
        textInput.text = "";
        if (_devOn) {
            _lastSelected = EventSystem.current.currentSelectedGameObject;
            textInput.ActivateInputField();
        }
        else if (_lastSelected != null) {
            if (_lastSelected.activeSelf && _lastSelected.TryGetComponent(out UnityEngine.UI.Selectable selectable)) selectable.Select();
            _lastSelected = null;
        }
        _initalTimescale = _devOn ? Time.timeScale : _initalTimescale;
        Time.timeScale = _devOn ? 0 : _initalTimescale;
    }
    
    void CloseDevMenu()
    {
        _devOn = false;
        GameManager.PlayerInput.ToggleControls(true);
        
        devMenu.SetActive(_devOn);
        Time.timeScale = 1;
    }
    
    void CloseDevMenu(Scene s, LoadSceneMode lsm)
    {
        CloseDevMenu();
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += CloseDevMenu;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= CloseDevMenu;
    }
    
    string helpMessage = 
    "Commands:\n"+
    "    give <item ID> [<amount>] - Give item with <item ID> with optional <amount> (default: 1)\n"+
    "    remove <item ID> - Remove one instance of the item with <item ID>\n"+
    "    setflag <flag ID> [true|false] - Set the flag with <flag ID> to true or false (default: true)\n"+
    "    setspeed <n> - Set the player speed to <n> times the default\n"+
    "    addtime <n> - Add <n> seconds to the timer\n"+
    "    pauseloop - Pause and Unpause the timeloop\n"+
    "    endloop - End the time loop\n"+
    "    printitems - Prints a list of all item IDs\n"+
    "    printdata - Prints save data file\n"+
    "    reset - Reset game\n"+
    "    quit - Quit game\n"+
    "    exit - Close menu\n"+
    "    clear - Clear the console\n"+
    "    help - Display the command list\n";
    
    void ConsoleCommand(string command)
    {
        textInput.text = "";
        textInput.ActivateInputField();
        if (command.Length <=0) return;
        
        output.text += command+"\n";
        string[] parser = command.ToLower().Split();
        
        switch (parser[0])
        {
            case "help":
                output.text += helpMessage;
                break;
            case "clear":
                output.text = "Console Cleared.\n";
                break;
            case "printdata":
                output.text += File.ReadAllText(SaveSystem.SaveFileName())+"\n";
                break;
            case "printitems":
                output.text += "-----Items-----\n";
                foreach (InventoryCardData data in GameManager.Inventory.AllCardDatas.datas)
                {
                    output.text += $"{data.ID}\n";
                }
                output.text += "---------------\n";
                break;
            case "pauseloop":
                if (TimeLoopManager.Instance == null)
                {
                    output.text += "<color=\"red\">Error: Timeloop is not Active</color>\n";
                    return;
                }
                TimeLoopManager.SetLoopPaused(!TimeLoopManager.LoopPaused);
                string s = TimeLoopManager.LoopPaused ? "Paused" : "Unpaused";
                output.text += $"<color=\"green\">Timeloop {s}</color>\n";
                break;
            case "endloop":
                if (TimeLoopManager.Instance == null)
                {
                    output.text += "<color=\"red\">Error: Timeloop is not Active</color>\n";
                    return;
                }
                output.text += "<color=\"green\">Ending Loop...</color>\n";
                ResetLoop();
                break;
            case "reset":
                output.text += "<color=\"green\">Resetting Game...</color>\n";
                ResetGame();
                break;
            case "quit":
                output.text += "<color=\"green\">Quitting Game...</color>\n";
                QuitGame();
                break;
            case "exit":
                CloseDevMenu();
                break;
            case "give":
                if (parser.Length < 2 || parser.Length > 3)
                {
                    output.text += "<color=\"red\">Error: Invalid Number of Arguments</color>\n";
                    return;
                }
                Inventory inventory = GameManager.Inventory;
                InventoryCardData card = inventory.AllCardDatas.datas.FirstOrDefault(x => x.ID == parser[1]);
                if (card)
                {
                    try
                    {
                        int num = (parser.Length == 3) ? (int) uint.Parse(parser[2]) : 1;
                        for (int i = 0; i < num; i++)
                        {
                            inventory.AddCard(card);
                        }
                        output.text += "<color=\"green\">Success</color>\n";
                    }
                    catch (System.Exception)
                    {
                        output.text += "<color=\"red\">Error: Invalid Amount</color>\n";
                    }
                }
                else {
                    output.text += $"<color=\"red\">Error: Couldn't find item with ID {parser[1]}</color>\n";
                }
                break;
            case "remove":
                if (parser.Length < 2 || parser.Length > 3)
                {
                    output.text += "<color=\"red\">Error: Invalid Number of Arguments</color>\n";
                    return;
                }
                Inventory inventory2 = GameManager.Inventory;
                InventoryCardData card2 = inventory2.AllCardDatas.datas.FirstOrDefault(x => x.ID == parser[1]);
                if (card2)
                {
                    if (inventory2.HasCard(card2))
                    {
                        inventory2.RemoveCard(card2);
                        output.text += "<color=\"green\">Success</color>\n";
                    }
                    else {
                        output.text += "<color=\"red\">Error: Item Not in Inventory</color>\n";
                    }
                }
                else {
                    output.text += $"<color=\"red\">Error: Couldn't find item with ID {parser[1]}</color>\n";
                }
                break;
            case "setflag":
                if (parser.Length < 2 || parser.Length > 3)
                {
                    output.text += "<color=\"red\">Error: Invalid Number of Arguments</color>\n";
                    return;
                }
                FlagTracker flagTracker = GameManager.FlagTracker;
                if (flagTracker.GetFlag(parser[1]) != null)
                {
                    bool value = true;
                    if (parser.Length > 2)
                    {
                        try
                        {
                            value = bool.Parse(parser[2]);
                        }
                        catch (System.Exception)
                        {
                            output.text += "<color=\"red\">Error: Invalid Argument</color>\n";
                            return;
                        }
                    }
                    flagTracker.SetFlag(parser[1], value);
                    output.text += "<color=\"green\">Success</color>\n";
                }
                else {
                    output.text += $"<color=\"red\">Error: Couldn't find flag with ID {parser[1]}</color>\n";
                }
                break;
            case "setspeed":
                if (parser.Length != 2)
                {
                    output.text += "<color=\"red\">Error: Invalid Number of Arguments</color>\n";
                    return;
                }
                if (GameManager.Player == null)
                {
                    output.text += "<color=\"red\">Error: Player Doesn't Exist</color>\n";
                    return;
                }
                try
                {
                    float sp = float.Parse(parser[1]);
                    GameManager.Player.Movement.SetSpeed(sp);
                    output.text += "<color=\"green\">Success</color>\n";
                }
                catch (System.Exception)
                {
                    output.text += "<color=\"red\">Error: Invalid Number</color>\n";
                }
                break;
            case "addtime":
                if (parser.Length != 2)
                {
                    output.text += "<color=\"red\">Error: Invalid Number of Arguments</color>\n";
                    return;
                }
                if (TimeLoopManager.Instance == null)
                {
                    output.text += "<color=\"red\">Error: Timeloop is not Active</color>\n";
                    return;
                }
                try
                {
                    float t = float.Parse(parser[1]);
                    TimeLoopManager.AddTime(t);
                    output.text += "<color=\"green\">Success</color>\n";
                }
                catch (System.Exception)
                {
                    output.text += "<color=\"red\">Error: Invalid Number of Seconds</color>\n";
                }
                break;
            case "rumble":
                if (parser.Length != 4)
                {
                    output.text += "<color=\"red\">Error: Invalid Number of Arguments</color>\n";
                    return;
                }
                try
                {
                    float l = float.Parse(parser[1]);
                    float h = float.Parse(parser[2]);
                    float t = float.Parse(parser[3]);
                    
                    PlayerInputHandler.SetHaptics(l, h, t);
                    output.text += "<color=\"green\">Success</color>\n";
                }
                catch (System.Exception)
                {
                    output.text += "<color=\"red\">Error: Invalid Number</color>\n";
                }
                break;
            default:
                output.text += "<color=\"red\">Error: Invalid Command</color>\n";
                return;
        }
    }
    
    #region Public Methods
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void ResetLoop()
    {
        CloseDevMenu();
        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(false);
            TimeLoopManager.ResetLoop();
        }
    }
    
    public void PauseLoop()
    {
        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(!TimeLoopManager.LoopPaused);
        }
    }
    
    public void ResetGame(){
        if (GameManager.Inventory != null) {
            GameManager.Inventory.Clear();
        }
        SaveSystem.ResetSaveData();
        SceneManager.LoadScene(0);
    }
    
    public void GiveAllItems()
    {
        if (GameManager.Inventory != null)
        {
            foreach (InventoryCardData data in GameManager.Inventory.AllCardDatas.datas)
            {
                if (GameManager.Inventory.HasCard(data)) continue;
                GameManager.Inventory.AddCard(data);
            }
        }
    }
    #endregion
}
