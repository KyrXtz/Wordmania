using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinAcquire : MonoBehaviour
{
    // Start is called before the first frame update
    RectTransform rect;
    Vector2 posToGoEnd;
    void Start()
    {
        rect = this.gameObject.GetComponent<RectTransform>();
        posToGoEnd = Vector2.zero;
        StartCoroutine(destroyThis());
    }

    // Update is called once per frame
    void Update()
    {
        rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, posToGoEnd, Time.deltaTime * 3);

    }
    IEnumerator destroyThis()
    {
        
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}
