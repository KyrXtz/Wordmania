using PolyAndCode.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollManager : MonoBehaviour, IRecyclableScrollRectDataSource
{
    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;
    [SerializeField]
    private int _dataLength;
    int passedLet = 0;
    public LevelManager lvlManager;


    //Dummy data List
    //Recyclable scroll rect's data source must be assigned in Awake.
    private void Awake()
    {
        //
        //getDataSourceFromFile(lastLet);
        _recyclableScrollRect.DataSource = this;
    }
    public void ReInit(int let)
    {
        getDataSourceFromFile(let);
        _recyclableScrollRect.ReloadData(this);
    }
    void getDataSourceFromFile(int let)
    {
        passedLet = let;
        var count =lvlManager.getLevelsNoOfLetters(passedLet);
        _dataLength = Convert.ToInt32(count);
    }
    
    /// <summary>
    /// Data source method. return the list length.
    /// </summary>
   
    // <summary>
    /// Called for a cell every time it is recycled
    /// Implement this method to do the necessary cell configuration.
    /// </summary>
    public void SetCell(ICell cell, int index)
    {
        //Casting to the implemented Cell
        var item = cell as ScrollCell;
        var txtAsset = Resources.Load<TextAsset>("Data/" + "l" + passedLet + "/" + "l" + passedLet + "s" + index);
        //item.ConfigureCell(_contactList[index], index);
        item.ConfigureCell(index, txtAsset == null,passedLet,lvlManager);

    }

    public int GetItemCount()
    {
        return _dataLength;
    }
}
