using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButtonClicked : MonoBehaviour
{
    public string wordToSearch;
    public void InfoButtonClickedMethod(GameObject WordSearchView)
    {
        GameObject.Find("AdManager").GetComponent<Banner>().ShowBanner();
        WordSearchView.GetComponent<SampleWebView>().Url = WordSearchView.GetComponent<SampleWebView>().defaultUrl + wordToSearch;
        var info = Instantiate(WordSearchView);
        Debug.Log(wordToSearch);
        Time.timeScale = 0;
    }
}
