using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRotation : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DebugThis());
    }
    IEnumerator DebugThis()
    {
        while (true)
        {
            yield return new WaitForSeconds(Time.timeScale);
            Debug.LogError(transform.rotation);
        }
    }
}
