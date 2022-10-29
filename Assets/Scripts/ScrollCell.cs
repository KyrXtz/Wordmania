using UnityEngine;
using UnityEngine.UI;
using PolyAndCode.UI;
using Gravitons.UI.Modal;

//Cell class for demo. A cell in Recyclable Scroll Rect must have a cell class inheriting from ICell.
//The class is required to configure the cell(updating UI elements etc) according to the data during recycling of cells.
//The configuration of a cell is done through the DataSource SetCellData method.
//Check RecyclableScrollerDemo class
public class ScrollCell : MonoBehaviour, ICell
{
    LevelManager _lvlManager;
    int _let = 0;
    bool _hasToDownload;
    bool _notUnlocked;

    //UI
    public Image downloadIcon;
    public Image lockIcon;
    public Image clearedImg;
    public Image notClearedImg;
    public Text idLabel;
    private int _cellIndex;

    private void Start()
    {
        //Can also be done in the inspector
        
        GetComponent<Button>().onClick.AddListener(ButtonListener);
    }

    //This is called from the SetCell method in DataSource
    public void ConfigureCell(int cellIndex,bool haveToDownload,int let,LevelManager lvlManager)
    {
        _cellIndex = cellIndex;

        //nameLabel.text = haveToDownload?"Download":"Available";
        // nameLabel.transform.parent.gameObject.GetComponent<Image>().sprite = haveToDownload ? lvlManager.LetterGridStates[2] : null;
        downloadIcon.gameObject.SetActive(haveToDownload);
        lockIcon.gameObject.SetActive(!downloadIcon.gameObject.activeSelf && PrefsWrapper.GetInt("isLetterUnlocked:"+let) != 1);
        clearedImg.gameObject.SetActive((PrefsWrapper.GetInt("l" + let + "s" + _cellIndex.ToString()) == 1));
        notClearedImg.gameObject.SetActive((PrefsWrapper.GetInt("l" + let + "s" + _cellIndex.ToString()) == 0));

        // clearedImg.text = PrefsWrapper.GetInt("l"+let+"s"+_cellIndex.ToString())==1?"Passed":"NotPassed";
        idLabel.text = (_cellIndex+1).ToString();
        _lvlManager = lvlManager;
        _let = let;
        _hasToDownload = haveToDownload;
        _notUnlocked = lockIcon.gameObject.activeSelf;
    }


    private void ButtonListener()
    {
        if (_hasToDownload)
        {
            ModalManager.Show("Oops!", "This level is not available right now!", _lvlManager.iconsForModals[0], new[] {new ModalButton(){Text = "Ok.."}
                 }); ;
            
        }else if (_notUnlocked)
        {
            var howManyLeft = "";
            for (int i = 4; i < _let; i++)
            {
                var totalLvlsNeeded = Mathf.Min(_lvlManager.getLevelsNoOfLetters(i), 10 * (_let - i));
                var c = totalLvlsNeeded - PrefsWrapper.GetInt(i + "LetterStagesCleared");
                if (c <= 0) continue;
                howManyLeft += c + " levels of " + i + " letters\n";
            }
            ModalManager.Show("Oops!", "This level is locked!\nYou must clear\n"+howManyLeft, _lvlManager.iconsForModals[0], new[] {new ModalButton(){Text = "Ok.."}
                 }); ;
        }
        else 
        {
            //_lvlManager.showLevelSelect(false);
            _lvlManager.setLevel(_let, _cellIndex);
            _lvlManager.showLevelSelect(false);
            _lvlManager.playButton();
        }
        // Debug.Log("Index : " + _cellIndex + ", Name : " + _contactInfo.Name + ", Gender : " + _contactInfo.Gender);
    }
}
