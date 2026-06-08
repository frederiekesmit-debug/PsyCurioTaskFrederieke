using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] private Transform[] slots = new Transform[5];
    [SerializeField] private Vector3 placedScale = new Vector3(0.5f, 0.5f, 0.5f);

    private GameObject[] placedItems;
    private int nextSlotIndex = 0;

    private void Awake()
    {
        placedItems = new GameObject[slots.Length];
    }

    public bool TryPlace(GameObject obj)
    {
        if (nextSlotIndex >= slots.Length)
        {
            Debug.Log("Counter is full!");
            return false;
        }

        Transform slot = slots[nextSlotIndex];

        obj.transform.position = slot.position;
        obj.transform.rotation = slot.rotation;
        obj.transform.localScale = placedScale;

        placedItems[nextSlotIndex] = obj;
        nextSlotIndex++;

        return true;
    }

    public GameObject[] GetPlacedItems()
    {
        return placedItems;
    }
}