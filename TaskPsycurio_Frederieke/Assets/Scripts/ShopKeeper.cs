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

        agent.angularSpeed = 0f;        // IMPORTANT: we fully control rotation now
        agent.updateRotation = false;    // IMPORTANT
        agent.acceleration = 12f;
        agent.stoppingDistance = 0.2f;
        agent.autoBraking = true;
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

    public void FulfillOrder(GameObject prefabToSpawn, Transform grabPoint)
    {
        if (busy)
            return;

        StartCoroutine(FulfillOrderRoutine(prefabToSpawn, grabPoint));
    }

    private IEnumerator FulfillOrderRoutine(GameObject prefabToSpawn, Transform grabPoint)
    {
        busy = true;

        if (counter.IsFull())
        {
            busy = false;
            yield break;
        }

        // -----------------------
        // WALK TO SHELF
        // -----------------------
        yield return MoveTo(grabPoint.position);

        // Face shelf (intentional action)
        yield return FaceTarget(grabPoint.position);

        // Pickup
        animator.SetTrigger("PickUp");
        yield return new WaitForSeconds(pickupDuration);

        GameObject heldItem = Instantiate(prefabToSpawn);

        heldItem.transform.SetParent(handPoint);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        // -----------------------
        // WALK TO COUNTER
        // -----------------------
        yield return MoveTo(counterPoint.position);

        // Face counter (intentional action)
        yield return FaceTarget(counterPoint.position);

        yield return new WaitForSeconds(0.25f);

        heldItem.transform.SetParent(null);

        if (!counter.TryPlace(heldItem))
        {
            Destroy(heldItem);
        }

        // -----------------------
        // RETURN HOME
        // -----------------------
        yield return MoveTo(homePosition);

        // Face camera when returning home (important fix)
        Camera cam = Camera.main;
        if (cam != null)
        {
            yield return FaceTarget(cam.transform.position);
        }
        else
        {
            yield return RotateTo(homeRotation);
        }

        busy = false;
    }

    private IEnumerator MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
        agent.updateRotation = false;

        Vector3 direction = destination - transform.position;
        direction.y = 0f;

        //  immediate initial facing (this is the key change)
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        while (true)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.01f)
                        break;
                }
            }

            //  continuously re-aim toward final destination (not velocity, not steering)
            Vector3 toTarget = destination - transform.position;
            toTarget.y = 0f;

            if (toTarget.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(toTarget);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    10f * Time.deltaTime);
            }

            yield return null;
        }
    }

    private IEnumerator FaceTarget(Vector3 targetPosition, float speed = 5f)
    {
        agent.updateRotation = false;

        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
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
                speed * Time.deltaTime);

            yield return null;
        }

        transform.rotation = targetRotation;
        agent.updateRotation = true;
    }

    private IEnumerator RotateTo(Quaternion targetRotation, float speed = 5f)
    {
        agent.updateRotation = false;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                speed * Time.deltaTime);

            yield return null;
        }

        transform.rotation = targetRotation;
        agent.updateRotation = true;
    }
}