using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    List<isBlockClimbed> blocksToClimb = new List<isBlockClimbed>();
    float blockPaddingX;
    public LevelManager lvlManager;
    public GameObject Player;
    public GameObject GameGrid;
    public GameObject IntermissionScreen;

    SprytLite playerAnim;
    RectTransform playerRect;
    Vector2 startingSize;
    int scaleIndx = 0;
    bool lvlStart = false;
    bool intermission = false;

    Vector2 startingPos;
    Vector2 posToSendInIntermission;
    // List<IEnumerator> coroutines = new List<IEnumerator>();
    /// <summary>
    /// index = 0 Dog<br></br>
    ///  index = 1 Dog 2
    /// </summary>
    public List<SpriteAtlas> Players;
    public List<int> PlayerPrices;
    public List<Sprite> goingIdleFrames = new List<Sprite>();
    public List<Sprite> IdleFrames = new List<Sprite>();
    public List<Sprite> goingSittingFrames = new List<Sprite>();
    public List<Sprite> sitingFrames = new List<Sprite>();
    public List<Sprite> runningFrames = new List<Sprite>();
    public List<Sprite> jumpingFrames = new List<Sprite>();
    public List<Sprite> levelEndFrames = new List<Sprite>();
    areFramesActive goingIdle;
    areFramesActive Idle;
    areFramesActive goingSitting;
    areFramesActive sitting;
    areFramesActive running;
    areFramesActive jumping;
    areFramesActive levelEnd;


    List<areFramesActive> allFrames = new List<areFramesActive>();
    List<Coroutine> coroutinesToStop = new List<Coroutine>();
    bool isComing = true;
    void Start()
    {
       // lvlManager.updateCoins(600);
        //PlayerPrefs.SetInt("OwnsP1", 1);
        //PlayerPrefs.SetInt("OwnsP2", 1);
        //PlayerPrefs.SetInt("OwnsP3", 1);
        //PlayerPrefs.SetInt("OwnsP4", 1);

        setPlayer(PrefsWrapper.GetInt("ActivePlayer"),false);
        playerRect = Player.GetComponent<RectTransform>();
        startingSize = playerRect.sizeDelta;
        playerAnim = Player.GetComponent<SprytLite>();
        startingPos = playerRect.anchoredPosition;
        posToSendInIntermission = new Vector2(0, 0);
        goingIdle = new areFramesActive
        {
            frames = goingIdleFrames,
            areActive = false
        };
        allFrames.Add(goingIdle);

        Idle = new areFramesActive
        {
            frames = IdleFrames,
            areActive = false
        };
        allFrames.Add(Idle);

        goingSitting = new areFramesActive
        {
            frames = goingSittingFrames,
            areActive = false
        };
        allFrames.Add(goingSitting);

        sitting = new areFramesActive
        {
            frames = sitingFrames,
            areActive = false
        };
        allFrames.Add(sitting);

        running = new areFramesActive
        {
            frames = runningFrames,
            areActive = false
        };
        allFrames.Add(running);
        jumping = new areFramesActive
        {
            frames = jumpingFrames,
            areActive = false
        };
        allFrames.Add(jumping);
        levelEnd = new areFramesActive
        {
            frames = levelEndFrames,
            areActive = false
        };
        allFrames.Add(levelEnd);
    }
    void stopAnimCoroutines()
    {
        foreach (var co in coroutinesToStop)
        {
            StopCoroutine(co);
        }
        coroutinesToStop.Clear();
    }
    public void setPlayerButton(int playerNo)
    {
        setPlayer(playerNo, true);
    }
    public void setPlayer(int playerNo, bool forceAssignFrames)
    {
        if (!lvlManager.ownsThisPlayer(playerNo,PlayerPrices[playerNo])) return;
        PrefsWrapper.SetInt("ActivePlayer",playerNo);
        Sprite[] q = new Sprite[34];
        Players[playerNo].GetSprites(q);
        var ls = q.ToList().OrderBy(k => Convert.ToInt32(k.name.Replace("(Clone)", ""))).ToList();
        goingIdleFrames.Clear();
        goingIdleFrames.Add(ls[6]); goingIdleFrames.Add(ls[20]); goingIdleFrames.Add(ls[21]); goingIdleFrames.Add(ls[22]); goingIdleFrames.Add(ls[23]);
        goingIdleFrames.Add(ls[24]);
        IdleFrames.Clear();
        IdleFrames.Add(ls[24]); IdleFrames.Add(ls[25]); //IdleFrames.Add(ls[30]);
        goingSittingFrames.Clear();
        goingSittingFrames.Add(ls[7]); goingSittingFrames.Add(ls[0]); goingSittingFrames.Add(ls[12]); goingSittingFrames.Add(ls[11]); goingSittingFrames.Add(ls[30]);
        sitingFrames.Clear();
        sitingFrames.Add(ls[29]); sitingFrames.Add(ls[28]);
        runningFrames.Clear();
        runningFrames.Add(ls[31]); runningFrames.Add(ls[32]); runningFrames.Add(ls[33]);
        jumpingFrames.Clear();
        jumpingFrames.Add(ls[31]);
        levelEndFrames.Clear();
        levelEndFrames.Add(ls[18]); levelEndFrames.Add(ls[19]);
        if (forceAssignFrames)
        {
            //force assign frames
            //not needed when called from start
            stopAnimCoroutines();
            resetStateOfAllButThis(sitting);
            var x = StartCoroutine(setFramesAndSpeed(sitting, 0.5f, 0));
            coroutinesToStop.Add(x);
        }
    }
    void FixedUpdate()
    {

        if (lvlStart)
        {
            var pos = playerRect.anchoredPosition;

            if (scaleIndx >= blocksToClimb.Count)
            {
                var blockHeight = blocksToClimb[1].blockPositionRelativeToPlayer - blocksToClimb[0].blockPositionRelativeToPlayer;
                var lastBlockHeight = blocksToClimb[blocksToClimb.Count - 1].blockPositionRelativeToPlayer + blockHeight;
                //var posToGoEnd = new Vector2(blockPaddingX, lastBlockHeight - playerRect.rect.height / 2.5f);
                var posToGoEnd = lastBlockHeight + new Vector2(blockPaddingX, -playerRect.rect.height / 2.5f);
                if (Mathf.Abs(pos.y - posToGoEnd.y) < 2)
                {
                    //Debug.Log("APRoXIMATE");
                    playerRect.anchoredPosition += new Vector2(8f, 0);
                    if (jumping.areActive)
                    {
                        //ayto xreiazetai an ftasei sto teleytaio block kateytheian
                        jumping.areActive = false;
                        running.areActive = false;
                    }
                    //StopAllCoroutines();
                    if (!running.areActive)
                    {
                        var x = StartCoroutine(setFramesAndSpeed(running, 0.4f, 0));
                        coroutinesToStop.Add(x);
                    }
                }
                else
                {
                    if (!jumping.areActive)
                    {
                        stopAnimCoroutines();
                        resetStateOfAllButThis(jumping);
                        var x = StartCoroutine(setFramesAndSpeed(jumping, 0.5f, 0));
                        coroutinesToStop.Add(x);

                    }
                    playerRect.anchoredPosition = Vector2.Lerp(pos, posToGoEnd, Time.deltaTime * 3);

                }

                return;
            }
            //var blockRect = blocksToClimb[scaleIndx].block.GetComponent<RectTransform>();
            var posToGo = blocksToClimb[scaleIndx].blockPositionRelativeToPlayer + new Vector2(blockPaddingX, -playerRect.rect.height / 2.5f);
            if (Mathf.Abs(pos.y - posToGo.y) < 2)
            {

                //vale to idle animation
                if (!goingIdle.areActive) { var x = StartCoroutine(setFramesAndSpeed(goingIdle, 0.2f, 0, jumping)); coroutinesToStop.Add(x); }
                if (!Idle.areActive) { var x = StartCoroutine(setFramesAndSpeed(Idle, 0.2f, 0.5f)); coroutinesToStop.Add(x); }
                if (!goingSitting.areActive) { var x = StartCoroutine(setFramesAndSpeed(goingSitting, 0.2f, 11)); coroutinesToStop.Add(x); }
                if (!sitting.areActive) { var x = StartCoroutine(setFramesAndSpeed(sitting, 0.2f, 12)); coroutinesToStop.Add(x); }
                isComing = false;



            }
            else
            {

                if (!jumping.areActive && !isComing)
                {

                    resetStateOfAllButThis(running);
                    stopAnimCoroutines();
                    var x = StartCoroutine(setFramesAndSpeed(jumping, 0.5f, running.areActive ? 0 : 1)); //running.areActive?0:1 th prwth fora na exei ena delay, meta oxi
                    coroutinesToStop.Add(x);
                }
                if (!running.areActive) //prepei na einai apo katw, epeidh to apo panw exei to resetStateOfAllButThis
                {
                    var x = StartCoroutine(setFramesAndSpeed(running, 0.4f, 0, jumping));
                    coroutinesToStop.Add(x);

                }
                playerRect.anchoredPosition = Vector2.Lerp(pos, posToGo, Time.deltaTime * 3);

            }
            //edw check an to blockToClimb[indx+1].isClimbable , kai allazoyme to index
            //if (blocksToClimb[scaleIndx + 1].isClimbable && blocksToClimb[scaleIndx].isClimbed)
            //{
            //    scaleIndx += 1;
            //}else
            if (blocksToClimb[scaleIndx].isClimbable && !blocksToClimb[scaleIndx].isClimbed)
            {
                blocksToClimb[scaleIndx].isClimbed = true;
                scaleIndx += 1;

            }
        }
        if (intermission)
        {
            isComing = true;
            var pos = playerRect.anchoredPosition;
            var posToGo = posToSendInIntermission;
            if (Mathf.Abs(pos.x - posToGo.x) < 10)
            {

                //vale to idle animation
                if (!levelEnd.areActive) { var x = StartCoroutine(setFramesAndSpeed(levelEnd, 0.2f, 0, running)); coroutinesToStop.Add(x); }
               // if (!levelEnd.areActive) { var x = StartCoroutine(setFramesAndSpeed(levelEnd, 0.2f, 0.5f)); coroutinesToStop.Add(x); }
                //if (!goingSitting.areActive) { var x = StartCoroutine(setFramesAndSpeed(goingSitting, 0.2f, 11)); coroutinesToStop.Add(x); }
                //if (!Sitting.areActive) { var x = StartCoroutine(setFramesAndSpeed(Sitting, 0.2f, 12)); coroutinesToStop.Add(x); }




            }
            else
            {

                //playerRect.anchoredPosition = Vector2.Lerp(pos, posToGo, Time.deltaTime);
                if (!running.areActive)
                {
                    stopAnimCoroutines();
                    resetStateOfAllButThis(running);
                    var x = StartCoroutine(setFramesAndSpeed(running, 0.4f, 0));
                    coroutinesToStop.Add(x);
                }
                playerRect.anchoredPosition += new Vector2(16f, 0);


            }

        }

    }
    /// <summary>
    /// frames = null gia na ginoyn reset ola
    /// </summary>
    /// <param name="frames"></param>
    void resetStateOfAllButThis(areFramesActive frames)
    {
        foreach (var framObj in allFrames)
        {
            if (framObj != frames)
            {
                framObj.areActive = false;
            }
        }
    }
    public void Intermission()
    {
        resetBlocks();
        intermission = true;
        Player.transform.SetParent(IntermissionScreen.transform);
        playerRect.sizeDelta = startingSize ;
        playerRect.anchoredPosition = new Vector2(-Screen.width / 1.5f, 0);
    }
    public void resetBlocks()
    {
        //stop to animation kai hide to image
        lvlStart = false;
        blocksToClimb.Clear();
        playerRect.anchoredPosition = startingPos; //ayto xreiazetai gia otan kaleitai apo alloy, px to home button
        posToSendInIntermission = new Vector2(0, 0);
        scaleIndx = 0;
        stopAnimCoroutines();
        resetStateOfAllButThis(null);
    }
    public void addBlockToClimb(GameObject block, float _blockPaddingX)
    {
        blocksToClimb.Add(new isBlockClimbed { block = block, isClimbed = false, isClimbable = false });
        blockPaddingX = _blockPaddingX;
    }
    public void LevelStarted(float sizeModifier)
    {
        playerRect.sizeDelta = startingSize*sizeModifier;
        foreach (var block in blocksToClimb) //pairnw ta position poy prepei na paei, gia na min valw to player ws child toy word
        {
            var x = Instantiate(block.block, Player.transform.parent);
            block.blockPositionRelativeToPlayer = x.GetComponent<RectTransform>().anchoredPosition;
            Destroy(x);
        }
        //start to animation kai show to image       
        StartCoroutine(levelStarAnimationWithDelay());

    }
    IEnumerator levelStarAnimationWithDelay()
    {
        if (intermission)
        {
            posToSendInIntermission = new Vector2(Screen.width, 0);
            yield return new WaitForSeconds(1f);
            intermission = false;
        }
        Player.transform.SetParent(GameGrid.transform);
        Player.transform.SetAsFirstSibling();
        playerRect.anchoredPosition = startingPos;
        stopAnimCoroutines();
        lvlStart = true;

    }
    public void setBlockToClimbable(int indx)
    {
        blocksToClimb[indx].isClimbable = true;
    }
    IEnumerator setFramesAndSpeed(areFramesActive frames, float speed, float timeToWait, areFramesActive resetStateOfThis = null)
    {
        frames.areActive = true; //prin to wait prepei ayto
        if (resetStateOfThis != null) //kai ayto
        {
            resetStateOfThis.areActive = false;
        }
        yield return new WaitForSeconds(timeToWait);
        //playerAnim.frames = frames;
        playerAnim.Pause();
        yield return new WaitForEndOfFrame();
        playerAnim.AssignFrames(frames.frames);
        playerAnim.speed = speed;
        yield return new WaitForEndOfFrame();
        playerAnim.Resume();
        yield return new WaitForEndOfFrame();


    }

}
class isBlockClimbed
{
    public GameObject block { get; set; }
    public bool isClimbed { get; set; }
    public bool isClimbable { get; set; }
    public Vector2 blockPositionRelativeToPlayer { get; set; }
}

class areFramesActive
{
    public List<Sprite> frames { get; set; }
    public bool areActive { get; set; }
}
