using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMovedToPreviewGrid : MonoBehaviour
{
    public GameObject PreviewGrid;
    public Vector2  positionToGo;
    public Vector2 startingPos;
    public static bool isExtraLetter = false;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.transform.SetParent(PreviewGrid.transform);
       // startingPos = this.gameObject.GetComponent<RectTransform>().anchoredPosition;
       // this.gameObject.GetComponent<RectTransform>().anchoredPosition = positionToGo;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var pos = this.gameObject.GetComponent<RectTransform>().anchoredPosition;
        if (!isExtraLetter)
        {
            if (this.gameObject.transform.GetChild(0).gameObject.GetComponent<LerpColorRainbow>() != null)
            {
                Destroy(this.gameObject.transform.GetChild(0).gameObject.GetComponent<LerpColorRainbow>());
            }
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(pos, positionToGo, Time.deltaTime * 5) + new Vector2(Mathf.Sin(Time.time * 30), Mathf.Sin(Time.time));
        }
        else
        {
            if (this.gameObject.transform.GetChild(0).gameObject.GetComponent<LerpColorRainbow>() == null)
            {
                this.gameObject.transform.GetChild(0).gameObject.AddComponent<LerpColorRainbow>();
            }
            this.gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(pos, positionToGo, Time.deltaTime * 5) + new Vector2(Mathf.Sin(Time.time * 30)*4, Mathf.Sin(Time.time*10) *2);// *getParameter()
        }

    }
    float getParameter()
    {
        var x = this.transform.parent.childCount / 2;
        var sInd = this.transform.GetSiblingIndex() + 1;
        if(x%2 == 0)
        {
            if (sInd > x)
            {
                return x / sInd;
            }
            else if (sInd <= x)
            {
                return sInd / x;

            }
            else
            {
                return 0;
            }
        }
        else
        {
            if (sInd > x)
            {
                return x / sInd;
            }
            else if (sInd < x)
            {
                return sInd / x;

            }
            else
            {
                return 1;
            }
        }
        
    }
    /// <summary>
    /// PREPEI na kalestei prin ginei destroy to component!!!!! 
    /// </summary>
    /// <param name="set"></param>
    public static void setExtraLetter(bool set)
    {
        isExtraLetter = set;
        
    }
}
