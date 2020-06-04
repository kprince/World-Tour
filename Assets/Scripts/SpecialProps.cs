using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialProps : MonoBehaviour
{
    Transform coin;
    private void Awake()
    {
        coin = transform.GetChild(0);
    }
    private void OnEnable()
    {
        StartCoroutine("UpDownSelf");
    }
    private void OnDisable()
    {
        StopCoroutine("UpDownSelf");
    }
    IEnumerator UpDownSelf()
    {
        bool moveUp = false;
        while (true)
        {
            yield return null;
            coin.localPosition += (moveUp ? Vector3.up : Vector3.down) * Time.deltaTime*0.5f/2;
            float upOffset = coin.localPosition.y - 0.39f;
            float downOffset = 0.65f - coin.localPosition.y;
            if (upOffset <= 0.02f)
                moveUp = true;
            else if (downOffset <= 0.02f)
                moveUp = false;
        }
    }
}
