using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideThisIfPrefNotSet : MonoBehaviour
{
    // Start is called before the first frame update
    public string Pref;
    public bool reverse = false;
    List<GameObject> Children = new List<GameObject>();
    void Start()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            Children.Add(this.gameObject.transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (reverse)
        {
            if (PrefsWrapper.HasKey(Pref)) SetObjectsActive(Children, false);
            else SetObjectsActive(Children, true);
        }
        else
        {
            if (!PrefsWrapper.HasKey(Pref)) SetObjectsActive(Children, false);
            else SetObjectsActive(Children, true);
        }
    }

    void SetObjectsActive(List<GameObject> objects, bool active)
    {
        foreach (var obj in objects)
        {
            obj.SetActive(active);
        }
    }
}
