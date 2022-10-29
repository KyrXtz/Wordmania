using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpColorRainbow : MonoBehaviour
{
    // Start is called before the first frame update
    static float lerpTime = 3f;
    Color32[] Colors =
    {
        new Color32(148,0,211,255),
        new Color32(75, 0, 130,255),
        new Color32(0, 0, 255,255),
        new Color32(0, 255, 0,255),
        new Color32(255, 255, 0,255),
        new Color32(255, 127, 0,255),
        new Color32(255, 0 , 0,255)
    };
    static int clrIndx = 0;
    static float t = 0f;

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
        bg.color = Color.white;
    }
}
