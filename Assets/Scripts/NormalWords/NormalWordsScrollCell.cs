using UnityEngine;
using UnityEngine.UI;
using PolyAndCode.UI;
using Gravitons.UI.Modal;
using TMPro;

//Cell class for demo. A cell in Recyclable Scroll Rect must have a cell class inheriting from ICell.
//The class is required to configure the cell(updating UI elements etc) according to the data during recycling of cells.
//The configuration of a cell is done through the DataSource SetCellData method.
//Check RecyclableScrollerDemo class
public class NormalWordsScrollCell : MonoBehaviour, ICell
{
    public TextMeshProUGUI Word;
    public GameObject InfoButton;
    LevelManager lvlManager;
    private void Start()
    {
        //Can also be done in the inspector
        InfoButton.AddComponent<Button>().onClick.AddListener(InfoButtonClicked);

    }

    //This is called from the SetCell method in DataSource
    public void ConfigureCell(string word,LevelManager _lvlManager)
    {
        lvlManager = _lvlManager;
        Word.text = word;
        InfoButton.gameObject.GetComponent<InfoButtonClicked>().wordToSearch = word;
    }
    public void InfoButtonClicked()
    {

        InfoButton.gameObject.GetComponent<InfoButtonClicked>().InfoButtonClickedMethod(lvlManager.WordSearchView);
    }
}
