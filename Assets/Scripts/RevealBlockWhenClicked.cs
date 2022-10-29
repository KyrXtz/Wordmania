using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevealBlockWhenClicked : MonoBehaviour
{
    public static bool isAllowedForClick = false;
    public LevelManager lvlManager;
    Image bg;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.AddComponent<Button>().onClick.AddListener(onClick);
        bg = this.gameObject.transform.GetChild(2).GetComponent<Image>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isAllowedForClick)
        {
            var hid = this.gameObject.transform.GetChild(2).gameObject;
            if (hid.activeSelf)
            {
                if(hid.GetComponent<LerpColorToMagenta>() == null)
                {
                    hid.AddComponent<LerpColorToMagenta>();
                }
            }
        }
        else
        {
            var hid = this.gameObject.transform.GetChild(2).gameObject;
            if (hid.activeSelf)
            {
                if (hid.GetComponent<LerpColorToMagenta>() != null)
                {
                    Destroy(hid.GetComponent<LerpColorToMagenta>());
                }
            }
        }
        
    }
    void onClick()
    {
        if (!isAllowedForClick) return;
        // Destroy(this.gameObject.transform.GetChild(2).gameObject.GetComponent<LerpColorToMagenta>());
        isAllowedForClick = false;
        lvlManager.actuallyRevealLetterByChoosing(this.gameObject);
    }
}
