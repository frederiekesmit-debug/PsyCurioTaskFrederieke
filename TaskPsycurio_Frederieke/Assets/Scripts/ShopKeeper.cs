using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Shopkeeper : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Counter counter;
    [SerializeField] private Transform counterPoint;
    [SerializeField] private Transform handPoint;
    [SerializeField] private Animator animator;

    [Header("Animation")]
    [SerializeField] private float pickupDuration = 1f;

    private NavMeshAgent agent;

    private Vector3 homePosition;
    private Quaternion homeRotation;

    private bool busy;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        homePosition = transform.position;
        homeRotation = transform.rotation;
    }

    private void Update()
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    public bool IsBusy()
    {
        return busy;
    }

    // NOW TAKES GRAB POINT (not shelf object)
    public void FulfillOrder(GameObject prefabToSpawn, Transform grabPoint)
    {
        if (busy)
            return;

        StartCoroutine(FulfillOrderRoutine(prefabToSpawn, grabPoint));
    }

    private IEnumerator FulfillOrderRoutine(GameObject prefabToSpawn, Transform grabPoint)
    {
        busy = true;

        // Stop if counter is full
        if (counter.IsFull())
        {
            busy = false;
            yield break;
        }

        // -----------------------
        // WALK TO SHELF GRAB POINT
        // -----------------------
        yield return MoveTo(grabPoint.position);

        // Face shelf
        yield return FaceTarget(grabPoint.position);

        // Pickup animation
        animator.SetTrigger("PickUp");
        yield return new WaitForSeconds(pickupDuration);

        // Spawn item in hand
        GameObject heldItem = Instantiate(prefabToSpawn);

        heldItem.transform.SetParent(handPoint);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        // -----------------------
        // WALK TO COUNTER
        // -----------------------
        yield return MoveTo(counterPoint.position);

        // Face counter
        yield return FaceTarget(counterPoint.position);

        // Small pause for polish
        yield return new WaitForSeconds(0.25f);

        // Place item
        heldItem.transform.SetParent(null);

        if (!counter.TryPlace(heldItem))
        {
            Destroy(heldItem);
        }

        // -----------------------
        // RETURN HOME
        // -----------------------
        yield return MoveTo(homePosition);

        yield return RotateTo(homeRotation);

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

    private IEnumerator FaceTarget(Vector3 targetPosition)
    {
        agent.updateRotation = false;

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
        {
            agent.updateRotation = true;
            yield break;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                8f * Time.deltaTime);

            yield return null;
        }

        transform.rotation = targetRotation;
        agent.updateRotation = true;
    }

    private IEnumerator RotateTo(Quaternion targetRotation)
    {
        agent.updateRotation = false;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                5f * Time.deltaTime);

            yield return null;
        }

        transform.rotation = targetRotation;
        agent.updateRotation = true;
    }
}