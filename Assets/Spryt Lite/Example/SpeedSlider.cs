using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedSlider : MonoBehaviour {

	public SprytLite spryt; //Reference to the Spryt
    public Slider slider; //Reference to the Speed Slider
	
//When the Slider Value updates, pass it along to the Spryt
	public void UpdateSpeed () {
		spryt.speed = slider.value;
	}
}
