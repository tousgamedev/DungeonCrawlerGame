﻿using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(ControllerRaycaster))]
[RequireComponent(typeof(ControllerStateMachine))]
public class CrawlerController : MonoBehaviour, IController
{
    public ControllerRaycaster Raycaster => raycaster;
    public float MoveDuration => moveDuration;
    public bool CanClimbDown => canClimbDown;
    public bool CanClimbHorizontally => canClimbHorizontally;
    public bool CanClimbAcrossGap => canClimbAcrossGap;
    
    [Header("Movement")]
    [SerializeField] private float headHeight = 2.0f;
    [SerializeField] private float moveDistance = 4.0f;
    [SerializeField] private float moveDuration = 0.5f;
    [SerializeField] private float rotateDuration = 0.3f;
    [SerializeField] private int maxInclineAngle = 35;
    [SerializeField] [Range(0, 1f)] private float slopeCheckOffset = .1f;

    [Header("Obstacle Rebound")]
    [SerializeField] private float reboundDuration = 0.3f;

    [Header("Climbing")]
    [SerializeField] private bool canClimbDown = true;
    [SerializeField] private bool canClimbHorizontally = true;
    [SerializeField] private bool canClimbAcrossGap;

    [Header("Falling")]
    [SerializeField] private float fallDuration = 0.15f;

    private ControllerRaycaster raycaster;
    private Transform agentTransform;
    private Vector3 previousPosition;

    private IEnumerator movementCoroutine;

    private void Awake()
    {
        agentTransform = transform;
        if (!TryGetComponent(out raycaster))
        {
            raycaster = gameObject.AddComponent<ControllerRaycaster>();
        }

        raycaster.InitializeClimbCheckValues(headHeight, moveDistance);
    }

    public void OnEnable()
    {
        if (raycaster.IsOnGround(out RaycastHit hit))
        {
            transform.position = AdjustForHeadHeightPosition(hit.point);
        }
    }

    private Vector3 AdjustForHeadHeightPosition(Vector3 position)
    {
        position.y += headHeight;
        return position;
    }

    public void BumpAgent(Action idleStateCallback)
    {
        StopMovementCoroutine();
        StartMovementCoroutine(BumpAgentCo(idleStateCallback));
    }
    
    private IEnumerator BumpAgentCo(Action idleStateCallback)
    {
        Vector3 bumpPosition = agentTransform.position;

        float elapsedTime = 0;
        while (elapsedTime <= reboundDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.Clamp01(elapsedTime / reboundDuration);
            agentTransform.position = Vector3.Lerp(bumpPosition, previousPosition, step);

            yield return null;
        }

        agentTransform.position = previousPosition;
        idleStateCallback?.Invoke();
    }
    
    public void MoveAgent(Vector3 direction, Action groundCheckCallback)
    {
        StopMovementCoroutine();

        Vector3 worldSpaceDirection = agentTransform.TransformDirection(direction);
        float distance = GetMoveDistance(worldSpaceDirection);

        Vector3 endPosition = GetMovementPosition(worldSpaceDirection, distance);
        float slopeCheckAdjust = GetDistanceCheckPoint(endPosition.y);
        Vector3 midPosition = GetMovementPosition(worldSpaceDirection, distance * slopeCheckAdjust);
        
        previousPosition = agentTransform.position;
        StartMovementCoroutine(MoveAgentCo(midPosition, endPosition, groundCheckCallback));
    }

    private float GetMoveDistance(Vector3 direction)
    {
        return direction == Vector3.up || direction == Vector3.down
            ? raycaster.CalculateClimbDistance(direction)
            : moveDistance;
    }
    
