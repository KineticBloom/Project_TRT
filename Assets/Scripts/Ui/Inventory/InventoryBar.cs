using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InventoryBar : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private RectTransform smallInventory;
    [SerializeField] private RectTransform largeInventory;
    [SerializeField] private InventoryGridController smallInventoryController;
    [SerializeField] private InventoryGridController largeInventoryController;

    [Header("Animation Parameters")]
    [SerializeField] private float duration;
    [SerializeField] private Ease showEase;
    [SerializeField] private Ease hideEase;
    [SerializeField] private Vector2 smallToLargeScale = new Vector3(1.52f, 2.27f, 1);
    [SerializeField] private Vector2 largeToSmallScale = new Vector3(0.657f, 0.44f, 1);

    private Dictionary<GameObject, bool> _activeSources = new Dictionary<GameObject, bool>();
    private bool _isShown;


    /// <summary>
    /// Will have the large inventory bar show as long as one source has its value set to true
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value">Whether the large inventory should show</param>
    public void SetActiveSource(GameObject source, bool value)
    {
        _activeSources[source] = value;

        // Check if anything is true, if so show. otherwise, false
        foreach (KeyValuePair<GameObject, bool> entry in _activeSources)
        {
            if (!entry.Value) continue;

            if (!_isShown)
            {
                SwitchFromSmall();
            }
            return;
        }

        if (_isShown)
        {
            SwitchFromLarge();
        }
    }

    public void SetInteractable(bool interactable)
    {
        smallInventoryController.SetSlotsInteractable(interactable);
        largeInventoryController.SetSlotsInteractable(interactable);
    }

    private void SwitchFromSmall()
    {
        _isShown = true;
        DOTween.Kill(smallInventory);
        DOTween.Kill(largeInventory);
        DOTween.Kill(largeInventory.GetComponent<CanvasGroup>());

        // Scale Tweens
        largeInventory.localScale = largeToSmallScale;

        smallInventory.DOScale(smallToLargeScale, duration)
            .SetEase(showEase).SetUpdate(true);
        smallInventory.GetComponent<CanvasGroup>()
              .DOFade(0, duration).SetEase(showEase).SetUpdate(true);
        largeInventory.DOScale(Vector3.one, duration)
            .SetEase(showEase).SetUpdate(true);

        // Fade In
        largeInventory.gameObject.SetActive(true);
        largeInventory.GetComponent<CanvasGroup>()
            .DOFade(1, duration).SetEase(showEase).SetUpdate(true)
            .OnComplete(() => smallInventory.gameObject.SetActive(false));
    }


    private void SwitchFromLarge()
    {
        _isShown = false;

        DOTween.Kill(smallInventory);
        DOTween.Kill(largeInventory);
        DOTween.Kill(largeInventory.GetComponent<CanvasGroup>());

        // Scale Tween
        smallInventory.localScale = smallToLargeScale;

        smallInventory.DOScale(Vector3.one, duration)
            .SetEase(hideEase).SetUpdate(true);
        smallInventory.GetComponent<CanvasGroup>()
      .DOFade(1, duration).SetEase(showEase).SetUpdate(true);
        largeInventory.DOScale(largeToSmallScale, duration)
            .SetEase(hideEase).SetUpdate(true);

        // Fade Out
        smallInventory.gameObject.SetActive(true);
        largeInventory.GetComponent<CanvasGroup>()
            .DOFade(0, duration).SetEase(hideEase).SetUpdate(true)
            .OnComplete(() => largeInventory.gameObject.SetActive(false));
    }
}
