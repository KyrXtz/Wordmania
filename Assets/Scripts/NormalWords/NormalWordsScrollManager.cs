using PolyAndCode.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NormalWordsScrollManager : MonoBehaviour , IRecyclableScrollRectDataSource
{
    [SerializeField]
    RecyclableScrollRect _recyclableScrollRect;
    [SerializeField]
    private int _dataLength;
    public LevelManager lvlManager;
    List<string> wrds = new List<string>();

    //Dummy data List
    //Recyclable scroll rect's data source must be assigned in Awake.
    private void Awake()
    {
        //
        //getDataSourceFromFile(lastLet);
        _recyclableScrollRect.DataSource = this;
    }
    public void ReInit()
    {
        wrds = PrefsWrapper.GetString("AllNormalWordsFound").Split("\n").Where(c=>c!="").ToList();
        wrds.Sort();
        _dataLength = wrds.Count();
        _recyclableScrollRect.ReloadData(this);
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
        var item = cell as NormalWordsScrollCell;
        //item.ConfigureCell(_contactList[index], index);
        item.ConfigureCell(wrds[index], lvlManager);

    }

    public int GetItemCount()
    {
        return _dataLength;
    }
}
