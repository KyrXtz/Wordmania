using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // Start is called before the first frame update
    List<int> wavIds = new List<int>();
    List<int> letterIds = new List<int>();
    List<int> extraletterIds = new List<int>();
    List<int> songs = new List<int>();
    int activeSong;
    bool isSfxOn;
    bool isMusicOn;
    public Toggle SFXToggle;
    public Toggle MusicToggle;

    void Start()
    {
        
        


        //ANAMusic.makePool(files.Count);
        //  int fileID = AndroidNativeAudio.load("Effect.wav");

        AndroidNativeAudio.makePool(10);
        wavIds.Add(AndroidNativeAudio.load("startButton.wav"));  //index = 0
        wavIds.Add(AndroidNativeAudio.load("wrong.wav"));  //index = 1     
        wavIds.Add(AndroidNativeAudio.load("wordFound.wav"));  //index = 2
        wavIds.Add(AndroidNativeAudio.load("shuffle.wav"));  //index = 3
        wavIds.Add(AndroidNativeAudio.load("bigWordFound.wav"));  //index = 4
        wavIds.Add(AndroidNativeAudio.load("extraWordFound.wav"));  //index = 5
        wavIds.Add(AndroidNativeAudio.load("extraBigWordFound.wav"));  //index = 6
        wavIds.Add(AndroidNativeAudio.load("coin.wav"));  //index = 7
        wavIds.Add(AndroidNativeAudio.load("Cancel.wav"));  //index = 8






        letterIds.Add(AndroidNativeAudio.load("l1.wav"));  //index = 0
        letterIds.Add(AndroidNativeAudio.load("l2.wav"));  //index = 1
        letterIds.Add(AndroidNativeAudio.load("l3.wav"));  //index = 2
        letterIds.Add(AndroidNativeAudio.load("l4.wav"));  //index = 3
        letterIds.Add(AndroidNativeAudio.load("l5.wav"));  //index = 4
        letterIds.Add(AndroidNativeAudio.load("l6.wav"));  //index = 5
        letterIds.Add(AndroidNativeAudio.load("l7.wav"));  //index = 6
        letterIds.Add(AndroidNativeAudio.load("l8.wav"));  //index = 7
        letterIds.Add(AndroidNativeAudio.load("l9.wav"));  //index = 8
        letterIds.Add(AndroidNativeAudio.load("l10.wav"));  //index = 9
        letterIds.Add(AndroidNativeAudio.load("l11.wav"));  //index = 10
        letterIds.Add(AndroidNativeAudio.load("l12.wav"));  //index = 11

        extraletterIds.Add(AndroidNativeAudio.load("e1.wav"));  //index = 0
        extraletterIds.Add(AndroidNativeAudio.load("e2.wav"));  //index = 1
        extraletterIds.Add(AndroidNativeAudio.load("e3.wav"));  //index = 2
        extraletterIds.Add(AndroidNativeAudio.load("e4.wav"));  //index = 3
        extraletterIds.Add(AndroidNativeAudio.load("e5.wav"));  //index = 4
        extraletterIds.Add(AndroidNativeAudio.load("e6.wav"));  //index = 5
        extraletterIds.Add(AndroidNativeAudio.load("e7.wav"));  //index = 6
        extraletterIds.Add(AndroidNativeAudio.load("e8.wav"));  //index = 7
        extraletterIds.Add(AndroidNativeAudio.load("e9.wav"));  //index = 8
        extraletterIds.Add(AndroidNativeAudio.load("e10.wav"));  //index = 9
        extraletterIds.Add(AndroidNativeAudio.load("e11.wav"));  //index = 10
        extraletterIds.Add(AndroidNativeAudio.load("e12.wav"));  //index = 11

        


        isSfxOn = PrefsWrapper.GetInt("IsSFXOn", 1) == 1;
        SFXToggle.SetIsOnWithoutNotify(isSfxOn);
        isMusicOn = PrefsWrapper.GetInt("IsMusicOn", 1) == 1;
        MusicToggle.SetIsOnWithoutNotify(isMusicOn);
        ANAMusic.load("song1.ogg", false, true, (id) => { songs.Add(id); if (isMusicOn) ToggleMusic(isMusicOn); });




        //int streamID = AndroidNativeAudio.play(fileID);

        //AndroidNativeAudio.pause(streamID);
        //AndroidNativeAudio.resume(streamID);
        //AndroidNativeAudio.setVolume(streamID, 0.5f);

        //AndroidNativeAudio.unload(fileID);
        //AndroidNativeAudio.releasePool();

    }
    private void playSoundWithCheck(int id)
    {
        if (isSfxOn) AndroidNativeAudio.play(id);
    }
    public void CancelSound()
    {
        stopLetterSounds();
        playSoundWithCheck(wavIds[8]);
    }
    public void startGameSound()
    {
        playSoundWithCheck(wavIds[0]);
    }
    public void wrongSound()
    {
        stopLetterSounds();
        playSoundWithCheck(wavIds[1]);
    }
    public void coinSound()
    {
        AndroidNativeAudio.stop(wavIds[7]);
        playSoundWithCheck(wavIds[7]);
    }
    public void wordFoundSound()
    {
        stopLetterSounds();
        if (currentLetter >= 7)
        {
            playSoundWithCheck(wavIds[4]);

        }
        else
        {
            playSoundWithCheck(wavIds[2]);

        }
        currentLetter = 0;
    }
    public void extraWordFoundSound()
    {
        stopLetterSounds();
        if (currentLetter >= 6)
        {
            playSoundWithCheck(wavIds[6]);

        }
        else
        {
            playSoundWithCheck(wavIds[5]);

        }
        currentLetter = 0;
    }
    public void shuffleSound()
    {
        playSoundWithCheck(wavIds[3]);
    }
    int currentLetter = 0;
    public void letterSound(int ind, bool isExtra)
    {
        stopLetterSounds();
        try
        {
            currentLetter = ind;
            if (isExtra)
            {
                playSoundWithCheck(extraletterIds[ind]);
            }
            else
            {
                playSoundWithCheck(letterIds[ind]);

            }
        }
        catch
        {

        }
    }
    void stopLetterSounds()
    {
        foreach (var sound in letterIds)
        {
            AndroidNativeAudio.stop(sound);
        }
        foreach (var sound in extraletterIds)
        {
            AndroidNativeAudio.stop(sound);
        }
    }

    public void ToggleSoundButton()
    {
        PrefsWrapper.SetInt("IsSFXOn", PrefsWrapper.GetInt("IsSFXOn", 1) == 1 ? 0 : 1);
        PrefsWrapper.Save();
        isSfxOn = PrefsWrapper.GetInt("IsSFXOn", 1) == 1;
    }
    public void ToggleMusicButton()
    {
        PrefsWrapper.SetInt("IsMusicOn", PrefsWrapper.GetInt("IsMusicOn", 1) == 1 ? 0 : 1);
        PrefsWrapper.Save();
        isMusicOn = PrefsWrapper.GetInt("IsMusicOn", 1) == 1;
        ToggleMusic(isMusicOn);
    }
    void ToggleMusic(bool isMusicOn)
    {
        if (isMusicOn)
        {
            activeSong = songs[0];
            ANAMusic.play(activeSong);
            ANAMusic.setLooping(activeSong, true);
        }
        else
        {
            ANAMusic.seekTo(activeSong, 0);
            ANAMusic.pause(activeSong);
        }

    }
}
