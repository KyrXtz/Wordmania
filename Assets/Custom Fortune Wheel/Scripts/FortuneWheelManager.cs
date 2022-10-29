using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class FortuneWheelManager : MonoBehaviour
{
    public FortuneWheel fortuneWheel;
    public Text resultLabel;
    void Awake()
    {
        
    }
    public void Spin()
    {
        StartCoroutine(SpinCoroutine());
    }
    IEnumerator SpinCoroutine()
    {
        yield return StartCoroutine(fortuneWheel.StartFortune());

        if(resultLabel == null) yield break;
        resultLabel.text = fortuneWheel.GetLatestResult();

        print(fortuneWheel.GetLatestResult());

    }

}
