using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] private Transform[] slots = new Transform[5];

    private int nextSlotIndex = 0;

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

        nextSlotIndex++;

        return true;
    }
}
