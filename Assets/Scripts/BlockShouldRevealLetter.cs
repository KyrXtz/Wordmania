using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockShouldRevealLetter : MonoBehaviour
{
    // Start is called before the first frame update
    public string word;
     List<string> wordChars = new List<string>();
    int indx = 0;
    public GameObject PreviewGrid;
    public GameObject Info;
    bool wordDone = false;
    bool markForRemovalOfPendingScript = false;
    void Start()
    {
        foreach (var c in word)
        {
            wordChars.Add(c.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// returns 1 an kanei reveal, 0 an oxi , 2 an oloklirwthike h leksi
    /// <para>
    /// an dwseis letter = "" , kanei remove ta lerps.
    /// </para>
    /// </summary>
    /// <param name="letter"></param>
    /// <returns></returns>
    public int checkIfShouldRevealLetter(string letter)
    {
        markForRemovalOfPendingScript = false;
        if (wordChars[indx] == letter && PreviewGrid.transform.childCount == indx && !wordDone)
        {
            this.gameObject.transform.GetChild(word.Length-1-indx).transform.GetChild(2).gameObject.SetActive(false);
            //this.gameObject.transform.GetChild(word.Length - 1 - indx).GetChild(0).gameObject.AddComponent<LerpColorToGreen>();

            indx += 1;
            if (indx == word.Length)
            {
                removePendingScript();
                setDone();
                return 2;
            }
            else
            {
                addPendingScript(this.gameObject.transform.GetChild(word.Length - indx).transform.GetChild(0).gameObject); //oxi ( word.Length - 1 - indx) epeidh exei ginei indx+=1
            }
            return 1;
        }
        else if (wordChars[indx] != letter && PreviewGrid.transform.childCount == indx && !wordDone)
        {
            markForRemovalOfPendingScript = true;
            return 0;

        }
        else
        {
            return 0;
        }
    }
    /// <summary>
    /// prepei na kalestei afoy elegxthoyn ola ta words toy game grid
    /// <para>
    /// an exei ginei reveal kapio letter kai ayto to object exei ginei mark gia removaltoyscript , to afairei
    /// </para>
    /// <para>
    /// an exei ginei mark gia removal alla den exei emfanistei gramma, den kaleitai
    /// </para>
    /// </summary>
    public void removePendingScriptIfMarkedForRemoval()
    {
        if (markForRemovalOfPendingScript)
        {
            removePendingScript();
            indx = 0;
        }
    }
    public bool checkIfWordIsFoundByRevealButton()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            var isActive =this.gameObject.transform.GetChild(i).GetChild(2).gameObject.activeSelf;
            if (isActive)
            {
                return false;
            }
        }
        setDone();
        Debug.Log("WORD FOUND VY REVEAL");
        return true;
    }
    void addGreenScript()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            this.gameObject.transform.GetChild(i).GetChild(0).gameObject.AddComponent<LerpColorToGreen>();
        }
    }
    void addPendingScript(GameObject block)
    {
        if(block.GetComponent<LerpColorToOrange>() == null)
        {
            block.AddComponent<LerpColorToOrange>();

        }
    }
    void removePendingScript()
    {
        for (int i = 0; i < this.gameObject.transform.childCount; i++)
        {
            if (this.gameObject.transform.GetChild(i).GetChild(2).gameObject.activeSelf) continue; //an einai akoma hidden, dld den exei lerpToOrange
            var c = this.gameObject.transform.GetChild(i).GetChild(0).gameObject.GetComponent<LerpColorToOrange>();
            if (c!=null) Destroy(c);         
        }
        markForRemovalOfPendingScript = false;
    }
    public void resetIndex()
    {
        removePendingScript();
        indx = 0;
    }
    /// <summary>
    /// tha kanei to index oso to size poy menei sto preview grid. AN einai <0 , tote indx = 0
    /// </summary>
    public void IndexMinus()
    {
        if (indx > 0)
        {
            var c = this.gameObject.transform.GetChild(word.Length - indx).GetChild(0).gameObject.GetComponent<LerpColorToOrange>();
            if (c != null) Destroy(c);
            indx -= 1;
            
        }
        
    }
    void setDone()
    {
        indx = 0;
        wordDone = true;
        addGreenScript(); revealInfoButton();
    }
    void revealInfoButton()
    {
        Info.SetActive(true);
    }
}
