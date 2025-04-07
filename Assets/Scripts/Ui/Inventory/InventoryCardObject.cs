using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCardObject : MonoBehaviour {
    #region ======== [ OBJECT REFERENCES ] ========

    [Header("Data")]
    public bool IsPreviewCard = false;
    [SerializeField] public InventoryCardData Card;

    [Header("Deactivated")]
    [SerializeField, BoxGroup("Deactivated")] private GameObject deactiveObject;
    [SerializeField, BoxGroup("Deactivated")] private Button deactiveButton;

    [Header("Item")]
    [SerializeField, BoxGroup("Item")] private GameObject itemLayoutObject;
    [SerializeField, BoxGroup("Item")] private Button itemLayoutButton;
    [SerializeField, BoxGroup("Item")] private TMP_Text itemNameText;
    [SerializeField, BoxGroup("Item")] private Image itemSpriteImage;
    [SerializeField, BoxGroup("Item")] private TMP_Text itemValueText;

    #endregion

    #region ======== [ INTERNAL PROPERTIES ] ========

    [HideInInspector] public string CardName;
    [HideInInspector] public string CardDescription;
    [HideInInspector] public string CardID;
    [HideInInspector] public Button CurrentActiveButton;

    private int _index;
    private AutoScrollGrid _scroller;
    private InventoryAction _onSelectAction = null;

    public enum CurrentState {
       ITEM, DEACTIVE
    }

    #endregion

    #region ======== [ INIT METHODS ] ========

    // Start is called before the first frame update
    void Start() {

        if (Card != null && IsPreviewCard == false) {
            SetData(Card);
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
    /// Sets the data of this UI object to the card given
    /// </summary>
    /// <param name="newCard">The cardData to fill</param>
    /// <returns></returns>
    public void SetData(InventoryCardData newCard, bool UseLargeItem = false)
    {
        if (newCard == null) return;
        
        Card = newCard;


        SwapState(CurrentState.ITEM);

        itemNameText.text = Card.CardName;
        itemSpriteImage.sprite = Card.Sprite;
        itemValueText.text = Card.BaseValue.ToString();
    }

    /// <summary>
    /// Sets card to empty!
    /// </summary>
    public void SetCardToEmpty(bool usingPreviewSize) {
        SwapState(CurrentState.DEACTIVE);
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
        deactiveButton.interactable = interactable;
        itemLayoutButton.interactable = interactable;
    }

    public void SwapState(CurrentState stateToEnter) {

        switch (stateToEnter) {
            case CurrentState.ITEM:
                itemLayoutObject.SetActive(true);
                deactiveObject.SetActive(false);

                CurrentActiveButton = itemLayoutButton;
                break;
            case CurrentState.DEACTIVE:
                itemLayoutObject.SetActive(false);
                deactiveObject.SetActive(true);

                Card = null;

                CurrentActiveButton = deactiveButton;
                break;

        }

    }

    #endregion
}
