using Google.Play.Review;
using Gravitons.UI.Modal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAppReview : MonoBehaviour
{
    // Start is called before the first frame update
    public LevelManager lvlManager;
    public PlayGamesController playGamesController;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void askForReview()
    {
        ModalManager.Show("One time offer!", "Rate us with 5 stars and earn 300 coins!\nA rate window will open inside the app , you dont even have to close it!", lvlManager.iconsForModals[7], new[] {new ModalButton(){Text = "Later" } ,
                new ModalButton() { Text = "Ok!" ,
                Callback = () => {
                    if (!PrefsWrapper.HasKey("HasRated"))
                    {
                        if(LevelManager.checkInternetConnection()){
                           StartCoroutine(startRate());

                        }
                        else
                        {
                            ModalManager.Show("Oops!", "Something went wrong, check your internet connection.", lvlManager.iconsForModals[6], new[] {new ModalButton(){Text = "Ok.." } ,
                                    });
                        }
                        
                        
                    }


                } } });
    }

    IEnumerator startRate()
    {
        ModalManager.Show("...", "Just a moment...", lvlManager.iconsForModals[7], null);
        yield return new WaitForSeconds(1f);
        var _reviewManager = new ReviewManager();
        var requestFlowOperation = _reviewManager.RequestReviewFlow();
        yield return requestFlowOperation;
        if (requestFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            ModalManager.Close();
            ModalManager.Show("Oops!", "Something went wrong, try again later.", lvlManager.iconsForModals[6], new[] {new ModalButton(){Text = "Ok..." } ,
        });
            yield break;
        }
        var _playReviewInfo = requestFlowOperation.GetResult();
        var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
        yield return launchFlowOperation;
        _playReviewInfo = null; // Reset the object
        if (launchFlowOperation.Error != ReviewErrorCode.NoError)
        {
            // Log error. For example, using requestFlowOperation.Error.ToString().
            ModalManager.Close();
            ModalManager.Show("Oops!", "Something went wrong, try again later.", lvlManager.iconsForModals[6], new[] {new ModalButton(){Text = "Ok..." } ,
        });
            yield break;
        }
        ModalManager.Close();
        ModalManager.Show("Thanks!", "You win 300 coins!", lvlManager.iconsForModals[7], new[] {new ModalButton(){Text = "Nice!" } ,
        });
        lvlManager.updateCoins(300,false);
        PrefsWrapper.SetInt("HasRated", 1);
        PrefsWrapper.Save();
        playGamesController.GetComponent<Achievements>().Rated();

    }


}
