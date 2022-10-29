using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Threading;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using GooglePlayGames.BasicApi.SavedGame;
using Gravitons.UI.Modal;


public class PlayGamesController : MonoBehaviour
{
    public static bool isAuthenticated = false;
    public Button CloudUp;
    public Button CloudDown;
    public Button CloudAuto;
    public LevelManager levelManager;
    private void Start()
    {
        AuthenticateUser();
    }
    private void Update()
    {
        CloudDown.interactable = !isRunning;
        CloudUp.interactable = !isRunning;
        CloudAuto.interactable = !isRunning;
    }
    void AuthenticateUser()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder().EnableSavedGames().Build();
        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.Activate();
        Social.localUser.Authenticate((bool success) =>
        {
            if (success == true)
            {
                Debug.Log("Logged in to Google Play Games Services");
                isAuthenticated = true;
                askForBackup();
            }
            else
            {
                Debug.LogError("Unable to sign in to Google Play Games Services");

            }
        });

    }


    bool isImport;
    bool isRunning;
    bool showModals;
    void askForBackup()
    {
        var x = PrefsWrapper.GetInt("AskForBackup", 0);
        if (x == 2)
        {
            checkForOldBackup();
        }
        else if (x > 0)
        {
            PrefsWrapper.SetInt("AskForBackup", x + 1);
        }
    }
    void checkAndAutoBackup()
    {
        if(PrefsWrapper.GetInt("AskForBackup", 0) == -2)
        {
            isRunning = true;
            isImport = false;
            showModals = false;
            loadSaveState();
        }
    }
    public void OnUserQuit()
    {
        checkAndAutoBackup();
        StartCoroutine(QuitWhenSaved());

    }
    //Auto backup enable/disable
    public void CloudAutoButton()
    {
        var x = PrefsWrapper.GetInt("AskForBackup", 0);
        if (x >= 0)
        {
            checkForOldBackup();
        }
        else if (x == -2)
        {
            ModalManager.Show("Auto cloud backup", "Automatic cloud backup is enabled. Disable?", levelManager.iconsForModals[1], new[] {new ModalButton(){Text = "No" } ,
                new ModalButton() { Text = "Yes" ,
                Callback = () => {
                    ModalManager.Show("Success!", "Automatic cloud backup is disabled!", levelManager.iconsForModals[10], new[] {new ModalButton(){Text = "Nice!" } });
                    PrefsWrapper.SetInt("AskForBackup", -1);
                    PrefsWrapper.Save();
                } } });
        }
        else if (x == -1)
        {
            ModalManager.Show("Auto cloud backup", "Automatic cloud backup is disabled. Enable?", levelManager.iconsForModals[1], new[] {new ModalButton(){Text = "No" } ,
                new ModalButton() { Text = "Yes" ,
                Callback = () => {
                    ModalManager.Show("Success!", "Automatic cloud backup is enabled!", levelManager.iconsForModals[10], new[] {new ModalButton(){Text = "Nice!" } });
                    PrefsWrapper.SetInt("AskForBackup", -2);
                    PrefsWrapper.Save();
                } } });
        }

    }
    IEnumerator QuitWhenSaved()
    {
        ModalManager.Show("...", "Saving to the cloud..", levelManager.iconsForModals[1], null);
        yield return new WaitUntil(() => !isRunning);
        Application.Quit();
    }
    //Save Game
    public void saveGameButton()
    {
        var hasBackedUp = PrefsWrapper.GetInt("HasBackedUp", 0);
        ModalManager.Show("Cloud backup", "Backup your progress in the cloud?" + (hasBackedUp == 1 ? "\n Any existing backups will be deleted!" : ""), levelManager.iconsForModals[1], new[] {new ModalButton(){Text = "No" } ,
                new ModalButton() { Text = "Yes" ,
                Callback = () => {
                    ModalManager.Show("...", "Collecting files..", levelManager.iconsForModals[1], null);
                    isRunning = true;
                    isImport = false;
                    showModals = true;
                    loadSaveState();
                } } });

    }
    public void loadGameButton()
    {
        ModalManager.Show("...", "Searching the cloud..", levelManager.iconsForModals[1], null);
        isRunning = true;
        isImport = true;
        showModals = true;
        loadSaveState();
    }
    void SaveGameData(ISavedGameMetadata metadata)
    {
        string convert = PrefsWrapper.GetString("GlobalSave");
        // From string to byte array
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(convert);
        // From byte array to string
        // string s = System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length);

        SaveGame(metadata, buffer, metadata.TotalTimePlayed);
    }
    void SaveGame(ISavedGameMetadata game, byte[] savedData, System.TimeSpan totalPlaytime)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;

        SavedGameMetadataUpdate.Builder builder = new SavedGameMetadataUpdate.Builder();
        builder = builder
            .WithUpdatedPlayedTime(totalPlaytime)
            .WithUpdatedDescription("Saved game at " + System.DateTime.Now);

        SavedGameMetadataUpdate updatedMetadata = builder.Build();
        savedGameClient.CommitUpdate(game, updatedMetadata, savedData, OnSavedGameWritten);
    }
    void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // handle reading or writing of saved game.
            PrefsWrapper.SetInt("HasBackedUp", 1);
            if(showModals)
            ModalManager.Show("Success", "Your progress was saved in the cloud!", levelManager.iconsForModals[1], new[] { new ModalButton() { Text = "Nice!" } });
            isRunning = false;
        }
        else
        {
            Debug.LogError("Game not saved");
            // handle error
        }
    }
    //Load Save State
    /// <summary>
    /// eite gia read eite gia write prepei na treksei
    /// </summary>
    public void loadSaveState()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        if (savedGameClient == null) ModalManager.Show("Oops!", "You're not signed in to google play games services, or you're not connected to the internet.", levelManager.iconsForModals[6], new[] { new ModalButton() { Text = "Ok", Callback = () => { isRunning = false; } } });
        else
            savedGameClient.OpenWithAutomaticConflictResolution("cloudsave", DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, onSaveStateLoaded);
    }
    void onSaveStateLoaded(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // handle reading or writing of saved game.
            if (isImport) ReadGameData(game);
            else SaveGameData(game);


        }
        else
        {
            // handle error
            Debug.LogError("coudlnt load save state");
            Debug.LogError(status);


        }
    }
    //ReadGame

    void ReadGameData(ISavedGameMetadata game)
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        savedGameClient.ReadBinaryData(game, OnSavedGameDataRead);
    }

    void OnSavedGameDataRead(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // handle processing the byte array data
            string s = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
            if (string.IsNullOrEmpty(s))
            {
                ModalManager.Show("Oops!", "There is no backup in the cloud!", levelManager.iconsForModals[6], new[] { new ModalButton() { Text = "Ok.." } });
                isRunning = false;
            }
            else
            {
                ModalManager.Show("Restore", "Restore backup ? Any unsaved progress will be lost!", levelManager.iconsForModals[0], new[] {new ModalButton(){Text = "Cancel", Callback = ()=> { isRunning = false; } } ,
                new ModalButton() { Text = "Yes!" ,
                Callback = () => { PrefsWrapper.ImportDataFromCloud(s); levelManager.InitLevelParameters(); isRunning = false;
                                   ModalManager.Show("Success", "Your progress was restored!", levelManager.iconsForModals[1], new[] { new ModalButton() { Text = "Nice!" } });

                } } });
            }


            // Debug.Log(s);

        }
        else
        {
            // handle error
            Debug.LogError("coudlnt read save state");
            Debug.LogError(status);
        }
    }

    #region autobackup
    void checkForOldBackup()
    {
        ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
        if (savedGameClient != null)        
            savedGameClient.OpenWithAutomaticConflictResolution("cloudsave", DataSource.ReadCacheOrNetwork,
                ConflictResolutionStrategy.UseLongestPlaytime, checkIfOldFileDone);       
    }
    void checkIfOldFileDone(SavedGameRequestStatus status, ISavedGameMetadata game)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            ISavedGameClient savedGameClient = PlayGamesPlatform.Instance.SavedGame;
            savedGameClient.ReadBinaryData(game, onDone);

        }
    }
    void onDone(SavedGameRequestStatus status, byte[] data)
    {
        if (status == SavedGameRequestStatus.Success)
        {
            // handle processing the byte array data
            string s = System.Text.Encoding.UTF8.GetString(data, 0, data.Length);           
            finallyAskForAutoBackup(!string.IsNullOrEmpty(s));

        }
    }
    void finallyAskForAutoBackup(bool hasExistingBackup)
    {
        //AskForBackup = -1 dont remind later , -2 = auto backup enabled        
        ModalManager.Show("Cloud backup", "Do you want to enable auto cloud backup? We will save your current game state every time you close the app.", levelManager.iconsForModals[1], new[] {
                new ModalButton(){Text = "Remind me later",Callback=()=>{PrefsWrapper.SetInt("AskForBackup",0); } } ,
                new ModalButton(){Text = "No",Callback=()=>{PrefsWrapper.SetInt("AskForBackup",-1); } } ,
                new ModalButton() { Text = "Yes" ,
                Callback = () => {
                    if (hasExistingBackup)
                    {
                         ModalManager.Show("Warning", "We found an existing backup in the cloud. Do you want to retrieve it or overwrite it with the current progress?", levelManager.iconsForModals[0], new[] {
                                        new ModalButton(){Text = "Retrieve",
                                        Callback=()=>{
                                            PrefsWrapper.SetInt("AskForBackup",-2);
                                            isRunning = true;
                                            isImport = true;
                                            showModals = false;
                                            loadSaveState();
                                            ModalManager.Show("Done!", "Auto cloud backup enabled!", levelManager.iconsForModals[10], new[] { new ModalButton() { Text = "Nice!" } });
                                        } } ,                                       
                                        new ModalButton() { Text = "Overwrite" ,
                                        Callback = () => {
                                            PrefsWrapper.SetInt("AskForBackup",-2);
                                            isRunning = true;
                                            isImport = false;
                                            showModals = false;
                                            loadSaveState();
                                            ModalManager.Show("Done!", "Auto cloud backup enabled!", levelManager.iconsForModals[10], new[] { new ModalButton() { Text = "Nice!" } });

                                        } } });
                    }
                    else
                    {
                        PrefsWrapper.SetInt("AskForBackup",-2);
                        isRunning = true;
                        isImport = false;
                        showModals = false;
                        loadSaveState();
                        ModalManager.Show("Done!", "Auto cloud backup enabled!", levelManager.iconsForModals[10], new[] { new ModalButton() { Text = "Nice!" } });
                    }
                } } });      
    }
    #endregion
}
