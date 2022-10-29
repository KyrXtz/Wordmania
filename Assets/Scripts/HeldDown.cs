using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldDown : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    public GameObject dbg;
    // Update is called once per frame
    float t = 0;
    void Update()
    {
        if (isDown)
        {
            t += Time.deltaTime;
        }
        else
        {
            t = 0;
        }
        if (t > 5)
        {
            Debug();
        }
    }
    bool isDown = false;
    public void OnPointerDown()
    {
        isDown = true;
    }
    public void OnPointerUp()
    {
        isDown = false;

    }
    public void Debug()
    {
        PrefsWrapper.SetInt("isLetterUnlocked:5", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:6", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:7", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:8", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:9", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:10", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:11", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:12", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:13", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:14", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:15", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:16", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:17", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:18", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:19", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:20", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:21", 1);
        PrefsWrapper.SetInt("isLetterUnlocked:22", 1);

        dbg.SetActive(true);

    }
}
