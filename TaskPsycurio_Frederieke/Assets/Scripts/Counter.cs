using UnityEngine;

public class Counter : MonoBehaviour
{
    [SerializeField] private Transform[] slots = new Transform[5];

    [SerializeField] private Vector3 placedScale = new Vector3(0.5f, 0.5f, 0.5f);

    private int nextSlotIndex = 0;

    public bool TryPlace(GameObject obj)
    {
        if (nextSlotIndex >= slots.Length)
        {
            Debug.Log("Counter is full!");
            return false;
        }

        Transform slot = slots[nextSlotIndex];

        // Move to slot
        obj.transform.position = slot.position;
        obj.transform.rotation = slot.rotation;

        // Make smaller on counter
        obj.transform.localScale = placedScale;

        nextSlotIndex++;

        return true;
    }
}