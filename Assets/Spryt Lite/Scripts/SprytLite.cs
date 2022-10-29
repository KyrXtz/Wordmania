using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprytLite : MonoBehaviour
{
    ///<summary>The speed at which the frames are cycled.</summary>
    public float speed = 1f;

    ///<summary>Whether the animation is paused or playing.
    ///<para>You can use the Pause / Resume / Toggle Methods to Set this Property.</para></summary>
    public bool isPaused = false;
    
    ///<summary>The List of frames passed to the renderer</summary>
    public List<Sprite> frames = new List<Sprite>();
    
//Properties
    ///<summary>The current index of the frames.
    ///<para>The actual frame being displayed is this value floor-rounded.</para></summary>
    public float Frame { get { return _frame; } set {
        //Ensure the assigned frame is within the currently available frames
            value = Mathf.Clamp(value, 0f, frames.Count - 1f);
            _frame = value;
        //Update the renderer
            myRenderer.sprite = frames[Mathf.FloorToInt(_frame)];
            } }
    private float _frame;

    ///<summary>The number of frames in the current Spryt Index.
    ///<para>Set automatically when a new Spryt is assigned to the instance.</para></summary>
    public int Count { get; protected set; }

    ///<summary>Controls whether the Renderer is enabled or not.
    ///<para>When this property is false, the Update method is not executed (the frame index is not updated).</para></summary>
    public bool Visible { get { return _visible; } set { _visible = value; myRenderer.enabled = _visible; } }
    private bool _visible;

//Private
    ///<summary>The Sprite Renderer</summary>
    private Image myRenderer;

    ///<summary>The internal list of Frames used</summary>
    private List<Sprite> _frames = new List<Sprite>();

    ///<summary>If true, plays a Spryt's animation only once, then pauses it. Set by PlayOneShot Animation Method</summary>
    private bool playOneShot = false;

    ///<summary>If playOneShot is used, this controls whether the Spryt pauses on its last frame or loops back to its first.</summary>
    private bool pauseOnLastFrame = false;

//"Original" values as defined across various Components and used in Reset method.
    private bool oVisible;
    private float oSpeed;

    public virtual void Start() {
        //Assign either a SpriteRenderer or an Image Component to myRenderer depending on which is found on the GameObject
        Image spriteRenderer = GetComponent<Image>();
        if (spriteRenderer != null) {
            myRenderer = spriteRenderer;
        } else {
    //Display an error but add a SpriteRenderer as a fallback.
            Debug.LogError("Spryt requires either a Component of type SpriteRenderer or Image to operate! SpriteRenderer has been added as default.");
            spriteRenderer = gameObject.AddComponent<Image>();
            myRenderer = spriteRenderer;
        }
    //Set original values
        Visible = myRenderer.enabled;
        oVisible = Visible;
        oSpeed = speed;
    //Assign the frames placed on this instance to be used
        AssignFrames(frames);
    }
  
    protected void FixedUpdate() {
    //Only update the frame if the Spryt is Visible, is not paused, its frames are not null (and there is more than one frame), and the magnitude of its speed is not zero. 
        if (_visible && !isPaused && frames != null ) {
            if (Mathf.Abs(speed) > Mathf.Epsilon && frames.Count > 1) {
                if (speed > 0) {
            //Animation speed is positive
                    if (_frame + speed + 0.01 < frames.Count) { //0.01 because floating point arithmatic is unreliable
                        _frame += speed;
                    } else {
                //Loop back to the beginning
                        if (playOneShot) {
                    //If this is a one-shot animation, freeze the frame
                            Pause();
                            if (!pauseOnLastFrame)
                                _frame = 0;
                        } else {
                    //Go back to the beginning of the animation
                            _frame = 0;
                        }
                    }
                } else {
            //Animation speed is Negative
                    if (_frame + speed > Mathf.Epsilon)
                        _frame += speed;
                    else {
                //Loop back to the end
                        if (playOneShot) {
                    //If this is a one-shot animation, freeze the frame
                            Pause();
                            if (!pauseOnLastFrame)
                                _frame = frames.Count + speed;
                        } else {
                    //Go back to the end of the animation
                            _frame = frames.Count + speed;
                        }
                    }
                }
                //Update the renderer
                try {
                    myRenderer.sprite = _frames[Mathf.FloorToInt(_frame)];
                }
                catch
                {
                    frames = _frames; //kakos programatistis, eprepe na kanw ayto gia na doylepsei. tha mporousa na grapsw allh AssignFrames alla bareimai
                }
            }
        }
    }

//Animation Methods
    ///<summary>Pauses the animation, freezing the Spryt on the current frame.</summary>
    public void Pause() {
        isPaused = true;
    }

    /// <summary>Pauses the animation on a specified frame.</summary>
    /// <param name="_frame">The frame of the animation to pause on.</param>
    public void Pause(int _frame) { //Assign a specific frame and Pause
        Pause();
        this._frame = Mathf.FloorToInt(_frame);
        myRenderer.sprite = _frames[Mathf.FloorToInt(this._frame)];
    }
    
    ///<summary>Resumes playing the animation and disables "playOneShot" behaviour.</summary>
    public void Resume() {
        isPaused = false;
        playOneShot = false;
    }
    
    ///<summary>Pauses the animation or Resumes it depending on its current state</summary>
    public void Toggle() {
        if (isPaused) {
            Resume();
        } else {
            Pause();
        }
    }

    ///<summary>Pause on the Last frame</summary>
    public void Last() { Pause(_frames.Count - 1); }

    ///<summary>Pause on the First frame</summary>
    public void First() { Pause(0); }

    ///<summary>Inverts the animation speed.
    ///<para>If the animation is on the first or last frame, it jumps to the end or beginning.</para></summary>
    public void Reverse() {
        speed *= -1;
        if (_frame < Mathf.Epsilon) {
            _frame = _frames.Count + speed;
        } else if (_frame + speed + 0.01 > _frames.Count) { //0.01 because floating point arithmatic is unreliable
            _frame = 0;
        }
    }

    /// <summary>Plays the animation once through, then pauses it.
    /// <para>Automatically jumps to the first frame if speed is positive, or the last frame if speed is negative.</para>
    /// <para>Also automatically unpauses the animation.</para></summary>
    /// <param name="_pauseOnLastFrame">When the animation ends, should it stop on the last frame or cycle back to the beginning?</param>
    public void PlayOneShot(bool _pauseOnLastFrame) {
        isPaused = false;
        playOneShot = true;
        pauseOnLastFrame = _pauseOnLastFrame;
        if (speed < -Mathf.Epsilon)
            _frame = _frames.Count + speed;
        else
            _frame = 0f;
    }

//Utility Methods
    /// <summary>A publicly accessible Method which can be used to assign a List of Sprites to the internal List used by Spryt.
    /// <para>This method is called each time a new Spryt is assigned to the Index of this instance.</para></summary>
    /// <param name="sprites">A List of type Sprite, in the order the frames should be shown.</param>
    public  void AssignFrames(List<Sprite> sprites) {
       
        _frames.Clear();
        //frames.Clear();
        Count = sprites.Count; //Set the Count property to match the new total of frames
        if (Count == 0) {
            Debug.LogError("Call to 'AssignFrames' with an Empty List");
        } else {
        //Assign list of Sprites to "_frames"
            foreach (Sprite sprite in sprites) {
                _frames.Add(sprite);
              //  frames.Add(sprite);
            }
        //Reset the frame index based on whether speed is positive or negative
            _frame = 0f;
            if (speed < -Mathf.Epsilon)
                _frame = _frames.Count + speed;
        //Update the renderer
            myRenderer.sprite = _frames[0];
        }
    }

    /// <summary>Resets all properties of the Spryt to default values as defined on the GameObject in the Inspector.</summary>
    public virtual void ResetSprite() {
        Visible = oVisible;
        speed = oSpeed;
        if (speed < -Mathf.Epsilon)
            _frame = _frames.Count + speed;
        else
            _frame = 0f;
    }
}