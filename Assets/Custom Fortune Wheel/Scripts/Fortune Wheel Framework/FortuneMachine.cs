using System.Collections;
using UnityEngine;

/// <summary>
/// The parent class that FortuneWheel that inherits from and implements its functions.
/// </summary>
public abstract class FortuneMachine : MonoBehaviour
{
    /// <summary> 
    ///If you want to double the instances number of the elements to get the selection chance doubled. Set to false by default.
    /// </summary>
    public bool doubleChance = false;
    /// <summary>
    /// Abstract function responsible for instantiating one of the FortuneMachine and its components.
    /// </summary>
    public abstract void CreateFortune();

    /// <summary>
    /// Starts moving the FortneMachine.
    /// The machine then starts to decelarate gradually until it stops on the selected choice.
    /// <br/>
    /// This function works asynchronously so call it in a coroutine.
    /// <br/>
    /// <example> Exmaple:
    /// <code>
    ///
    ///    IEnumerator CoroutineExamle()
    ///    { 
    ///        <br/>
    ///        &emsp; yield return StartCoroutine( StartFortune() );
    ///        <br/>
    ///    }
    /// </code>
    /// </example>
    /// </summary>  
    public abstract IEnumerator StartFortune();
    /// <summary>
    /// Get the label result of the latest fortune spin.
    /// <br/>
    /// You can use if after you make at least one spin.
    /// </summary>
    /// <returns>
    /// The label of the result.
    /// </returns>
    public abstract string GetLatestResult();
    /// <summary>
    /// Get the ID result of the latest fortune spin.
    /// <br/>
    /// You can use if after you make at least one spin.
    /// </summary>
    /// <returns>
    /// The ID of the result.
    /// </returns>
    public abstract int GetLatestResultID();
}
