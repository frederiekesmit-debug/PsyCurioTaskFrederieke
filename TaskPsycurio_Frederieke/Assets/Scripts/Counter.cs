using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] private Transform[] slots = new Transform[5];
    [SerializeField] private Vector3 placedScale = new Vector3(0.8f, 0.8f, 0.8f);

    private List<GameObject> placedItems;

    private void Awake()
    {
        placedItems = new List<GameObject>(slots.Length);

        // Pre-fill so indices match slot positions
        for (int i = 0; i < slots.Length; i++)
        {
            placedItems.Add(null);
        }
    }

    public bool TryPlace(GameObject obj)
    {
        int freeSlot = GetFirstFreeSlot();

        if (freeSlot == -1)
        {
            Debug.Log("Counter is full!");
            return false;
        }

        Transform slot = slots[freeSlot];

        obj.transform.position = slot.position;
        obj.transform.rotation = slot.rotation;
        obj.transform.localScale = placedScale;

        placedItems[freeSlot] = obj;

        // Ensure only one CounterItem exists
        CounterItem item = obj.GetComponent<CounterItem>();
        if (item == null)
            item = obj.AddComponent<CounterItem>();

        item.Setup(this, freeSlot);

        return true;
    }

    public bool IsFull()
    {
        return GetFirstFreeSlot() == -1;
    }

    public void RemoveItem(int index)
    {
        if (index < 0 || index >= placedItems.Count)
            return;

        placedItems[index] = null;
    }

    public GameObject[] GetPlacedItems()
    {
        return placedItems.ToArray();
    }

    private int GetFirstFreeSlot()
    {
        for (int i = 0; i < placedItems.Count; i++)
        {
            if (placedItems[i] == null)
                return i;
        }

        return -1;
    }
}