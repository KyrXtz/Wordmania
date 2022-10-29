using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomInOut : MonoBehaviour
{
    // Start is called before the first frame update
    private void Start()
    {
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(
            -this.gameObject.transform.parent.gameObject.GetComponent<RectTransform>().rect.width / 2,
            this.gameObject.transform.parent.gameObject.GetComponent<RectTransform>().rect.height / 2);
    }
    // Update is called once per frame
    bool scale = true;
    void Update()
    {
        if (scale)
        {
            this.gameObject.transform.localScale += new Vector3(0.01f, 0.01f, 0.01f);
        }
        else
        {
            this.gameObject.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
        }
        if(this.gameObject.transform.localScale.x > 2 && scale) scale = false;
        if (!scale && this.gameObject.transform.localScale.x < 1) scale = true;
    }
}
