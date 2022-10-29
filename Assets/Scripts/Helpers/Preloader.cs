using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Preloader : MonoBehaviour
{
    public Slider loadingSlider;
    public TextMeshProUGUI percent;
    private void Start()
    {
        StartCoroutine(LoadAsync());
    }



    IEnumerator LoadAsync()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Main");
        asyncLoad.allowSceneActivation = false;
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.05f);
                percent.text = "100%";
                loadingSlider.value = 1.0f;
                yield return new WaitForSeconds(0.05f);
                asyncLoad.allowSceneActivation = true;
                break;
            }
            else
            {
                percent.text = System.Math.Round(asyncLoad.progress * 100, 2) + "%";
                loadingSlider.value = asyncLoad.progress;
            }
        }
        yield return null;


    }


}
