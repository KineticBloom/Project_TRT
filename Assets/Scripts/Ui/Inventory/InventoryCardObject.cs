using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static InventoryCardObject;

public class InventoryCardObject : MonoBehaviour {
    #region ======== [ OBJECT REFERENCES ] ========

    [Header("Data")]
    public bool IsPreviewCard = false;
    [SerializeField] public InventoryCardData Card;

    [SerializeField, BoxGroup("Item")] private GameObject itemSmallObject;
    [SerializeField, BoxGroup("Item")] private Button itemSmallButton;
    [SerializeField, BoxGroup("Item")] private Image itemSpriteImage;
    [SerializeField, BoxGroup("Item")] private TMP_Text itemValueText;

    [SerializeField, BoxGroup("Item Unactive")] private GameObject itemUnactiveObject;
    [SerializeField, BoxGroup("Item Unactive")] private Button itemUnactiveButton;

    [SerializeField, BoxGroup("Item Full")] private GameObject itemFullObject;
    [SerializeField, BoxGroup("Item Full")] private Button itemFullButton;
    [SerializeField, BoxGroup("Item Full")] private Image itemFullSprite;
    [SerializeField, BoxGroup("Item Full")] private TMP_Text itemFullValueTextA;
    [SerializeField, BoxGroup("Item Full")] private TMP_Text itemFullValueTextB;

    [SerializeField, BoxGroup("Item Full Unactive")] private GameObject itemFullUnactiveObject;
    [SerializeField, BoxGroup("Item Full Unactive")] private Button itemFullUnactiveButton;



    #endregion

    #region ======== [ INTERNAL PROPERTIES ] ========

    [HideInInspector] public string CardName;
    [HideInInspector] public string CardDescription;
    [HideInInspector] public string CardID;
    [HideInInspector] public Button CurrentActiveButton => GetCurrentButton();

    private int _index;
    private AutoScrollGrid _scroller;
    private InventoryAction _onSelectAction = null;
    public enum CurrentState {
       ITEMSMALL, SMALLDEACTIVE, ITEMFULL, FULLDEACTIVE
    }

    CurrentState _currentState;
    GameObject _currentObject;

    #endregion

    #region ======== [ INIT METHODS ] ========

    // Start is called before the first frame update
    void Start() {

        if (Card != null && IsPreviewCard == false) {
            SetData(Card, false);
        }
    }

    /// <summary>
    /// Creates an empty inventory card for a InventoryGridController
    /// </summary>
    public void InitalizeToGrid(int indexInGrid, AutoScrollGrid gridAutoScroller, InventoryAction onSelectAction, bool usingPreviewSize) {
        _index = indexInGrid;
        _scroller = gridAutoScroller;
        _onSelectAction = onSelectAction;

        SetCardToEmpty(usingPreviewSize);
    }

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Set scale of InventoryCardObject
    /// </summary>
    /// <param name="sizeOfIcon"></param>
    public void SetScale(float ratio) {

        Vector3 newScale = Vector3.one * ratio;

        itemSmallObject.GetComponent<RectTransform>().localScale = newScale;
        itemUnactiveObject.GetComponent<RectTransform>().localScale = newScale;
        itemFullObject.GetComponent<RectTransform>().localScale = newScale;
        itemFullUnactiveObject.GetComponent<RectTransform>().localScale = newScale;
    }

    /// <summary>
    /// Get Current Scale of the InventoryCardObject
    /// </summary>
    /// <returns></returns>
    public Vector2 GetScale() {

        RectTransform CurrentTransform = null;

        switch (_currentState) {
            case CurrentState.ITEMSMALL:
                CurrentTransform = itemSmallObject.GetComponent<RectTransform>();
                break;
            case CurrentState.SMALLDEACTIVE:
                CurrentTransform = itemUnactiveObject.GetComponent<RectTransform>();
                break;
            case CurrentState.ITEMFULL:
                CurrentTransform = itemFullObject.GetComponent<RectTransform>();
                break;
            case CurrentState.FULLDEACTIVE:
                CurrentTransform = itemFullUnactiveObject.GetComponent<RectTransform>();
                break;
        }

        if(CurrentTransform == null) {
            return Vector2.zero;
        }

        Vector2 Scale = new Vector2(CurrentTransform.rect.width, CurrentTransform.rect.height);

        return Scale;
    }

    public Button GetCurrentButton() {

        switch (_currentState) {
            case CurrentState.ITEMSMALL:
                return itemSmallButton;
            case CurrentState.SMALLDEACTIVE:
                return itemUnactiveButton;
            case CurrentState.ITEMFULL:
                return itemFullButton;
            case CurrentState.FULLDEACTIVE:
                return itemFullUnactiveButton;
        }
        return itemSmallButton;
    }

    /// <summary>
    /// Sets the data of this UI object to the card given
    /// </summary>
    /// <param name="newCard">The cardData to fill</param>
    /// <returns></returns>
    public void SetData(InventoryCardData newCard, bool UseSmallSize)
    {
        if (newCard == null) return;
        
        Card = newCard;

        if (UseSmallSize) {
            SwapState(CurrentState.ITEMSMALL);
        } else {
            SwapState(CurrentState.ITEMFULL);
        }
        

        itemSpriteImage.sprite = Card.Sprite;
        itemFullSprite.sprite = Card.Sprite;

        itemValueText.text = "¥" + Card.CurrentValue.ToString();
        itemFullValueTextA.text = "¥" + Card.CurrentValue.ToString();
        itemFullValueTextB.text = "¥" + Card.CurrentValue.ToString();
    }

    /// <summary>
    /// Sets card to empty!
    /// </summary>
    public void SetCardToEmpty(bool usingPreviewSize) {
        if (usingPreviewSize) {
            SwapState(CurrentState.SMALLDEACTIVE);
        } else {
            SwapState(CurrentState.FULLDEACTIVE);
        }
    }

    /// <summary>
    /// When user hovers over this card.
    /// </summary>
    public void OnSelect(BaseEventData eventData) {
        if (_scroller != null) {
            _scroller.FrameCardInGrid(_index);
        }
    }

    /// <summary>
    /// When user chooses this card.
    /// </summary>
    public void OnPress() {

        if (_onSelectAction == null) {
            return;
        }

        InventoryAction.ActionContext ctx = new InventoryAction.ActionContext();
        ctx.cardData = Card;
        _onSelectAction.ActionOnClick(ctx);

    }

    /// <summary>
    /// Turns all buttons in the InventoryCardObject interactable or not
    /// </summary>
    /// <param name="interactable">Whether or not the buttons can be pressed</param>
    public void SetInteractable(bool interactable)
    {
        itemSmallButton.interactable = interactable;
    }

    public void SwapState(CurrentState stateToEnter) {

        _currentState = stateToEnter;

        if (_currentObject != null) {
            _currentObject.SetActive(false);
        }

        switch (stateToEnter) {
            case CurrentState.ITEMSMALL:
                _currentObject = itemSmallObject;
                break;
            case CurrentState.SMALLDEACTIVE:
                _currentObject = itemUnactiveObject;
                Card = null;
                break;
            case CurrentState.ITEMFULL:
                _currentObject = itemFullObject;
                break;
            case CurrentState.FULLDEACTIVE:
                _currentObject = itemFullUnactiveObject;
                break;
        }

        _currentObject.SetActive(true);

    }
    #endregion
}
