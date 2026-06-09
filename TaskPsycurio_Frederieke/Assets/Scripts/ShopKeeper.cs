using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Shopkeeper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Counter counter;
    [SerializeField] private Transform handPoint;
    [SerializeField] private Animator animator;

    private NavMeshAgent agent;
    private Vector3 homePosition;

    private bool busy;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        homePosition = transform.position;
    }

    private void Update()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
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

        // Counter full?
        if (counter.IsFull())
        {
            busy = false;
            yield break;
        }

        // Walk to shelf
        yield return MoveTo(shelfPoint.position);

        // Create held item
        GameObject heldItem = Instantiate(prefabToSpawn);

        heldItem.transform.SetParent(handPoint);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        // Walk to counter
        yield return MoveTo(counter.transform.position);

        // Remove from hand
        heldItem.transform.SetParent(null);

        // Place item
        if (!counter.TryPlace(heldItem))
        {
            Destroy(heldItem);
        }

        // Walk home
        yield return MoveTo(homePosition);

        busy = false;
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);

        while (agent.pathPending)
            yield return null;

        while (agent.remainingDistance > agent.stoppingDistance)
            yield return null;

        while (agent.velocity.sqrMagnitude > 0.01f)
            yield return null;
    }
}
