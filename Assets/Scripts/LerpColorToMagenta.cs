using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpColorToMagenta : MonoBehaviour
{
    // Start is called before the first frame update
    float lerpTime = 2f;
    Color32[] Colors =
    {
        Color.magenta,
        Color.white
    };
    int clrIndx = 0;
    float t = 0f;

    Image bg;
    void Start()
    {
        bg = this.gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        bg.color = Color.Lerp(bg.color, Colors[clrIndx], Time.deltaTime * lerpTime);
        t = Mathf.Lerp(t, 1f, lerpTime * Time.deltaTime);
        if (t > 0.9f)
        {
            t = 0f;
            clrIndx++;
            clrIndx = (clrIndx >= Colors.Length) ? 0 : clrIndx;
        }
    }

    public void OnDestroy()
    {
        try
        {
            bg.color = Color.white;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e);
        }
    }
}