    private Vector3 GetMovementPosition(Vector3 direction, float checkDistance)
    {
        Vector3 raycastOrigin = raycaster.GetMoveRaycastOrigin(direction, checkDistance);
        Vector3 newPosition;
        if (!raycaster.IsWalkableAngle(raycastOrigin, maxInclineAngle, out RaycastHit hit))
        {
            newPosition = agentTransform.position + direction * checkDistance;
            newPosition.x = Mathf.RoundToInt(newPosition.x);
            newPosition.z = Mathf.RoundToInt(newPosition.z);
            return newPosition;
        }

        newPosition = AdjustForHeadHeightPosition(hit.point);
        newPosition.x = Mathf.RoundToInt(hit.point.x);
        newPosition.z = Mathf.RoundToInt(hit.point.z);
        return newPosition;
    }
    
    private float GetDistanceCheckPoint(float height)
    {
        bool isDownSlope = height < agentTransform.position.y;
        return .5f + (isDownSlope ? -slopeCheckOffset : slopeCheckOffset);
    }

    private IEnumerator MoveAgentCo(Vector3 halfPosition, Vector3 endPosition, Action groundCheckCallback)
    {
        Vector3 currentPosition = agentTransform.position;
        float halfWalkDuration = moveDuration * .5f;
        
        float elapsedTime = 0;
        while (elapsedTime < halfWalkDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.Clamp01(elapsedTime / halfWalkDuration);
            agentTransform.position = Vector3.Lerp(currentPosition, halfPosition, step);

            yield return null;
        }

        elapsedTime = 0;
        while (elapsedTime < halfWalkDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.Clamp01(elapsedTime / halfWalkDuration);
            agentTransform.position = Vector3.Lerp(halfPosition, endPosition, step);

            yield return null;
        }

        agentTransform.position = endPosition;
        groundCheckCallback?.Invoke();
    }

    public void DropAgent(Action groundCheckCallback, Action playYellCallback)
    {
        StopMovementCoroutine();

        Vector3 newPosition;
        var fallMultiplier = 1f;
        if (raycaster.IsSafeFall(out RaycastHit hit))
        {
            newPosition = new Vector3(hit.point.x, hit.point.y + headHeight, hit.point.z);
            fallMultiplier = Vector3.Distance(agentTransform.position, newPosition) * fallDuration;
        }
        else
        {
            newPosition = agentTransform.position + agentTransform.TransformDirection(Vector3.down) * moveDistance;
            playYellCallback?.Invoke();            
        }

        StartMovementCoroutine(DropAgent(newPosition, fallMultiplier, groundCheckCallback));
    }

    private IEnumerator DropAgent(Vector3 endPosition, float fallDurationMultiplier, Action groundCheckCallback)
    {
        if (fallDurationMultiplier == 0)
            yield break;

        Vector3 currentPosition = agentTransform.position;
        float adjustedFallDuration = fallDuration * fallDurationMultiplier;

        var step = 0f;
        while (step < 1.0f)
        {
            step += Time.deltaTime / adjustedFallDuration;
            agentTransform.position = Vector3.Lerp(currentPosition, endPosition, step);

            yield return null;
        }

        agentTransform.position = endPosition;
        groundCheckCallback?.Invoke();
    }

    public void RotateAgent(Quaternion angleChange, Action groundCheckCallback)
    {
        StopMovementCoroutine();
        StartMovementCoroutine(RotateAgentCo(angleChange, groundCheckCallback));
    }

    private IEnumerator RotateAgentCo(Quaternion angleChange, Action groundCheckCallback)
    {
        Quaternion startRotation = agentTransform.rotation;
        Quaternion targetRotation = startRotation * angleChange;

        float elapsedTime = 0;
        while (elapsedTime < rotateDuration)
        {
            elapsedTime += Time.deltaTime;
            float step = Mathf.Clamp01(elapsedTime / rotateDuration);
            agentTransform.rotation = Quaternion.Slerp(startRotation, targetRotation, step);

            yield return null;
        }

        agentTransform.rotation = targetRotation;
        groundCheckCallback?.Invoke();
    }

    private void StartMovementCoroutine(IEnumerator coroutine)
    {
        movementCoroutine = coroutine;
        StartCoroutine(movementCoroutine);
    }
    
    private void StopMovementCoroutine()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
        }
    }
}