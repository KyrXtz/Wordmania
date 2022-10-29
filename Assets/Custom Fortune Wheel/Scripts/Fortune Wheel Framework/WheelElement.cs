using UnityEngine;

[System.Serializable]
/// <summary>
/// The unit object of the wheel elements. <br>
/// Contains all the slice properties.
/// </summary>
public class WheelElement
{

    /// <summary>
    /// The label written on the wheel element. This label is also used as a reference for the wheel element.<br>
    /// Default value is "New Slice".
    /// </summary>
    public string label;
    /// <summary>
    /// The color of the label on the wheel. <br>
    /// Default color is white.
    /// </summary>
    public Color labelColor;

    /// <summary>
    /// The font size of the wheel label. It ranges between 4 and 10.
    /// </summary>
    [Range(4, 10)]
    public float fontSize;
    /// <summary>
    /// Determines whether the label will be visible on the wheel element or not.<br>
    /// Default value is true.
    /// </summary>
    public bool showLabelInWheel = true;

    /// <summary>
    /// The main color of the slice.<br>
    /// Default color is black.
    /// </summary>
    public Color fillColor;
    
    /// <summary>
    /// The icon on the wheel element.<br>
    /// This icon size is adapted to the wheel element size with few controls in the size.
    /// If you leave it blank, it will not be drawn.
    /// </summary>
    public Sprite icon;
    
    public WheelElement()
    {
        this.label = "New Slice";
        this.labelColor =  new Color(0, 0, 0, 1);
        this.fontSize = 5;
        this.fillColor = new Color(1, 1, 1, 1);
    }

}
