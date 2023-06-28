using System;
using System.Collections;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
    public static CoroutineManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Utilities.Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void RunCoroutine(IEnumerator coroutine)
    {
        if (coroutine != null)
        {
            StartCoroutine(coroutine);
        }
    }
    
    public void HaltCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }
    
    public Coroutine RunCoroutineWithCallback(IEnumerator routine, Action callback)
    {
        return StartCoroutine(CoroutineWithCallback(routine, callback));
    }

    private IEnumerator CoroutineWithCallback(IEnumerator routine, Action callback)
    {
        yield return StartCoroutine(routine);
        callback?.Invoke();
    }
    
    public Coroutine RunChainCoroutines(params IEnumerator[] coroutines)
    {
        Coroutine previousCoroutine = null;
        foreach (IEnumerator routine in coroutines)
        {
            previousCoroutine = StartCoroutine(ChainCoroutine(routine, previousCoroutine));
        }

        return previousCoroutine;
    }

    private IEnumerator ChainCoroutine(IEnumerator routine, Coroutine previousCoroutine)
    {
        yield return previousCoroutine;
        yield return StartCoroutine(routine);
    }
}
