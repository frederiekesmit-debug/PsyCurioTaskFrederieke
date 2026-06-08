using UnityEngine;

public class ClickToDuplicateToCounter : MonoBehaviour, IClickable
{
    [SerializeField] private Counter counter;
    [SerializeField] private GameObject prefabToSpawn;

    public void OnClick()
    {
        if (counter == null || prefabToSpawn == null)
            return;

        if (counter.TryPlace(Instantiate(prefabToSpawn)))
        {
            Debug.Log("Spawned on counter");
        }
    }
}
