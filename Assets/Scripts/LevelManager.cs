using Gravitons.UI.Modal;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    public int startL = 4;
    public int endL = 22;
    int letters = 0;
    int stage = 0;
    int coins = 0;
    public int isReadyForInterstitial = 2;
    bool shouldReShuffle = false;
    public PlayGamesController PlayGamesController;
    public GameObject GameGridScrollViewViewport;
    public GameObject Coin;
    public Animator ExtraWordFoundAnim;
    public TextMeshProUGUI ExtraWordFoundText;
    public ScrollManager scrlManager;
    public NormalWordsScrollManager nrmlWordsscrlManager;
    public TextMeshProUGUI LevelTitle;
    public GameObject LevelSelect;
    public GameObject ExtraWordsView;
    public GameObject MarketView;
    public GameObject FortuneWheelView;
    public GameObject ExtraCoinsView;
    public GameObject SettingsView;
    public GameObject LeaderboardView;
    public TextMeshProUGUI ExtraWordsViewText;
    public TextMeshProUGUI ExtraWordsViewSubtitle;
    /// <summary>
    /// <para>
    /// 0 = warning
    /// </para>
    /// <para>
    /// 1 = cloud
    /// </para>
    /// <para>
    /// 2 = coin
    /// </para>
    /// <para>
    /// 3 = fortune wheel
    /// </para>
    /// <para>
    /// 4 = searching
    /// </para>
    /// <para>
    /// 5 = ad
    /// </para>
    /// <para>
    /// 6 = error
    /// </para>
    /// <para>
    /// 7 = gift
    /// </para>
    /// <para>
    /// 8 = eye reveal
    /// </para>
    /// <para>
    /// 9 = coins
    /// </para>
    /// <para>
    /// 10 = tick
    /// </para>
    /// </summary>
    public List<Sprite> iconsForModals;
    public AudioManager audioManager;
    public TextMeshProUGUI coinsNumberUI;
    public PlayerManager playerManager;
    public GameObject AdManager;
    public GameObject commingSoon;
    public Animator Game;
    public Animator Intermission;
    public List<GameObject> objectsToHide;
    public List<GameObject> objectsToShow;
    public List<Button> buttonsToDisableWhenLevelDone;
    public Image GameBg;
    public List<Sprite> GameBGs;
    public GameObject GameGrid;
    public GameObject LetterGrid;
    public GameObject PreviewGrid;
    public GameObject CancelButton;
    public List<Sprite> LetterGridStates;
    Vector2 gameGridStartingSize;
    float gridSemiWidth, gridSemiHeight, letterGridSemiWidth, letterGridSemiHeight,previewGridSemiWidth, previewGridSemiHeight;
    float blockX, blockY;
    float blockPaddingX, blockPaddingY;
    float sizeModifier=1;
    public GameObject Block;
    public GameObject Info;
    public GameObject WordSearchView;
    List<GameObject> wordObjects = new List<GameObject>();
    List<List<List<string>>> dicts = new List<List<List<string>>>();
    List<GameObject> blocksInPreview = new List<GameObject>();
    List<Vector2> letterPositions = new List<Vector2>();
    char[] levelLetters;
    const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private void Awake()
    {
        if (PrefsWrapper.GetInt("WasBetaTester", 0) == 0) //first time opening new app
        {
            if (PrefsWrapper.GetInt("IsV1.1", 0) == 1) //was beta tester
            {
                PrefsWrapper.DeleteAll();
                PrefsWrapper.SetInt("WasBetaTester", 1);
                PlayGamesController.GetComponent<Achievements>().BetaTester();
                updateCoins(2000, false); 
            }
            else
            {
                PrefsWrapper.SetInt("WasBetaTester", -1);
            }
            PrefsWrapper.SetInt("OwnsP0", 1);
            PrefsWrapper.SetInt("ActivePlayer", 0);
            PrefsWrapper.SetInt("isLetterUnlocked:4", 1);
            PrefsWrapper.Save();
            PlayGamesController.GetComponent<Achievements>().LetterUnlocked(4);
        }
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 120;
    }
    void Start()
    {
#if !(DEVELOPMENT_BUILD || UNITY_EDITOR)
     Debug.unityLogger.logEnabled = false; 
#endif
        //PrefsWrapper.SetInt("isLetterUnlocked:6", 1);
        //PrefsWrapper.SetInt("isLetterUnlocked:8", 1);
        //PrefsWrapper.SetInt("isLetterUnlocked:12", 1);

        var gridRect = GameGrid.GetComponent<RectTransform>();
        gridSemiWidth  = gridRect.rect.width / 2;
        gridSemiHeight  = -gridRect.rect.height / 2;
        var letterRect = LetterGrid.GetComponent<RectTransform>();
        letterGridSemiWidth = letterRect.rect.width / 2;
        letterGridSemiHeight  = -letterRect.rect.height / 2;
        var previewRect = PreviewGrid.GetComponent<RectTransform>();
        previewGridSemiWidth  = -previewRect.rect.width / 2;
        previewGridSemiHeight = previewRect.rect.height / 2;
        blockPaddingX = -Block.GetComponent<RectTransform>().rect.width - 10;
        blockPaddingY = Block.GetComponent<RectTransform>().rect.height + 10;
        gameGridStartingSize = GameGrid.transform.parent.GetComponent<RectTransform>().sizeDelta;
        StartCoroutine(loadDicts());
        InitLevelParameters();
        // setLevel(letters,100);
    }
    public void InitLevelParameters()
    {
        letters = PrefsWrapper.GetInt("Letters");
        if (letters == 0) letters = 4;
        stage = PrefsWrapper.GetInt("Stage");
        setCoins();
        setLevelTitle();
        setSizeModifier();
    }
    
    IEnumerator loadDicts()
    {
        // Debug.Log(Time.timeSinceLevelLoad);
        for (int i = startL-1; i <= endL; i++)
        {
            dicts.Add(new List<List<string>>());
            for (int y = 0; y < alphabet.Length; y++)
            {
                dicts[i- (startL - 1)].Add(new List<string>());
            }
        }
        
        for (int i = startL - 1; i <= endL; i++)
        {
            for (int y = 0; y < alphabet.Length; y++)
            {
                var q = "Data/Dicts/lDict" + i + "/" + alphabet[y];
                var x = Resources.Load<TextAsset>("Data/Dicts/lDict" + i + "/"+alphabet[y]);
                if (x == null) continue; //den yparxei leksi apo ayto to gramma me tosa grammata
                var lines = x.text.TrimEnd('\n').TrimEnd('\r').Replace("\r", "").Split('\n');
                foreach (var str in lines)
                {
                    dicts[i - (startL - 1)][y].Add(str);
                }
            }
            

        }
        yield return null;
       // Debug.Log(Time.timeSinceLevelLoad);

    }
    void Update()
    {
        #region background
        var l = getIndexOfActiveBg();
        if (GameBg.sprite.name != "l" + l)
        {
            try
            {
                GameBg.sprite = GameBGs[l];
            }
            catch { }
        }
        #endregion
        #region back key handler
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape) && objectsToHide[0].activeSelf)
            {
                ModalManager.Show("Quit", "Quit the app?", iconsForModals[0], new[] {new ModalButton(){Text = "No" } ,
                new ModalButton() { Text = "Yes" ,
                Callback = () => {PlayGamesController.OnUserQuit();
                } } });
                return;
            }
        }
        #endregion
        #region cancel button
        CancelButton.gameObject.SetActive(PreviewGrid.transform.childCount != 0);
        #endregion



    }
    public int getIndexOfActiveBg()
    {
        int l = letters;
        if (tempLetters != 0) l = tempLetters;
        return l - 4;
    }
    void setSizeModifier()
    {
        if (letters >= 19)
        {
            sizeModifier = 0.55f;
        }
        else if (letters >= 15)
        {
            sizeModifier = 0.6f;
        }
        else if (letters >= 11)
        {
            sizeModifier = 0.7f;
        }
        else if (letters >= 8)
        {
            sizeModifier = 0.8f;
        }
        else
        {
            sizeModifier = 1f;
        }
    }
    void setLevelTitle()
    {
        var stg = stage + 1;
        var strtoadd = stg == 1 ? "st" : (stg == 2 ? "nd" : (stg == 3 ? "rd" : "th"));
        LevelTitle.text = "Next level\n" + letters + " Letters: " + stg + strtoadd;
    }
    void HideAndShow(bool isPlaying)
    {
        foreach (var item in objectsToHide)
        {
            item.SetActive(!isPlaying);
        }
        foreach (var item in objectsToShow)
        {
            item.SetActive(isPlaying);

        }
    }
    void toggleButtons()
    {
        foreach (var btn in buttonsToDisableWhenLevelDone)
        {
            btn.interactable = !btn.interactable;
            
        }
    }
    void disableButtons()
    {
        foreach (var btn in buttonsToDisableWhenLevelDone)
        {
            btn.interactable = false;
        }
    }
    void toggleThisButton(string name,bool state)
    {
        foreach (var btn in buttonsToDisableWhenLevelDone)
        {
            if(btn.gameObject.name == name)
            {
                btn.interactable = state;
            }

        }
    }
    char[,] grid ;
    void readFromFileAndStartLevel(string file)
    {
        var m_Path = Application.dataPath;
        var txtAsset = Resources.Load<TextAsset>("Data/" + file);
        //var substr = txtAsset.text.Substring(txtAsset.text.IndexOf("s" + stage+"\r\n")).Replace("s" + stage + "\r\n", "");
        //var lvl = "";
        //int indxx = 0;
        //foreach (var c in substr)
        //{
        //    if(substr[indxx] =='s')//&& (substr[indxx+1] ==(letters+1) || substr[indxx+1] == letters))
        //    {
        //        break;
        //    }
        //    lvl += c;
        //    indxx++;
        //}
        var lines = txtAsset.text.TrimEnd('\n').TrimEnd('\r').Replace("\r","").Split('\n');
        
        int maxSize = lines[0].Length;
        var splitWords = splitBySize(lines, maxSize);
        splitWords.RemoveAll(x => x.Count == 0);
        maxSize = splitWords.Count;
        int noOfWords = maxSize; 
        List<string> wordsForPuzzle = new List<string>();

        //for (int i = splitWords.Count-1; i >= 0; i--)
        //{
        //    wordsForPuzzle.Add(splitWords[i][Random.Range(0, splitWords[i].Count)]);

        //}
        foreach (var x in splitWords)
        {
            var ind = Random.Range(0, x.Count);
            wordsForPuzzle.Add(x[ind]);
            x.RemoveAt(ind);
        }
        
        //int indx = 0;
        //while (indx < noOfWords)
        //{
        //    for (int i = 0; i < splitWords[indx + 1].Count; i++)
        //    {
        //        if (isStringContainedInString(wordsForPuzzle[indx], splitWords[indx + 1][i]))
        //        {
        //            wordsForPuzzle.Add(splitWords[indx + 1][i]);
        //            indx += 1;
        //            break;

        //        }
        //    }
        //}
        grid = new char[wordsForPuzzle.Count, wordsForPuzzle[0].Length]; //height*width
        for (int y = 0; y < wordsForPuzzle.Count; y++)
        {
            int indx = 0;
            for (int i = wordsForPuzzle[0].Length - 1; i >=0 ; i--) //tha ksekinane oi lekseis apo deksia, apo oso einai to length tis prwths lekshs
            {
                if(wordsForPuzzle[y].Length - 1 - indx < 0) //an teliwse h leksi
                {
                    continue;
                }
                grid[y, i] = wordsForPuzzle[y][wordsForPuzzle[y].Length-1 -indx];
                indx += 1;

            }
        }
        // Debug.Log(grid);
        string xx = "";

        for (int i = grid.GetLength(0)-1; i >=0 ; i--)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                xx += grid[i,y];
            }
            xx = xx.Replace('\0', ' ');
            xx += '\n';
        }
        //System.IO.File.WriteAllText(m_Path + "/Data/test",xx);

        //reset grid
        GameGrid.transform.parent.GetComponent<RectTransform>().sizeDelta = gameGridStartingSize;
        blockX = gridSemiWidth;
        blockY = gridSemiHeight - Block.GetComponent<RectTransform>().sizeDelta.y * (1 - sizeModifier);
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            var word = new GameObject("Word" + i,typeof(RectTransform));
            word.transform.SetParent(GameGrid.transform);
            word.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            word.GetComponent<RectTransform>().localScale = Vector3.one;
            word.AddComponent<BlockShouldRevealLetter>().word = wordsForPuzzle[i];
            word.GetComponent<BlockShouldRevealLetter>().PreviewGrid = PreviewGrid;
            wordObjects.Add(word);
            if (wordObjects.Count > 5)
            {
                //Make viewport of scrollview bigger
                var anchoredPos = GameGrid.transform.parent.GetComponent<RectTransform>().anchoredPosition;
                GameGrid.transform.parent.GetComponent<RectTransform>().sizeDelta += new Vector2(0, Block.GetComponent<RectTransform>().sizeDelta.y);
                GameGrid.transform.parent.GetComponent<RectTransform>().anchoredPosition = anchoredPos;
            }
            for (int y = grid.GetLength(1)-1; y >=0 ; y--)
            {
                //xx += grid[i, y];
                if (grid[i, y] == '\0') {
                    break;
                }
                blockX += blockPaddingX*sizeModifier;
                var block = Instantiate(Block, word.transform);
                block.GetComponent<RectTransform>().anchoredPosition += new Vector2(blockX, blockY);
                block.GetComponent<RectTransform>().sizeDelta = block.GetComponent<RectTransform>().sizeDelta*sizeModifier;
                block.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = grid[i, y].ToString();
                block.AddComponent<RevealBlockWhenClicked>().lvlManager = this;
                //block.gameObject.AddComponent<Button>().onClick.AddListener(delegate { var obj = block.transform.GetChild(2).gameObject; obj.SetActive(!obj.activeSelf); });
                //block.Add


            }
            playerManager.addBlockToClimb(word.transform.GetChild(word.transform.childCount-1).gameObject,blockPaddingX* sizeModifier);
            blockX += blockPaddingX * sizeModifier;
            var infoParent = Instantiate(Info, word.transform);
            var info = infoParent.transform.GetChild(0).gameObject;
            info.GetComponent<InfoButtonClicked>().wordToSearch = wordsForPuzzle[i];
            info.AddComponent<Button>().onClick.AddListener(delegate { showAdAndThenWordInfo(info); });
            info.transform.localScale *= sizeModifier;
            infoParent.GetComponent<RectTransform>().anchoredPosition += new Vector2(blockX, blockY);
            infoParent.GetComponent<RectTransform>().sizeDelta = infoParent.GetComponent<RectTransform>().sizeDelta * sizeModifier;
            infoParent.transform.SetParent(word.transform.parent);
            word.GetComponent<BlockShouldRevealLetter>().Info = infoParent;
            infoParent.SetActive(false);

            //reset 
            blockX = gridSemiWidth;
            blockY += blockPaddingY*sizeModifier;
            //xx = xx.Replace('\0', ' ');
            //xx += '\n';
        }
        setGameGridInScrollView();
        fillLetterGrid(wordsForPuzzle[0].ToCharArray());
        playerManager.LevelStarted(sizeModifier);
        toggleButtons();



    }
    public void playButton()
    {
        
        if (stage > 6 && false)
        {
            LevelDone();
            commingSoon.SetActive(true);
        }
        else
        {
            audioManager.startGameSound();
            HideAndShow(true);
           
           // AdManager.GetComponent<Interstitial>().LoadAd();
            clearEverything();
            readFromFileAndStartLevel("l" + letters + "/" + "l" + letters + "s" + stage);

        }
    }
    void setGameGridInScrollView()
    {
        GameGrid.transform.SetParent(GameGridScrollViewViewport.transform);
    }
    void fillLetterGrid(char[] letters)
    {
        levelLetters = letters;
        blockX = letterGridSemiWidth;
        blockY = letterGridSemiHeight;
        var modifiedblocksize = Block.GetComponent<RectTransform>().sizeDelta * sizeModifier;

        var howManyFitInSemiWidth = Mathf.FloorToInt(letterGridSemiWidth / modifiedblocksize.x) - 1;

        var pos = new Vector2[howManyFitInSemiWidth * 2, 4];
        for (int x = 0; x < howManyFitInSemiWidth; x++)
        {
            pos[x, 0] = new Vector2(modifiedblocksize.x * 1.2f, modifiedblocksize.y * 2 / 3);
            pos[x, 0] += x * new Vector2(modifiedblocksize.x * 1.2f, 0);
            pos[x, 1] = pos[x, 0] * new Vector2(-1, 1);
            pos[x, 2] = pos[x, 0] * new Vector2(1, -1);
            pos[x, 3] = pos[x, 0] * new Vector2(-1, -1);
        }
        //var howManyFitInSemiHeight = Mathf.FloorToInt(Mathf.Abs(letterGridSemiHeight) / modifiedblocksize.y);
        var noOfLetters = letters.Length;
        var noForCheck = noOfLetters;
        if (noOfLetters % 2 != 0)
        {
            letterPositions.Add(new Vector2(0, 0));
            noOfLetters--;
            noForCheck--;
        }
        int xind = 0;
        int yind = 0;
        while (noOfLetters > 0)
        {
            var spawnPos = pos[xind, yind];
            letterPositions.Add(spawnPos);
            noOfLetters--;
            yind++;
            if (yind > 3) { xind++; yind = 0; }
        }
        if (noForCheck % 4 == 2)
        {
            letterPositions[letterPositions.Count - 1] *= new Vector2(1, 0);
            letterPositions[letterPositions.Count - 2] *= new Vector2(1, 0);

        }
        letterPositions = rng(letterPositions);
        GameObject lastblock = null;
        for (int y = 0; y < letters.Length; y++)
        {
            //xx += grid[i, y];

            blockX += blockPaddingX * sizeModifier;
            var block = Instantiate(Block, LetterGrid.transform);


            /* Now spawn */
            //block.GetComponent<RectTransform>().anchoredPosition += new Vector2(blockX, blockY);
            block.GetComponent<RectTransform>().anchoredPosition += letterPositions[y];
            block.GetComponent<RectTransform>().sizeDelta = block.GetComponent<RectTransform>().sizeDelta * sizeModifier;
            block.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = letters[y].ToString();
            block.gameObject.AddComponent<Button>().onClick.AddListener(delegate { letterClicked(block); });
            block.transform.GetChild(2).gameObject.SetActive(false);

            //drag
            var trigger = block.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var drag = new UnityEngine.EventSystems.EventTrigger.Entry();
            var callback = new UnityEngine.EventSystems.EventTrigger.TriggerEvent();
            callback.AddListener(delegate { letterDragged(block); });
            drag.eventID = UnityEngine.EventSystems.EventTriggerType.Drag;
            drag.callback = callback;
            trigger.triggers.Add(drag);
            var release = new UnityEngine.EventSystems.EventTrigger.Entry();
            var callback2 = new UnityEngine.EventSystems.EventTrigger.TriggerEvent();
            callback2.AddListener(delegate { letterEndDrag(); });
            release.eventID = UnityEngine.EventSystems.EventTriggerType.EndDrag;
            release.callback = callback2;
            trigger.triggers.Add(release);


            //if(lastblock != null)
            //{
            //    if (block.GetComponent<RectTransform>().rect.Overlaps(lastblock.GetComponent<RectTransform>().rect))
            //    {
            //        block.GetComponent<RectTransform>().anchoredPosition *= (1/sizeModifier);
            //    }
            //}
            //lastblock = block;
            // block.transform.Rotate(new Vector3(0, 0,Random.Range(-20,21)));


        }

    }
    void removeButtonScriptFromLetterGrid()
    {
        for (int i = 1; i < LetterGrid.transform.childCount; i++) //i=0 to bg
        {
            Destroy(LetterGrid.transform.GetChild(i).gameObject.GetComponent<Button>());
        }
    }
    /// <summary>
    /// an yparxoyn childs sto PreviewGrid, prepei na treksei META to clearPreviewGrid()
    /// </summary>
    void removeRainbowLerpsFromLetterGrid()
    {
        for (int i = 1; i < LetterGrid.transform.childCount; i++) //i=0 to bg
        {
            Destroy(LetterGrid.transform.GetChild(i).transform.GetChild(0).gameObject.GetComponent<LerpColorRainbow>());
        }
    }
    public void setSearchesAfterSuccessAdShown()
    {
        var searchesLeft = PrefsWrapper.GetInt("SearchesLeft");
        if (searchesLeft < 0)
        {
            PrefsWrapper.SetInt("SearchesLeft", 4);
        }
    }
    public delegate void delegateShowInfo(GameObject wordSearchView);
    void showAdAndThenWordInfo(GameObject info)
    {
        var searchCount = PrefsWrapper.GetInt("SearchCount", 0) + 1;
        PrefsWrapper.SetInt("SearchCount", searchCount);
        PlayGamesController.GetComponent<Leaderboards>().SetScoreSearchedWords(searchCount);
        var searchesLeft = PrefsWrapper.GetInt("SearchesLeft", 4) - 1;
        PrefsWrapper.SetInt("SearchesLeft", searchesLeft);
        PrefsWrapper.Save();
        //info.gameObject.GetComponent<InfoButtonClicked>().InfoButtonClickedMethod(WordSearchView);
        //if (searchesLeft < 0)
        //{
        //    AdManager.GetComponent<NormalInterstitial>().ShowAd();
        //}
        if (searchesLeft < 0)
        {
            delegateShowInfo d = info.gameObject.GetComponent<InfoButtonClicked>().InfoButtonClickedMethod;
            AdManager.GetComponent<NormalInterstitial>().ShowAdWithDelegate(d, WordSearchView);
        }
        else
        {
            info.gameObject.GetComponent<InfoButtonClicked>().InfoButtonClickedMethod(WordSearchView);
            //oxi apolyta swsto , alla klain
            PlayGamesController.GetComponent<Achievements>().WordsSearched(PrefsWrapper.GetInt("SearchCount"));
        }
    }
    public void revealButton()
    {
       
        var newList = new List<GameObject>();
        foreach (var word in wordObjects)
        {
            //var childCount = word.transform.childCount; //epeidh mporei na einai kai o player sta childs
            //for (int i = 0; i < word.transform.childCount; i++)
            //{
            //    if (word.transform.GetChild(i).tag == "Player")
            //    {
            //        childCount -= 1;
            //        break;
            //    }
            //}
            //for (int i = 0; i < childCount; i++)
            //{
            //    if (word.transform.GetChild(i).transform.GetChild(2).gameObject.activeSelf)
            //    {
            //        newList.Add(word.transform.GetChild(i).gameObject);
            //    }
            //}
            for (int i = 0; i < word.transform.childCount; i++)
            {
                if (word.transform.GetChild(i).transform.GetChild(2).gameObject.activeSelf)
                {
                    newList.Add(word.transform.GetChild(i).gameObject);
                }
            }
            
            
        }
        if (newList.Count > 0) //diaforetika exei brei ola ta grammata
        {
            // revealLetter revLet = new revealLetter((newList)=>actuallyRevealLetter(newList));
            System.Action<List<GameObject>> actRev = newList=> actuallyRevealLetter(newList);
            ModalManager.Show("Reveal letter!", "You can reveal a random letter for 10 coins!\nYou can also choose the letter for 20 coins!",iconsForModals[8], new[] {new ModalButton(){Text = "Cancel" } ,
                new ModalButton() { Text = "I will pick!" ,
                Callback = () => { RevealBlockWhenClicked.isAllowedForClick = !RevealBlockWhenClicked.isAllowedForClick; toggleButtons(); } },
                new ModalButton() { Text = "Random!" ,
                Callback2 = (newList) => actRev(newList) , list = newList} });
            
        }
    }
    //delegate void revealLetter(List<GameObject> newList);
    void actuallyRevealLetter(List<GameObject> newList)
    {
        if (coins < 10)
        {
            ModalManager.Show("Oops!", "You dont have enough coins..\nPress the + on the top left to get more!", iconsForModals[8], new[] {new ModalButton(){Text = "Ok.." } 
                 });
            return;
        }
        removeLerps();
        clearPreviewGrid();
        removeRainbowLerpsFromLetterGrid();
        updateCoins(-10);
        //newList[i] -> block, newList[i].parent -> word
        var rand = Random.Range(0, newList.Count);
        newList[rand].transform.GetChild(2).gameObject.SetActive(false);
        var WordFound = newList[rand].transform.parent.GetComponent<BlockShouldRevealLetter>().checkIfWordIsFoundByRevealButton();
        if (WordFound)
        {
            SetWordFound(newList[rand].transform.parent.gameObject);
            checkIfAllWordsFound();
        }
    }
    public void actuallyRevealLetterByChoosing(GameObject blockPicked)
    {
        toggleButtons();
        if (coins < 20)
        {
            ModalManager.Show("Oops!", "You dont have enough coins..\nPress the + on the top left to get more!", iconsForModals[8], new[] {new ModalButton(){Text = "Ok.." }
                 });
            return;
        }
        removeLerps();
        clearPreviewGrid();
        removeRainbowLerpsFromLetterGrid();
        if (blockPicked.transform.GetChild(2).gameObject.activeSelf)
        {
            updateCoins(-20);
            //newList[i] -> block, newList[i].parent -> word
            blockPicked.transform.GetChild(2).gameObject.SetActive(false);
            var WordFound = blockPicked.transform.parent.GetComponent<BlockShouldRevealLetter>().checkIfWordIsFoundByRevealButton();
            if (WordFound)
            {
                SetWordFound(blockPicked.transform.parent.gameObject);
                checkIfAllWordsFound();
            }
        }
        else
        {
            return;
        }
       
    }
    /// <summary>
    /// asxeto me to preview grid.
    /// </summary>
    void removeLerps()
    {
        foreach (var obj in wordObjects)
        {
            if (obj.tag == "WordFound") continue;
            var res = obj.GetComponent<BlockShouldRevealLetter>().checkIfShouldRevealLetter("");
        }
    }
    void SetWordFound(GameObject word)
    {
        saveWordAsFoundAndCheckAchievementsAndLeaderboards(word.GetComponent<BlockShouldRevealLetter>().word, false);
        word.tag = "WordFound";
        playerManager.setBlockToClimbable(wordObjects.IndexOf(word));
        audioManager.wordFoundSound();
        //removeLerps();
        //removeRainbowLerpsFromLetterGrid();
        StartCoroutine(setLetterGridImage(true));
    }
    void SetExtraWordFound(string wrd)
    {
        saveWordAsFoundAndCheckAchievementsAndLeaderboards(wrd, true);
        if (extraWordFoundRout != null) StopCoroutine(extraWordFoundRout);
        extraWordFoundRout = StartCoroutine(showExtraWordFound(wrd));
        audioManager.extraWordFoundSound();
        clearPreviewGrid();
        removeRainbowLerpsFromLetterGrid();
        StartCoroutine(setLetterGridImage(true));
    }
    bool isExtraWordShowing = false;
    Coroutine extraWordFoundRout = null;
    IEnumerator showExtraWordFound(string wrd)
    {
        ExtraWordFoundText.text = "'" + wrd + "'";
        ExtraWordFoundAnim.SetBool("ExtraIn", true);
        yield return new WaitForSeconds(2.5f);
        ExtraWordFoundAnim.SetBool("ExtraIn", false);
        extraWordFoundRout = null;

    }
    void saveWordAsFoundAndCheckAchievementsAndLeaderboards(string wrd, bool isExtra)
    {
        var isNormalWordAlreadyFound = (PrefsWrapper.GetInt(wrd, 0) == 1);
        PrefsWrapper.SetInt(wrd, isExtra ? 2 : 1);
        if (isExtra)
        {
            var wrds = PrefsWrapper.GetString("AllExtraWordsFound");
            wrds += "\n" + wrd;
            var count = PrefsWrapper.GetInt("HowManyExtrasLeftForReward", 0);
            count++;
            if (count == 10)
            {
                updateCoins(5);
                count = 0;
            }
            PrefsWrapper.SetInt("HowManyExtrasLeftForReward", count);
            PrefsWrapper.SetString("AllExtraWordsFound", wrds);
            PlayGamesController.GetComponent<Achievements>().ExtraWords(PrefsWrapper.GetInt("ExtraWordsCount"));
            var extraWordCount = PrefsWrapper.GetInt("ExtraWordsCount", 0) + 1;
            PrefsWrapper.SetInt("ExtraWordsCount", extraWordCount);
            PlayGamesController.GetComponent<Leaderboards>().SetScoreExtraWords(extraWordCount);
            PrefsWrapper.Save();

        }
        else if (!isNormalWordAlreadyFound)
        {
            var wrds = PrefsWrapper.GetString("AllNormalWordsFound");
            wrds += "\n" + wrd;
            PrefsWrapper.SetString("AllNormalWordsFound", wrds);
            var normalWordCount = PrefsWrapper.GetInt("NormalWordsCount", 0) + 1;
            PrefsWrapper.SetInt("NormalWordsCount", normalWordCount);
            PlayGamesController.GetComponent<Leaderboards>().SetScoreWords(normalWordCount);
            PrefsWrapper.Save();
        }
    }
    bool getWordFoundFromPrefs(string wrd)
    {
        return PrefsWrapper.GetInt(wrd, 0) == 0 ?false:true;
    }
    public void shuffleButton(bool shouldPlaySound)
    {
        if(shouldPlaySound) audioManager.shuffleSound();
        letterPositions = rng(letterPositions);
        shouldReShuffle = PreviewGrid.transform.childCount > 0;
        for (int y = 1; y <= letterPositions.Count - PreviewGrid.transform.childCount; y++)
        {
            var block = LetterGrid.transform.GetChild(y).gameObject;
            block.GetComponent<RectTransform>().anchoredPosition = letterPositions[y-1];
        }
    }
    bool isDragging = false;
    void letterEndDrag()
    {
        isDragging = false;

    }
    void letterDragged(GameObject block)
    {
        if(Input.touchCount == 1)
        {
            isDragging = true;
            var touch = Input.GetTouch(0);
            //if (lastTouchPos != Vector2.zero)
            //{
            //    block.GetComponent<RectTransform>().anchoredPosition += touch.position -lastTouchPos  ;
            //}
            //lastTouchPos = touch.position;
            var sizeX = LetterGrid.GetComponent<RectTransform>().rect.size.x/2 - block.GetComponent<RectTransform>().rect.width/2; 
            var sizeY = LetterGrid.GetComponent<RectTransform>().rect.size.y/2 - block.GetComponent<RectTransform>().rect.width / 2;
            var X = (block.GetComponent<RectTransform>().anchoredPosition + touch.deltaPosition).x;
            var Y = (block.GetComponent<RectTransform>().anchoredPosition + touch.deltaPosition).y;

            if (X   < sizeX && Y  < sizeY && X > -sizeX && Y>-sizeY)
            {
                block.GetComponent<RectTransform>().anchoredPosition += touch.deltaPosition;
            }
           

        }

    }
    void removeLastLetterFromPreviewGrid(GameObject block)
    {
        if (block.transform.GetChild(0).GetComponent<LerpColorRainbow>() != null)
        {
            Destroy(block.transform.GetChild(0).GetComponent<LerpColorRainbow>());
        }
        var component = block.GetComponent<BlockMovedToPreviewGrid>();
        var posToGo = component.startingPos;
        Destroy(component);
        block.transform.SetParent(LetterGrid.transform);
        block.GetComponent<RectTransform>().anchoredPosition = posToGo;
        foreach (var word in wordObjects) //xreiazetai otan kaleitai apo simeio ektos apo to letterClicked
        {
            word.GetComponent<BlockShouldRevealLetter>().IndexMinus();
        }
        blocksInPreview.Remove(block);
        if (blocksInPreview.Count == 0)
        {
            clearPreviewGrid();
            removeRainbowLerpsFromLetterGrid();

        }
    }
    public void CancelButtonClicked()
    {
        audioManager.CancelSound();
        clearPreviewGrid();
        removeRainbowLerpsFromLetterGrid();
    }
    void removeLetterOrWordFromPreviewGrid(GameObject block)
    {
        if (block.transform.GetSiblingIndex() != PreviewGrid.transform.childCount - 1)
        {
            audioManager.CancelSound();

            clearPreviewGrid();
            removeRainbowLerpsFromLetterGrid();
        }

        else if (block.transform.GetSiblingIndex() == PreviewGrid.transform.childCount - 1) // einai to teleytaio
        {
            audioManager.CancelSound();
            removeLastLetterFromPreviewGrid(block);


        }
    }
    void letterClicked(GameObject block )
    {
        if (isDragging) return;
        if (RevealBlockWhenClicked.isAllowedForClick) return; //exei patisei na afairesei block apo to game grid
        if (block.transform.parent == PreviewGrid.transform) //to block einai sto preview grid  
        {
            removeLetterOrWordFromPreviewGrid(block);
            return;
        }         

        if (block.GetComponent<BlockMovedToPreviewGrid>() == null)
        {
            var posToGo = previewGridSemiWidth - blockPaddingX*sizeModifier * (PreviewGrid.transform.childCount + 1); //to ypologizw prin valw to component gia na min exoyme race condition
            var startingPos = block.GetComponent<RectTransform>().anchoredPosition;
            var component = block.AddComponent<BlockMovedToPreviewGrid>();
            component.PreviewGrid = PreviewGrid;
            component.positionToGo = new Vector2(posToGo, 0);
            component.startingPos = startingPos;

            blocksInPreview.Add(block);

        }
        bool anyRevealedLetter = false;
        bool wordFound = false;
        foreach(var obj in wordObjects)
        {
            var res =  obj.GetComponent<BlockShouldRevealLetter>().checkIfShouldRevealLetter(block.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text);
            if (res==1 && !anyRevealedLetter)//edw mpainei mia fora, osa gramta kai an emfanistoyn
            {
                audioManager.letterSound(PreviewGrid.transform.childCount,false);//mpainei sto epomeno fram to child opote de xreiazetai -1
                anyRevealedLetter = true;
            }
            if (res == 2)
            {
                wordFound = true;
                SetWordFound(obj);
            }
        }
        //if (wordFound)
        //{
        //    clearPreviewGrid();
        //}
        //else 
        if (!anyRevealedLetter)
        {
            //edw check an einai leksi poy den yparxei stis lekseis
            //prepei na parw ta grammata apo to preview grid
            //kai na dw an yparxei to string sto remaining words
            //prepei na ginetai me kapoio koumpi omws
            bool shouldClear = true;
            if (!wordFound)
            { //edw mesa nomizw mpainei an den exei brethei leksi KAI an den exei emfanistei allo grama
                //otan brisketai leksi , yparxei periptwsh na mhn emfanizetai kai allo grama, giayto xreiazetai ayto to check
                var wrd = getWordInPreviewGrid();
                bool extraWordFound = false;
                int startIndex = alphabet.IndexOf(wrd[0]);
                if (wrd.Length > 2)
                {
                    if (dicts[wrd.Length - 3][startIndex].Contains(wrd))
                    {
                        //h to wrd brethike sto leksiko.
                        //edw ena check an h leksi exei hdh vrethei. an nai prepei na synexiseis sta apo katw
                        if (!getWordFoundFromPrefs(wrd))
                        {
                            Debug.Log("extra word found");
                            SetExtraWordFound(wrd);
                            extraWordFound = true;
                            shouldClear = false; //ginetai clear sto setExtraWord()
                        }
                    }
                }
                var availableLetters = wrd.Length + LetterGrid.transform.childCount - 2;
                if (!extraWordFound && wrd.Length < availableLetters)
                {
                    //foreach (var wordList in dicts)
                    //for (int y = availableLetters - 3; y >= 0; y--)
                    //{
                    for (int y = wrd.Length<=2?0:wrd.Length-2; y <= availableLetters-3; y++)
                    {
                        var wordList = dicts[y][startIndex];
                        //if (wordList[0].Length > availableLetters) break; //den xreiazetai epeidh ebvala orio sto for
                        // int startIndex = wordList.IndexOf(wrd[0]);
                        for (int i = 0; i < wordList.Count; i++)
                        {
                            if (wordList[i].Contains(wrd))
                            {
                                //var ck = wordList[i].Replace(wrd,"")
                                //EDW check an h leksi poy ekane match, periexei kai ta ypoloipa gramata
                                //Debug.LogWarning(levelLetters.ToString());
                                //Debug.LogWarning(wordList[i]);
                                if (!getWordFoundFromPrefs(wordList[i]))
                                {
                                    List<char> remainingChars = new(levelLetters);
                                    List<char> remainingWord = new(wordList[i]);
                                    foreach (var c in wrd)
                                    {
                                        remainingChars.Remove(c);
                                        remainingWord.Remove(c);
                                    }
                                    bool isMatch = true;
                                    foreach (var c in remainingWord)
                                    {
                                        if (!remainingChars.Contains(c))
                                        {
                                            isMatch = false;
                                            break;
                                        }
                                        else
                                        {
                                            remainingChars.Remove(c);
                                        }
                                    }

                                    if (isMatch)
                                    {
                                        BlockMovedToPreviewGrid.setExtraLetter(true);
                                        audioManager.letterSound(PreviewGrid.transform.childCount, true);//mpainei sto epomeno fram to child opote de xreiazetai -1
                                        shouldClear = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (!shouldClear) break; //kati brethike

                    }

                }
            }

            if (shouldClear)
            {
                if (!wordFound)
                {
                    audioManager.wrongSound();
                    StartCoroutine(setLetterGridImage(false));

                    //BlockMovedToPreviewGrid.setExtraLetter(false);
                    var component = block.GetComponent<BlockMovedToPreviewGrid>();
                    var posToGo = component.startingPos;
                    Destroy(component);
                    block.transform.SetParent(LetterGrid.transform);
                    block.GetComponent<RectTransform>().anchoredPosition = posToGo;
                    blocksInPreview.Remove(block);
                }
                else
                {
                    clearPreviewGrid();
                }
                removeRainbowLerpsFromLetterGrid();
            }
        }
        else
        {
            foreach (var obj in wordObjects)
            {
                obj.GetComponent<BlockShouldRevealLetter>().removePendingScriptIfMarkedForRemoval();
            }
        }
        checkIfAllWordsFound();

    }
    void checkIfAllWordsFound()
    {
        if (wordObjects.TrueForAll(obj => obj.tag == "WordFound"))
        {
            Debug.Log("lvl done");
            LevelDone();
        }
    }
    void clearPreviewGrid()
    {
        BlockMovedToPreviewGrid.setExtraLetter(false);
        foreach (var word in wordObjects) //xreiazetai otan kaleitai apo simeio ektos apo to letterClicked
        {
            word.GetComponent<BlockShouldRevealLetter>().resetIndex();
        }
        foreach (var obj in blocksInPreview)
        {
            var component = obj.GetComponent<BlockMovedToPreviewGrid>();
            var posToGo = component.startingPos;
            Destroy(component);
            obj.transform.SetParent(LetterGrid.transform);
            obj.GetComponent<RectTransform>().anchoredPosition = posToGo;
        }

        blocksInPreview.Clear();
        if (shouldReShuffle)
        {
            shouldReShuffle = false;
            shuffleButton(false);
        }
    }
    string getWordInPreviewGrid()
    {
        string x = "";
        foreach (var block in blocksInPreview)
        {
            x += block.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text;
        }
        return x;
    }
    /// <summary>
    /// prepei na kalestei me to letters kai stage toy level poy tjeloyme na kanoyme set kai na fortwsoyme
    /// </summary>
    /// <param name="_let"></param>
    /// <param name="_stage"></param>
    public void setLevel(int _let, int _stage)
    {
        stage = _stage;
        letters = _let;
        var txtAsset = Resources.Load<TextAsset>("Data/" + "l" + letters + "/" + "l" + letters + "s" + stage);
        if (txtAsset == null)
        {
            if((PrefsWrapper.GetInt("isLetterUnlocked:" + (letters+1)) == 1))
            {
                stage = 0;
                letters++;
                txtAsset = Resources.Load<TextAsset>("Data/" + "l" + letters + "/" + "l" + letters + "s" + stage);
                if (txtAsset == null) //yparxei pithanotita na min exei levels to epomeno letterNo, mpakalia
                {
                    letters++;
                }
            }
            else
            {
                var tmpS = 0;
                while (true)
                {
                    if(PrefsWrapper.GetInt("l" + letters.ToString() + "s" + tmpS.ToString(), 0) == 0)
                    {
                        stage = tmpS;
                        break;
                    }
                    tmpS++;
                }
            }
            
        }
        PrefsWrapper.SetInt("Letters", letters);
        PrefsWrapper.SetInt("Stage", stage);
        setLevelTitle();
        setSizeModifier();


        //if (!File)

    }
    public int getLevelsNoOfLetters(int let)
    {
        var passedLet = let;
        var txtAsset = Resources.Load<TextAsset>("Data/" + "metadata");
        var tmp = txtAsset.text.Substring(txtAsset.text.IndexOf("l" + let + "sC")); //l4sC100
        tmp = tmp.Replace("l" + let + "sC", "");
        var count = "";
        foreach (var c in tmp)
        {
            if (c == '\r')
            {
                break;
            }
            count += c;
        }
        return System.Convert.ToInt32(count);
    }
    public void showSettingsView()
    {
        // StartCoroutine(loadLvlData());
        if (isAnyViewOpenOtherThanThis(SettingsView)) return;
        SettingsView.GetComponent<Animator>().SetBool("In", !SettingsView.GetComponent<Animator>().GetBool("In"));


    }
    public void showExtraCoinsView()
    {
        if (isAnyViewOpenOtherThanThis(ExtraCoinsView)) return;
        ExtraCoinsView.GetComponent<Animator>().SetBool("In", !ExtraCoinsView.GetComponent<Animator>().GetBool("In"));
    }
    public void showLeaderboardView()
    {
        if (isAnyViewOpenOtherThanThis(LeaderboardView)) return;
        LeaderboardView.GetComponent<Animator>().SetBool("In", !LeaderboardView.GetComponent<Animator>().GetBool("In"));
        if (LeaderboardView.GetComponent<Animator>().GetBool("In")) PlayGamesController.GetComponent<Leaderboards>().UpdateAllScores();
    }
    public void showFortuneWheelView(bool goIn)
    {
        // StartCoroutine(loadLvlData());
        if (isAnyViewOpenOtherThanThis(FortuneWheelView)) return;

        FortuneWheelView.GetComponent<Animator>().SetBool("In", goIn);
        if (goIn)
        {

        }
        else
        {

        }

    }
    public void showMarketView(bool goIn)
    {
        // StartCoroutine(loadLvlData());
        if (isAnyViewOpenOtherThanThis(MarketView)) return;

        MarketView.GetComponent<Animator>().SetBool("In", goIn);
        if (goIn)
        {
          
        }
        else
        {

        }

    }
    public void showExtraWordsView(bool goIn)
    {
        // StartCoroutine(loadLvlData());
        if (isAnyViewOpenOtherThanThis(ExtraWordsView)) return;

        ExtraWordsView.GetComponent<Animator>().SetBool("In", goIn);
        if (goIn)
        {
            var count = PrefsWrapper.GetInt("HowManyExtrasLeftForReward", 0);
            ExtraWordsViewText.text =  PrefsWrapper.GetString("AllExtraWordsFound");
            ExtraWordsViewSubtitle.text= "Find " +(10 - count).ToString() + " more words to earn coins!";
            nrmlWordsscrlManager.ReInit();
            //scrlManager.ReInit(tempLetters);

        }
        else
        {
            
        }

    }
    int tempLetters = 0;
    public void showLevelSelect(bool goIn)
    {
        // StartCoroutine(loadLvlData());
        if (isAnyViewOpenOtherThanThis(LevelSelect)) return;
        LevelSelect.GetComponent<Animator>().SetBool("In", goIn);
        if (goIn)
        {
            var lastLet = PrefsWrapper.GetInt("LastLetter");
            tempLetters = lastLet;
            if(tempLetters < startL || tempLetters > endL)
            {
                tempLetters = 4;
            }
            setLevelSelectTitle(tempLetters);
            scrlManager.ReInit(tempLetters);

        }
        else
        {
            PrefsWrapper.SetInt("LastLetter", tempLetters);
            PrefsWrapper.Save();
            tempLetters = 0;
        }

    }
    public void showLevelSelectNext()
    {
        if (tempLetters +1 > endL)
        {
            return;
        }
        tempLetters++;
        setLevelSelectTitle(tempLetters);
        scrlManager.ReInit(tempLetters);

    }
    public void showLevelSelectPrev()
    {
        if (tempLetters -1 < startL)
        {
            return;
        }
        tempLetters--;
        setLevelSelectTitle(tempLetters);
        scrlManager.ReInit(tempLetters);

    }
    bool isAnyViewOpenOtherThanThis(GameObject obj)
    {
        return (LevelSelect.GetComponent<Animator>().GetBool("In") ||
        ExtraWordsView.GetComponent<Animator>().GetBool("In") ||
        MarketView.GetComponent<Animator>().GetBool("In") ||
        FortuneWheelView.GetComponent<Animator>().GetBool("In") ||
        ExtraCoinsView.GetComponent<Animator>().GetBool("In") ||
        LeaderboardView.GetComponent<Animator>().GetBool("In") ||
        SettingsView.GetComponent<Animator>().GetBool("In"))
        && !obj.GetComponent<Animator>().GetBool("In") //nice code
        ;

    }
    void setLevelSelectTitle(int tempLet)
    {
        LevelSelect.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = tempLet + "LETTERS";

    }
    void setLevelAsPassed(int let , int s)
    {
        var passedLevelsCount = PrefsWrapper.GetInt("LevelsPassedCount", 0) + 1;
        PrefsWrapper.SetInt(let + "LetterStagesCleared", PrefsWrapper.GetInt(let + "LetterStagesCleared") + 1);
        PrefsWrapper.SetInt("LevelsPassedCount", passedLevelsCount);
        PlayGamesController.GetComponent<Achievements>().LevelsPassed(passedLevelsCount);
        PlayGamesController.GetComponent<Leaderboards>().SetScorePassedLevels(passedLevelsCount);
        PrefsWrapper.SetInt("l" + let.ToString() + "s" + s.ToString(), 1);
        PrefsWrapper.Save();
    }
    void checkIfAnyLetterUnlocked()
    {
        for (int y = startL; y <= endL; y++)
        {
            if (PrefsWrapper.GetInt("isLetterUnlocked:" + y) == 1) continue;
            bool isUnlocked = true;
            for (int i = 4; i < y; i++)
            {
                var totalLvlsNeeded = Mathf.Min(getLevelsNoOfLetters(i), 10 * (y - i));
                var c = totalLvlsNeeded - PrefsWrapper.GetInt(i + "LetterStagesCleared");
                if (c <= 0) continue;
                isUnlocked = false;
                break;
            }
            if (isUnlocked)
            {
                PrefsWrapper.SetInt("isLetterUnlocked:" + y, 1);
                ModalManager.Show("Nice!", "You unlocked levels with " +y+ " letters!\nGo to the starting menu to change the level.", iconsForModals[10], new[] {new ModalButton(){Text = "Yay!"}
                 }); ;
                PlayGamesController.GetComponent<Achievements>().LetterUnlocked(y);
                var letterUnlockedCount = PrefsWrapper.GetInt("LetterUnlockedCount", 0) + 1;
                PrefsWrapper.SetInt("LetterUnlockedCount", letterUnlockedCount);
                PlayGamesController.GetComponent<Leaderboards>().SetScoreUnlockedLetters(letterUnlockedCount);
                PrefsWrapper.Save();
            }
        }
    }
    void LevelDone()
    {
        checkIfNewLevelCompleted(letters, stage);
        removeButtonScriptFromLetterGrid();
        toggleButtons();
        toggleThisButton("Next", false);
        //stage += 1;
        //setLevel(letters,stage+1);
        StartCoroutine(waitForChangeScreen());
    }
    void checkIfNewLevelCompleted(int l, int s)
    {
        if (PrefsWrapper.GetInt("l" + l + "s" + s) != 1)
        {
            setLevelAsPassed(l, s);
            checkIfAnyLetterUnlocked();
            PrefsWrapper.Save();
            updateCoins(2);
            // EnergyManager.gainEnergy(1);
        }
    }
    IEnumerator waitForChangeScreen()
    {
        yield return new WaitForSeconds(3f);
        Game.SetBool("GameIn", false);
        Intermission.SetBool("IntermissionIn", true);
        yield return new WaitForSeconds(1f);
        setLevel(letters, stage + 1);//ayto to evala edw gia na min allazei to fonto toso grigora, den kserw an einai ok
        AdManager.GetComponent<Banner>().ShowBanner();
        if (isReadyForInterstitial <= 0)
        {
            AdManager.GetComponent<Interstitial>().ShowAd();
           // AdManager.GetComponent<Interstitial>().LoadAd();
        }
        else
        {
                isReadyForInterstitial -= 1;
        }
        clearEverything();
        playerManager.Intermission();
        yield return new WaitForSeconds(1f);//me end of frame kamia fora ksemenei to banner, isws thelei waitForSeconds(1f)
        toggleThisButton("Next", true);

    }
    void clearEverything()
    {
        clearPreviewGrid();
        removeRainbowLerpsFromLetterGrid();
        clearListsAndObjects();

    }
    void clearListsAndObjects()
    {
        for (int i = 1; i < GameGrid.transform.childCount; i++) //to 0 einai o player
        {
            Destroy(GameGrid.transform.GetChild(i).gameObject);
        }
        for (int i = 1; i < LetterGrid.transform.childCount; i++) //to 0 einai to bg
        {
            Destroy(LetterGrid.transform.GetChild(i).gameObject);
        }
        letterPositions.Clear();
        blocksInPreview.Clear();
        wordObjects.Clear();
        playerManager.resetBlocks();
    }
    public void nextButton()
    {
        if (stage > 6 && false)
        {
            commingSoon.SetActive(true);
        }
        else
        {
            Game.SetBool("GameIn", true);
            Intermission.SetBool("IntermissionIn", false);
            readFromFileAndStartLevel("l" + letters + "/" + "l" + letters + "s" + stage);
            AdManager.GetComponent<Banner>().HideBanner();

        }

    }
    public bool ownsThisPlayer(int playerNo, int price)
    {
        if (PrefsWrapper.GetInt("OwnsP" + playerNo) == 1)
        {
            if (PrefsWrapper.GetInt("ActivePlayer") != playerNo)
            {
                ModalManager.Show("Change", "You changed your buddy!", iconsForModals[10], new[] { new ModalButton() { Text = "Οκ!" } });
            }
            return true;
        }
        else
        {
            ModalManager.Show("Buy", "This partner costs " + price + " coins! Continue?", iconsForModals[9], new[] {new ModalButton(){Text = "No.." } ,
                new ModalButton() { Text = "Yes!" ,
                Callback = () => {
                    if (coins < price)
                    {
                        ModalManager.Show("Oops!", "You dont have enough coins..", iconsForModals[6], new[] {new ModalButton(){Text = "Ok.." }});
                    }
                    else
                    {
                        buyPlayer(playerNo,price);
                    }
                } } });
            return false;
        }

    }
    void buyPlayer(int playerNo, int price)
    {
        PlayGamesController.GetComponent<Achievements>().PartnerUnlocked(playerNo);
        PrefsWrapper.SetInt("OwnsP" + playerNo, 1);
        var partnersUnlockedCount = PrefsWrapper.GetInt("UnlockedPartnersCount", 0) + 1;
        PrefsWrapper.SetInt("UnlockedPartnersCount", partnersUnlockedCount);
        PlayGamesController.GetComponent<Leaderboards>().SetScoreUnlockedPartners(partnersUnlockedCount);
        PrefsWrapper.Save();
        updateCoins(-price);
        playerManager.setPlayer(playerNo, true);
    }
    public void homeButton()
    {
        ModalManager.Show("Leaving level", "Are you sure you want to go to the main menu? Your progress in this level will be lost.", iconsForModals[0], new[] {new ModalButton(){Text = "No" } ,
                new ModalButton() { Text = "Yes" ,
                Callback = () => {
                    clearEverything();
                    disableButtons();
                    HideAndShow(false);} } });
        

    }
    void setCoins()
    {

        var areCoinsSet = PrefsWrapper.GetString("areCoinsSet");
        if (areCoinsSet == "")
        {
            coins += 50;
            PrefsWrapper.SetString("areCoinsSet", "true");
            PrefsWrapper.SetInt("Coins", coins);
            PrefsWrapper.Save();

        }
        coins = PrefsWrapper.GetInt("Coins");
        coinsNumberUI.text = coins.ToString();

    }
    /// <summary>
    /// update coins
    /// </summary>
    /// <param name="amountToAdd">
    /// amount can be negative
    /// </param>
    public void updateCoins(int amountToAdd, bool Animate = true)
    {
        //
        coins += amountToAdd;
        PrefsWrapper.SetInt("Coins", coins);
        PrefsWrapper.Save();
        StartCoroutine(addCoinsOneByOne(amountToAdd, Animate));

    }
    bool isCoinsRunning = false;
    IEnumerator addCoinsOneByOne(int amountToAdd, bool Animate)
    {
        while (isCoinsRunning)
        {
            yield return new WaitForSeconds(0.1f);
        }
        isCoinsRunning = true;
        if (amountToAdd > 0 && Animate)
        {
            for (int i = amountToAdd; i > 0; i--)
            {
                Instantiate(Coin, GameGrid.transform.parent.parent.parent.parent.parent);
                audioManager.coinSound();
                int.TryParse(coinsNumberUI.text, out int coinTxt);
                coinsNumberUI.text = (coinTxt + 1).ToString();
                yield return new WaitForSeconds(0.1f);

            }
        }
        else
        {
            //coins += amountToAdd;
            int.TryParse(coinsNumberUI.text, out int coinTxt);
            coinsNumberUI.text = (coinTxt + amountToAdd).ToString();
        }
        isCoinsRunning = false;

    }
    IEnumerator setLetterGridImage(bool tick)
    {
        LetterGrid.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = LetterGridStates[tick ? 1 : 2];
        yield return new WaitForSeconds(0.5f);
        LetterGrid.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = LetterGridStates[0];

    }
    List<List<string>> splitBySize(string[] lines,int maxSize)
    {
        var ret = new List<List<string>>();
        for (int i = 0; i < maxSize; i++)
        {
            ret.Add(new List<string>());
        }
        for (int i = 0; i < lines.Length ; i++)
        {
            ret[maxSize -lines[i].Length].Add(lines[i]);
        }
        return ret;
    }
    bool isStringContainedInString(string wordToCheck, string wordToBeChecked)
    {

        while (true)
        {
            if (wordToCheck.Contains(wordToBeChecked[0]))
            {
                wordToCheck = wordToCheck.Remove(wordToCheck.IndexOf(wordToBeChecked[0]),1);
                wordToBeChecked =wordToBeChecked.Remove(0,1);
            }
            else
            {
                return false;
            }
            if (wordToBeChecked.Length == 0)
            {
                return true;
            }
        }
    }
    public static List<Vector2> rng(List<Vector2> aList)
    {

        System.Random _random = new System.Random();

        Vector2 myGO;

        int n = aList.Count;
        for (int i = 0; i < n; i++)
        {
            // NextDouble returns a random number between 0 and 1.
            // ... It is equivalent to Math.random() in Java.
            int r = i + (int)(_random.NextDouble() * (n - i));
            myGO = aList[r];
            aList[r] = aList[i];
            aList[i] = myGO;
        }

        return aList;
    }
    static string GetHtmlFromUri(string resource)
    {
        string html = string.Empty;
        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(resource);
        try
        {
            using (HttpWebResponse resp = (HttpWebResponse)req.GetResponse())
            {
                bool isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                if (isSuccess)
                {
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        //We are limiting the array to 80 so we don't have
                        //to parse the entire html document feel free to 
                        //adjust (probably stay under 300)
                        char[] cs = new char[80];
                        reader.Read(cs, 0, cs.Length);
                        foreach (char ch in cs)
                        {
                            html += ch;
                        }
                    }
                }
            }
        }
        catch
        {
            return "";
        }
        return html;
    }
    public static bool checkInternetConnection()
    {
        string HtmlText = GetHtmlFromUri("http://google.com");
        if (HtmlText == "")
        {
            //No connection
            return false;
        }
        else if (!HtmlText.Contains("schema.org/WebPage"))
        {

            return false;

            //Redirecting since the beginning of googles html contains that 
            //phrase and it was not found
        }
        else
        {
            //success
            return true;

        }
    }
}
