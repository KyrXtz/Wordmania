using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnergyManager : MonoBehaviour
{
    // Start is called before the first frame update
    public List<Image> energyBlocks;
    int energyCount;
    public TextMeshProUGUI timerText;
    int energiesAvailable;
    float timer;
    const float timeForEnergy = 10f;
    void Start()
    {
        energyCount = energyBlocks.Count;
        LoadState();
    }
    public void useEnergy()
    {
        energiesAvailable -= 2;
        //timer += 210f;
        PrefsWrapper.SetInt("Energies", energiesAvailable);
        drawEnergies();
        // PrefsWrapper.SetFloat("Timer", timer);

    }
    public void gainEnergy(int no)
    {
        energiesAvailable += no;
        PrefsWrapper.SetInt("Energies", energiesAvailable);
        LoadState();
    }
    public bool hasEnergy()
    {
        return energiesAvailable > 1;
    }
    // Update is called once per frame
    void Update()
    {
        if (energiesAvailable != energyCount)
        {
            if (!timerText.gameObject.activeSelf) timerText.gameObject.SetActive(true);
            if (timer <= 0)
            {
                energiesAvailable++;
                drawEnergies();
                PrefsWrapper.SetInt("Energies", energiesAvailable);
                if (energiesAvailable == energyCount)
                {
                    timerText.gameObject.SetActive(false);
                }

                timer = timeForEnergy;

            }
            else
            {
                timer -= Time.deltaTime;
                int minutes = Mathf.FloorToInt(timer / 60F);
                int seconds = Mathf.FloorToInt(timer - minutes * 60);
                if (minutes >= 0)
                {
                    timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
                }
            }


        }
    }
    private void OnApplicationQuit()
    {
        SaveState();
    }
    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SaveState();
            //Time.timeScale=0
        }
        else
        {
            LoadState();
        }
    }
    void SaveState()
    {
        PrefsWrapper.SetInt("Energies", energiesAvailable);
        PrefsWrapper.SetFloat("Timer", timer);
        PrefsWrapper.SetString("QuitTimestamp", DateTime.Now.ToString());
        PrefsWrapper.Save();
    }
    void LoadState()
    {
        energiesAvailable = PrefsWrapper.GetInt("Energies", energyCount);
        timer = PrefsWrapper.GetFloat("Timer", timeForEnergy);
        //timer = timeForEnergy; energiesAvailable = energyCount;
        if (energiesAvailable < energyCount)
        {
            var quitTS = DateTime.Parse(PrefsWrapper.GetString("QuitTimestamp"));
            var totalS = (float)(DateTime.Now - quitTS).TotalSeconds;
            var quantifiedTS = Mathf.FloorToInt(totalS / timeForEnergy);
            energiesAvailable = quantifiedTS + energiesAvailable > energyCount ? energyCount : energiesAvailable + quantifiedTS;
            //timer = timer - (totalS - quantifiedTS * 210.0f) <= 0 ? 210.0f : timer - (totalS - quantifiedTS * 210.0f);
            if (timer - (totalS - quantifiedTS * timeForEnergy) <= 0)
            {
                timer = timeForEnergy;
                energiesAvailable = energiesAvailable > energyCount ? energyCount : energiesAvailable + 1;
            }
            else
            {
                timer = timer - (totalS - quantifiedTS * timeForEnergy);
            }
        }

        if (energiesAvailable == energyCount)
        {
            timerText.gameObject.SetActive(false);
            timer = timeForEnergy;
        }
        drawEnergies();
    }
    void drawEnergies()
    {
        foreach (var eng in energyBlocks)
        {
            eng.color = Color.white;

        }
        for (int i = 0; i < energiesAvailable; i++)
        {
            energyBlocks[i].color = Color.green;
        }
    }
}
