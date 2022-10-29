using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActiveWhenAdReady : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject ad;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       // this.gameObject.transform.GetChild(0).transform.gameObject.SetActive(ad.GetComponent<Rewarded>().isReady);
    }
}
