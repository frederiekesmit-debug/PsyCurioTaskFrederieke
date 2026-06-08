using UnityEngine;

public class ClickToDuplicateToCounter : MonoBehaviour, IClickable
{
    [SerializeField] private Counter counter;
    [SerializeField] private GameObject prefabToSpawn;

    public void OnClick()
    {
        if (counter == null || prefabToSpawn == null)
            return;

        // BLOCK SPAWN IF FULL
        if (counter.IsFull())
        {
            Debug.Log("Counter is full!");
            return;
        }

        GameObject obj = Instantiate(prefabToSpawn);
        counter.TryPlace(obj);
    }
}
