using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropsManager : MonoBehaviour
{
    // Start is called before the first frame update
    public LevelManager levelManager;
    public List<NestedList> objectsToShowList;
    int activeLetter = -1;
    // Update is called once per frame
    void Update()
    {
        var l = levelManager.getIndexOfActiveBg();
        if (activeLetter != l)
        {
            for (int i = levelManager.startL - 4; i <= levelManager.endL - 4; i++)
            {
                if (i == l) ActivateObjects(objectsToShowList[i]);
                else DeactivateObjects(objectsToShowList[i]);
            }
        }
        activeLetter = l;
        
    }
    void DeactivateObjects(NestedList objects)
    {
        objects.List.ForEach(o=>o.SetActive(false));     
    }
    void ActivateObjects(NestedList objects)
    {
        objects.List.ForEach(o => o.SetActive(true));
    }
}
[System.Serializable]
public class NestedList
{
    public List<GameObject> List;
}
