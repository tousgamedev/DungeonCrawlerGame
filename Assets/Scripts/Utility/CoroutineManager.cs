using System;
using System.Collections;
using UnityEngine;

public class CoroutineManager : ManagerBase<CoroutineManager>
{
#pragma warning disable CS0108, CS0114
    private void Awake()
#pragma warning restore CS0108, CS0114
    {
        base.Awake();
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
