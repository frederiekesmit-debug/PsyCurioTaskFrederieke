using UnityEngine;

public class ShelfItem : MonoBehaviour, IClickable
{
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Shopkeeper shopkeeper;

    public void OnClick()
    {
        if (shopkeeper == null)
            return;

        if (shopkeeper.IsBusy())
            return;

        shopkeeper.FulfillOrder(itemPrefab, transform);
    }
}
