using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinCollect : MonoBehaviour
{
    Transform[] allCoinsTrans;
    Image[] allCoinsImg;
    Vector3[] allCoinsOriginPos;
    Sprite cashSprite;
    Sprite goldSprite;
    private void Awake()
    {
        int childCount = transform.childCount;
        allCoinsTrans = new Transform[childCount];
        allCoinsImg = new Image[childCount];
        allCoinsOriginPos = new Vector3[childCount];
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            allCoinsTrans[i] = child;
            allCoinsImg[i] = child.GetComponent<Image>();
            allCoinsOriginPos[i] = child.localPosition;
        }
        goldSprite = Resources.Load<Sprite>("gold");
    }
    bool isGold = false;
    private void OnEnable()
    {
        if(cashSprite is null)
        {
            if (GameManager.Instance.GetShowExchange())
                cashSprite = Resources.Load<Sprite>("cashB");
            else
                cashSprite = Resources.Load<Sprite>("cashA");
        }
        isGold = GameManager.Instance.coinCollectIsGold;
        int count = allCoinsTrans.Length;
        for(int i = 0; i < count; i++)
        {
            allCoinsTrans[i].localPosition = Vector3.zero;
            allCoinsImg[i].sprite = isGold ? goldSprite : cashSprite;
        }
        StartCoroutine(FlyToTarget());
    }
    IEnumerator FlyToTarget()
    {
        Transform target;
        if (isGold)
            target = GameManager.Instance.goldTrans;
        else
            target = GameManager.Instance.cashTrans;
        if (target is null)
        {
            gameObject.SetActive(false);
            yield break;
        }
        int count = allCoinsTrans.Length;
        float time = 0;
        while (true)
        {
            yield return null;
            time += Time.deltaTime*4/2;
            for(int i = 0; i < count; i++)
            {
                allCoinsTrans[i].localPosition = Vector3.Lerp(Vector3.zero, allCoinsOriginPos[i], time);
            }
            if (time > 1)
                break;
        }
        yield return new WaitForSeconds(0.4f);
        Vector3 targetPos = target.position;
        int num = 0;
        while (true)
        {
            yield return null;
            for (int i = 0; i < count; i++)
            {
                if (i <= num)
                    allCoinsTrans[i].position = Vector3.Lerp(allCoinsTrans[i].position, targetPos, 0.1f);
            }
            num++;
            if (Vector3.Distance(allCoinsTrans[count - 1].position, targetPos) < 0.1f)
                break;
        }
        gameObject.SetActive(false);
    }
}
