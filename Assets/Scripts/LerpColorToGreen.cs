using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LerpColorToGreen : MonoBehaviour
{
    // Start is called before the first frame update
    Image bg;
    void Start()
    {
        bg = this.gameObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        bg.color = Color.Lerp(bg.color, Color.green, Time.deltaTime );
    }
}
