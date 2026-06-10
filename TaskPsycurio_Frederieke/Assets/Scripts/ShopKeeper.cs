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

        agent.angularSpeed = 0f;
        agent.updateRotation = false;
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

    public void RemoveCounterItem(CounterItem item)
    {
        if (busy || item == null)
            return;

        StartCoroutine(RemoveCounterItemRoutine(item));
    }

    private IEnumerator FulfillOrderRoutine(GameObject prefabToSpawn, Transform grabPoint)
    {
        busy = true;

        if (counter.IsFull())
        {
            busy = false;
            yield break;
        }

        // Walk to shelf
        yield return MoveTo(grabPoint.position);
        yield return FaceTarget(grabPoint.position);

        animator.SetTrigger("PickUp");
        yield return new WaitForSeconds(pickupDuration);

        GameObject heldItem = Instantiate(prefabToSpawn);

        Vector3 worldScale = heldItem.transform.lossyScale;

        heldItem.transform.SetParent(handPoint, true);
        heldItem.transform.localPosition = Vector3.zero;
        heldItem.transform.localRotation = Quaternion.identity;

        heldItem.transform.localScale = new Vector3(
            worldScale.x / handPoint.lossyScale.x,
            worldScale.y / handPoint.lossyScale.y,
            worldScale.z / handPoint.lossyScale.z
        );

        // Walk to counter
        yield return MoveTo(counterPoint.position);
        yield return FaceTarget(counterPoint.position);

        yield return new WaitForSeconds(0.25f);

        heldItem.transform.SetParent(null, true);

        if (!counter.TryPlace(heldItem))
        {
            Destroy(heldItem);
        }

        // Return home
        yield return MoveTo(homePosition);

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

    private IEnumerator RemoveCounterItemRoutine(CounterItem item)
    {
        busy = true;

        Transform target = item.transform;

        // Walk to item
        yield return MoveTo(target.position);
        yield return FaceTarget(target.position);

        // Pickup animation
        animator.SetTrigger("PickUp");
        yield return new WaitForSeconds(pickupDuration);

        // Preserve scale before parenting
        Vector3 worldScale = item.transform.lossyScale;

        // Attach to hand
        item.transform.SetParent(handPoint, true);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;

        item.transform.localScale = new Vector3(
            worldScale.x / handPoint.lossyScale.x,
            worldScale.y / handPoint.lossyScale.y,
            worldScale.z / handPoint.lossyScale.z
        );

        // Free slot on counter
        counter.RemoveItem(item.Index);

        yield return new WaitForSeconds(0.25f);

        Destroy(item.gameObject);

        // Return home
        yield return MoveTo(homePosition);

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
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            yield break;

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
    }

    private IEnumerator RotateTo(Quaternion targetRotation, float speed = 5f)
    {
        while (Quaternion.Angle(transform.rotation, targetRotation) > 1f)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                speed * Time.deltaTime);

            yield return null;
        }

        transform.rotation = targetRotation;
    }
}