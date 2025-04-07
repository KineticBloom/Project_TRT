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

    [Header("Deactivated Layout")]
    [SerializeField, BoxGroup("Deactive Layout")] private GameObject deactiveObject;
    [SerializeField, BoxGroup("Deactive Layout")] private Button deactiveButton;

    [Header("Deactivated Preview")]
    [SerializeField, BoxGroup("Deactive Layout")] private GameObject deactivePreviewObject;
    [SerializeField, BoxGroup("Deactive Layout")] private Button deactivePreviewButton;
    //test
    [Header("Item Layout")]
    [SerializeField, BoxGroup("Item Layout")] private GameObject itemLayoutObject;
    [SerializeField, BoxGroup("Item Layout")] private Button itemLayoutButton;
    [SerializeField, BoxGroup("Item Layout")] private TMP_Text itemNameText;
    [SerializeField, BoxGroup("Item Layout")] private Image itemSpriteImage;
    [SerializeField, BoxGroup("Item Layout")] private TMP_Text itemDescriptionText;
    [SerializeField, BoxGroup("Item Layout")] private TMP_Text itemValueText;

    [Header("Item Preview Layout")]
    [SerializeField, BoxGroup("Item Preview Layout")] private GameObject itemPreviewLayoutObject;
    [SerializeField, BoxGroup("Item Preview Layout")] private Button itemPreviewLayoutButton;
    [SerializeField, BoxGroup("Item Preview Layout")] private TMP_Text itemPreviewNameText;
    [SerializeField, BoxGroup("Item Preview Layout")] private Image itemPreviewSpriteImage;
    [SerializeField, BoxGroup("Item Preview Layout")] private TMP_Text itemPreviewValueText;
    [SerializeField, BoxGroup("Item Preview Layout")] private TMP_Text itemPreviewCountText;

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
        DEACTIVE, ITEM, ITEMPREVIEW, DEACTIVEPREVIEW
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


        if (UseLargeItem) {
            SwapState(CurrentState.ITEM);

            itemNameText.text = Card.CardName;
            itemSpriteImage.sprite = Card.Sprite;
            itemDescriptionText.text = Card.Description;
            itemValueText.text = Card.BaseValue.ToString();
        } else {
            SwapState(CurrentState.ITEMPREVIEW);

            itemPreviewNameText.text = Card.CardName;
            itemPreviewSpriteImage.sprite = Card.Sprite;
            itemPreviewValueText.text = Card.BaseValue.ToString();
            itemPreviewCountText.text = GameManager.Inventory.GetCardFromData(newCard).Count.ToString();
        }

    }

    /// <summary>
    /// Sets card to empty!
    /// </summary>
    public void SetCardToEmpty(bool usingPreviewSize) {

        if (usingPreviewSize) {
            SwapState(CurrentState.DEACTIVEPREVIEW);
        } else {
            SwapState(CurrentState.DEACTIVE);
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
        deactiveButton.interactable = interactable;
        deactivePreviewButton.interactable = interactable;
        itemLayoutButton.interactable = interactable;
        itemPreviewLayoutButton.interactable = interactable;
    }

    public void SwapState(CurrentState stateToEnter) {

        switch (stateToEnter) {
            case CurrentState.DEACTIVE:
                itemLayoutObject.SetActive(false);
                itemPreviewLayoutObject.SetActive(false);
                deactiveObject.SetActive(true);
                deactivePreviewObject.SetActive(false);

                Card = null;

                CurrentActiveButton = deactiveButton;
                break;
            case CurrentState.ITEM:
                itemLayoutObject.SetActive(true);
                itemPreviewLayoutObject.SetActive(false);
                deactiveObject.SetActive(false);
                deactivePreviewObject.SetActive(false);


                CurrentActiveButton = itemLayoutButton;
                break;
            case CurrentState.ITEMPREVIEW:
                itemLayoutObject.SetActive(false);
                itemPreviewLayoutObject.SetActive(true);
                deactiveObject.SetActive(false);
                deactivePreviewObject.SetActive(false);

                CurrentActiveButton = itemPreviewLayoutButton;
                break;
            case CurrentState.DEACTIVEPREVIEW:
                itemLayoutObject.SetActive(false);
                itemPreviewLayoutObject.SetActive(false);
                deactiveObject.SetActive(false);
                deactivePreviewObject.SetActive(true);

                Card = null;

                CurrentActiveButton = deactivePreviewButton;
                break;

        }

    }

    #endregion
}
