using UnityEngine;

public class ShelfItem : MonoBehaviour, IClickable
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Shopkeeper shopkeeper;
    [SerializeField] private Transform grabPoint;

    public void OnClick()
    {
        if (shopkeeper == null || grabPoint == null)
            return;

        if (shopkeeper.IsBusy())
            return;

        shopkeeper.FulfillOrder(itemPrefab, grabPoint);
    }
}
