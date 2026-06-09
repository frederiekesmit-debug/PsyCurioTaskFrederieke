using System.Collections;
using UnityEngine;

public class Shopkeeper : MonoBehaviour
{
    [SerializeField] private Counter counter;
    [SerializeField] private float moveSpeed = 3f;

    [Header("Item Holding")]
    [SerializeField] private Transform handPoint;

    private Vector3 homePosition;
    private bool busy;

    private void Awake()
    {
        homePosition = transform.position;
    }

    public bool IsBusy()
    {
        return busy;
    }

    public void FulfillOrder(GameObject prefabToSpawn, Transform shelfPoint)
    {
        if (busy)
            return;

        StartCoroutine(FulfillOrderRoutine(prefabToSpawn, shelfPoint));
    }

    private IEnumerator FulfillOrderRoutine(GameObject prefabToSpawn, Transform shelfPoint)
    {
        busy = true;

        // Walk to shelf
        yield return MoveTo(shelfPoint.position);

        // Create item in hand
        GameObject heldItem = Instantiate(prefabToSpawn);

        heldItem.transform.SetParent(handPoint);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        // Walk to counter
        yield return MoveTo(counter.transform.position);

        // Remove from hand
        heldItem.transform.SetParent(null);

        // Place on counter
        if (!counter.TryPlace(heldItem))
        {
            Destroy(heldItem);
        }

        // Return home
        yield return MoveTo(homePosition);

        busy = false;
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime);

            yield return null;
        }

        transform.position = target;
    }
}
