using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class SlidingDoor : MonoBehaviour, IObserver
{
    private enum OpenDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [SerializeField] private float openDuration = 2f;
    [SerializeField] private float closeDuration = 2f;
    [SerializeField] private OpenDirection openDirection;
    [SerializeField] private float travelDistance = 4f;
    [SerializeField] private List<GameObject> interactablesList = new();

    private HashSet<ISubject> interactablesSet = new();
    private Coroutine doorActivationRoutine;

    private Vector3 closePosition;
    private Vector3 openPosition;
    private bool isOpening;

    private void Awake()
    {
        PopulateInteractableSubjectSet();

        closePosition = transform.localPosition;
        openPosition = GetOpenPosition();
    }

    private void PopulateInteractableSubjectSet()
    {
        foreach (GameObject interactionObject in interactablesList)
        {
            if (interactionObject.TryGetComponent(out IInteractable _) &&
                interactionObject.TryGetComponent(out ISubject subject))
            {
                interactablesSet.Add(subject);
            }
        }
    }

    private Vector3 GetOpenPosition()
    {
        return openDirection switch
        {
            OpenDirection.Up => closePosition + transform.up * travelDistance,
            OpenDirection.Down => closePosition - transform.up * travelDistance,
            OpenDirection.Right => closePosition + transform.right * travelDistance,
            OpenDirection.Left => closePosition - transform.right * travelDistance,
            _ => openPosition
        };
    }
    
    private void OnEnable()
    {
        RegisterObserver();
    }

    public void RegisterObserver()
    {
        foreach (ISubject interactable in interactablesSet)
        {
            interactable.RegisterObserver(this);
        }
    }

    private void OnDisable()
    {
        DeregisterObserver();
    }

    public void DeregisterObserver()
    {
        foreach (ISubject interactable in interactablesSet)
        {
            interactable.DeregisterObserver(this);
        }
    }

    public void Alert()
    {
        ActivateDoor();
    }

    public string GetName()
    {
        return gameObject.name;
    }

    private void ActivateDoor()
    {
        if (doorActivationRoutine != null)
        {
            StopCoroutine(doorActivationRoutine);
        }

        isOpening = !isOpening;

        if (isOpening)
        {
            OpenDoor();
        }
        else
        {
            CloseDoor();
        }
    }

    private void OpenDoor()
    {
        doorActivationRoutine = StartCoroutine(ActivateDoor(openPosition, openDuration));
    }

    private void CloseDoor()
    {
        doorActivationRoutine = StartCoroutine(ActivateDoor(closePosition, closeDuration));
    }

    private IEnumerator ActivateDoor(Vector3 endPosition, float activationTime)
    {
        Vector3 startPosition = transform.localPosition;
        float remainingDistance = Vector3.Distance(startPosition, endPosition);
        float adjustedActivationTime =
            activationTime * (remainingDistance / Vector3.Distance(closePosition, openPosition));

        var elapsedTime = 0f;
        while (elapsedTime < adjustedActivationTime)
        {
            float normalizedTime = elapsedTime / adjustedActivationTime;
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, normalizedTime);
            yield return null;

            elapsedTime += Time.deltaTime;
        }

        transform.localPosition = endPosition;
        doorActivationRoutine = null;
    }

#if UNITY_EDITOR
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawCube(transform.position, new Vector3(.5f, .5f, .5f));

        Handles.color = Color.grey;
        Gizmos.color = Color.green;

        foreach (GameObject notifier in interactablesList)
        {
            Vector3 position = notifier.transform.position;

            Handles.DrawLine(transform.position, position);
            Gizmos.DrawCube(position, new Vector3(.2f, .2f, .2f));
        }
    }
#endif
}