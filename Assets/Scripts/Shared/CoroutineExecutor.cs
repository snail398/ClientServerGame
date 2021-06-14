using System.Collections;
using UnityEngine;


public class CoroutineExecutor : MonoBehaviour, ICoroutineExecutor
{
}

public interface ICoroutineExecutor
{
    Coroutine StartCoroutine(IEnumerator routine);
    void StopCoroutine(Coroutine routine);
}
