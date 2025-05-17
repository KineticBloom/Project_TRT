using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Xml.Linq;
using DG.Tweening.Core.Easing;

public class ChooseItemCanvasController : MonoBehaviour
{

    [Header("Object References")]
    public GameObject InventoryCardObjectPrefab;
    public GameObject CardsHorizontalLayoutGroup;
    public Image NPCSprite;
    public InventoryBar InventoryBar;

    private List<GameObject> _cards = new List<GameObject>();
    private NPCData _passedInData;
    private NpcInteractable _npcInstance;

    public void InitOffer(NPCData npcData, NpcInteractable npcInstance) {
        InventoryBar.SetActiveSource(gameObject, true);

        _cards.Clear();

        _passedInData = npcData;
        _npcInstance = npcInstance;

        NPCSprite.sprite = _passedInData.Icon;

        int index = 0;

        foreach(InventoryCardData x in npcInstance.ItemsAvailable) {
            GameObject newCard = Instantiate(InventoryCardObjectPrefab, CardsHorizontalLayoutGroup.transform);
            _cards.Add(newCard);
            InventoryCardObject currentCardObject = newCard.GetComponent<InventoryCardObject>();
            currentCardObject.SetData(x, false);
            int copy = index;
            currentCardObject.CurrentActiveButton.onClick.AddListener(delegate { OnSelect(copy); });
            newCard.transform.localScale = Vector3.one * 0.85f;
            index += 1;
        }

        // Select the first item
        if (index > 0)
        {
            _cards[0].GetComponent<InventoryCardObject>().CurrentActiveButton.Select();
        }
    }

    public void OnSelect(int index) {       
        GameManager.MasterCanvas.GetComponent<InGameUi>().MoveToBartering(_passedInData, _npcInstance.ItemsAvailable[index], _npcInstance);
        InventoryBar.SetActiveSource(gameObject, false);
    }

    public void LeaveChooseItemScene() {
        StartCoroutine(LeaveSceneOnLastFrame());
    }

    IEnumerator LeaveSceneOnLastFrame()
    {
        yield return new WaitForEndOfFrame();
        InventoryBar.SetActiveSource(gameObject, false);
        Cleanup();
        GameManager.MasterCanvas.GetComponent<InGameUi>().MoveToDefault();
    }

    private void OnDisable() {
        Cleanup();
    }

    public void Cleanup() {
        foreach(GameObject x in _cards) {
            Destroy(x);
        }
    }

}
