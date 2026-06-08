using UnityEngine;

public class ItemData : MonoBehaviour
{
    [SerializeField] private string itemName;
    [SerializeField] private int price;

    public string GetItemName()
    {
        return string.IsNullOrEmpty(itemName) ? gameObject.name : itemName;
    }

    public int GetPrice()
    {
        return price;
    }
}
