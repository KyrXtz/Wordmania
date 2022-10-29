using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary> 
/// Create and customize your own Fortune Wheel.
/// </summary>
public class FortuneWheel : FortuneMachine
{
    const float CIRCLE_RADIUS = 35f;
    #region Public Members
    public UnityEngine.Object btn;
    public UnityEngine.Object pnt;

    /// <summary> 
    /// Scale size of the wheel. 
    /// </summary>
    public float wheelSize = 3f;
    /// <summary> 
    /// Controller of the wheel rotating speed.
    /// </summary>
    public float wheelSpeed = 200f;

    /// <summary> 
    /// The outline thickness around each slice in the wheel.
    /// </summary>
    public float sliceThickness = 0f;

    /// <summary> 
    /// Specify the orientation of labels on wheel. 
    /// </summary>
    public float labelRotation = 0f;

    /// <summary> 
    /// The offset of the icons on wheel from the origin till the end of the wheel.
    /// </summary>
    public float iconOffset = 2f;

    /// <summary>
    /// The offset of the labels on wheel from the origin till the end of the wheel.
    /// </summary>
    public float labelOffset = 1f;

    /// <summary>
    /// The scale size of the wheel pointer.
    /// </summary>
    public float pointerScaleSize = 1;
    /// <summary> 
    /// Create the wheel automatically at the start of the game. 
    /// </summary>
    public bool createOnStart = true;
    public Color sliceOutlineColor = new Color(1, 1, 1, 1);
    public Sprite pointerIcon;
    public TMP_FontAsset font;
    /// <summary> 
    /// The postion of the wheel pointer. Note that the pointer doesn't change its rotation relative to its postion. 
    /// </summary>
    public int pointerPositionIndex = 0;

    /// <summary> 
    /// The elements in the wheel. 
    /// </summary>
    public List<WheelElement> slices;
    #endregion
    #region  Private Members
    GameObject _circleAsset;
    GameObject _pointerAsset;
    GameObject _outlineAsset;
    int _wheelCycles;
    int _fortuneSize;
    int _rotationIterations = 0;
    int _randomSelectedChioceID = -1;
    bool _isSpinning=false;

    float _sliceProp;
    int _latestTickStats;
    Dictionary<int, string> _slicesStats = new Dictionary<int, string>();
    Tuple<int, string> _result;
    GameObject wheel;
    Vector3 offsetDistance;
    #endregion

    void Reset()
    {
        slices = new List<WheelElement>();
        slices.Add(new WheelElement());
        slices.Add(new WheelElement());

        _pointerAsset = Resources.Load<GameObject>("Prefabs/Pointer");

        pointerIcon = _pointerAsset.GetComponent<Image>().sprite;

        font = Resources.Load<TMP_FontAsset>("Fonts/Roboto/Roboto-Regular SDF");
    }

    void Awake()
    {
        _pointerAsset = Resources.Load<GameObject>("Prefabs/Pointer");
        _circleAsset = Resources.Load<GameObject>("Prefabs/Circle");
        _outlineAsset = Resources.Load<GameObject>("Prefabs/Outline");


        wheel = new GameObject("Wheel");
        wheel.transform.SetParent(transform);    
        wheel.transform.localPosition = Vector3.zero;
        

        _fortuneSize = slices.Capacity;

        if (doubleChance) _fortuneSize *= 2;
        _sliceProp = 1.0f / _fortuneSize;

        if(createOnStart)
            CreateFortune();
    }

   
    public override void CreateFortune()
    {
        offsetDistance = new Vector3(2 * Mathf.PI * 0.4f * _sliceProp * CIRCLE_RADIUS, CIRCLE_RADIUS);
        offsetDistance.x += sliceThickness * 2 * _sliceProp;

        Image[] imageSlices = new Image[_fortuneSize];

        GameObject outlineBackground = Instantiate(_outlineAsset, wheel.transform);
        //outlineBackground.transform.SetParent(wheel.transform);
        outlineBackground.transform.localPosition = Vector3.zero;
        // outlineBackground.transform.localScale += Vector3.one * (sliceThickness / 20);
        outlineBackground.GetComponent<RectTransform>().sizeDelta += Vector2.one * (sliceThickness / 20);
        outlineBackground.GetComponent<RectTransform>().sizeDelta += Vector2.one * 10;
        outlineBackground.GetComponent<RectTransform>().sizeDelta *= wheelSize;
        outlineBackground.GetComponent<Image>().color = sliceOutlineColor;

        SpawnElements(ref imageSlices);
        //Set text for each slice
        SetSlicesText(imageSlices);
        // fill colors
        FillSlicesColors(ref imageSlices);
        // put icon
        SetSlicesIcons(imageSlices);

        //transform.localScale *= wheelSize;
      //  if (btn != null)(btn as GameObject).transform.localScale *= wheelSize;
        if (pnt != null) (pnt as GameObject).transform.SetAsLastSibling();
        if (btn != null) (btn as GameObject).transform.GetComponent<RectTransform>().sizeDelta *= wheelSize;
        if (pnt != null) (pnt as GameObject).transform.GetComponent<RectTransform>().sizeDelta *= wheelSize;
        this.gameObject.transform.GetComponent<RectTransform>().sizeDelta *= wheelSize;
        this.gameObject.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta *= wheelSize;


        SpawnPointer();
    }
    void SpawnElements(ref Image[] imageSlices)
    {
        float totalSliceProp = 0;
        for(int i = 0;i < _fortuneSize;++i)
        {
            int index = i % ( (int)_fortuneSize * (doubleChance? 2 : 4) / 4 );
            // spawn a slice and set it up
            GameObject newSlice = Instantiate(_circleAsset, wheel.transform);
            //newSlice.transform.SetParent(wheel.transform);
            newSlice.transform.localScale *=wheelSize;
            newSlice.transform.localPosition = new Vector3(0, 0, 0);
            newSlice.name = slices[index].label;

            totalSliceProp += _sliceProp;
            float rotAngle = 360 * (1 + _sliceProp - totalSliceProp);
            newSlice.transform.Rotate(0, 0, rotAngle);

            newSlice.GetComponent<Slice>().SetStats(new Tuple<int, string>(i, slices[index].label));

            Image img = newSlice.GetComponent<Image>();
            imageSlices[i] = img;
            
            // Add the metadata
            _slicesStats.Add(i, slices[index].label);
        }
    }
    void SetSlicesText(Image[] imageSlices)
    {
        for(int i = 0;i < _fortuneSize;++i)
        {
            int index =  i % ( (int)_fortuneSize * (doubleChance? 2: 4) /4 );
            
            TMP_Text labelText = imageSlices[i].GetComponentInChildren<TMP_Text>();
            labelText.transform.Rotate(0, 0, labelRotation);
            labelText.color = slices[index].labelColor;
            labelText.fontSize = slices[index].fontSize;
            labelText.font = font;


            if(!slices[index].showLabelInWheel) { labelText.gameObject.SetActive(false) ; continue; }
            labelText.text = slices[index].label;

            
            labelText.transform.localPosition = offsetDistance * labelOffset;
        }
    }
    void FillSlicesColors(ref Image[] imageSlices)
    {
        for(int i = 0; i < _fortuneSize; ++i)
        {
            int index =  i % ( (int)_fortuneSize * (doubleChance? 2: 4) /4 );
            imageSlices[i].fillAmount = _sliceProp;
            imageSlices[i].color = slices[index].fillColor;

            Outline outline = imageSlices[i].GetComponent<Outline>();
            outline.effectDistance = new Vector2(sliceThickness, - sliceThickness);
            outline.effectColor = sliceOutlineColor;

        }
    }
    void SetSlicesIcons(Image[] imageSlices)
    {
        for(int i = 0; i < _fortuneSize; ++i)
        {
            int index =  i % ( (int)_fortuneSize * (doubleChance? 2 : 4) /4 );
            
            Image icon = imageSlices[i].transform.GetChild(0).GetComponent<Image>();

            if(slices[index].icon == null) { icon.gameObject.SetActive(false); continue; }

            icon.transform.localPosition = offsetDistance * iconOffset;
           // icon.transform.localScale *= Mathf.Clamp(6 * _sliceProp, 0, 1.2f);
            icon.gameObject.GetComponent<RectTransform>().sizeDelta *= Mathf.Clamp(6 * _sliceProp, 0, 1.2f);

            icon.sprite = slices[index].icon;
            icon.preserveAspect = true;
        }
    }
    void SpawnPointer()
    {
        GameObject pointer = Instantiate(_pointerAsset);
        pointer.transform.SetParent(transform);
        pointer.transform.localScale *= wheelSize * pointerScaleSize;
        pointer.GetComponent<Image>().sprite = pointerIcon;

        float radius = CIRCLE_RADIUS + 5;

        Transform pointerCollider = pointer.transform.GetChild(0);

        switch(pointerPositionIndex)
        {
            case ((int)PointerPosition.Top):
                pointer.transform.localPosition = new Vector3(0, transform.localPosition.y + 
                        radius, 0);
                pointerCollider.Rotate(0, 0, 180);
                break;

            case ((int)PointerPosition.Botton):
                pointer.transform.localPosition = new Vector3(0, transform.localPosition.y - 
                        radius, 0);
                pointerCollider.Rotate(0, 0, 0);        
                break;
                
            case ((int)PointerPosition.Right):
                pointer.transform.localPosition = new Vector3(transform.localPosition.x +
                        radius, 0, 0);
                pointerCollider.Rotate(0, 0, 90);
                break;

            case ((int)PointerPosition.Left):
                pointer.transform.localPosition = new Vector3(transform.localPosition.x -
                        radius, 0, 0);
                pointerCollider.Rotate(0, 0, 270);
                break;

            case ((int)PointerPosition.Center):
                pointer.transform.localPosition = Vector3.zero;
                break;
        }
    }
    public override IEnumerator StartFortune()
    {
        if (_isSpinning) yield break;

        _isSpinning = true;
        _rotationIterations = UnityEngine.Random.Range(3, 5);
        _randomSelectedChioceID = UnityEngine.Random.Range(0, _fortuneSize);
        _randomSelectedChioceID %= ( (int)_fortuneSize * (doubleChance? 2 : 4) /4 );
        //print(_randomSelectedChioceID);

        StartCoroutine( RollWheel() );
        yield return new WaitUntil( () => _rotationIterations == 0);
        StopCoroutine( RollWheel() );
        _isSpinning = false;

        _result = new Tuple<int, string>(_latestTickStats, _slicesStats[_latestTickStats]);
       // Debug.Log(_result.Item2);
        GetLatestResult();  
        //print(_result);
    }
    
    IEnumerator RollWheel()
    {
        float randWait = (wheelSpeed / 50f) * _rotationIterations;
        float speed = wheelSpeed;
        yield return new WaitUntil( () => rotateWheel(ref speed, randWait) );
        _rotationIterations = 0;
        wheel.transform.Rotate(0, 0, 360 * Mathf.Epsilon);
        float angle = ( 360 * (_sliceProp) ) / 2;
        float time = Mathf.Abs(angle) / (speed / 2);
        yield return new WaitUntil( () => rotateToSliceCenter(speed / 2, ref time) );
    }
    bool rotateWheel(ref float speed, float deceleration)
    {    
        wheel.transform.Rotate(0, 0, speed * Time.deltaTime);
        speed -= Time.deltaTime * deceleration;
        speed = Mathf.Clamp(speed, 0, wheelSpeed);
        //Debug.LogError(speed);

        return speed == 0;
    }
    bool rotateToSliceCenter(float speed, ref float time)
    {
        wheel.transform.Rotate(0, 0, speed * Time.deltaTime);   
        time -= Time.deltaTime;

        return time <= 0;
    }
    public void RecieveLatestTick(int id)
    {
        _latestTickStats = id;
        if(id == _randomSelectedChioceID)
            -- _rotationIterations;
    }
    public override string GetLatestResult()
    {
        try
        {
            Debug.Log(_result.Item2);
            return _result.Item2;
        }catch (Exception exc)
        {
            Debug.LogWarning(exc.Message);
            return null;
        }
    }
    public override int GetLatestResultID()
    {
        return _result.Item1;
    }
    public enum PointerPosition
    {
        Top,
        Botton,
        Right,
        Left,
        Center
    }
    /// <summary>
    /// Create a default instance from the fortune wheel, two slices minimum.
    /// </summary>
    public FortuneWheel() {}
    /// <summary>
    /// A much more detailed constructor to make a FortuneWheel.
    /// </summary>
    /// <param name="wheelSpeed"> Controller of the wheel rotating speed. </param>
    /// <param name="elements"> The elements in the wheel. </param>
    /// <param name="wheelSize"> Specify the scale size of the wheel. </param>
    /// <param name="doubleChance"> If you want to double the instances number of the elements to get the selection chance doubled. </param>
    /// <param name="labelOrientation"> Specify the orientation of labels on wheel. </param>
    /// <param name="labelOffset"> The offset of the labels on wheel from the origin till the end of the wheel. </param>
    /// <param name="iconOffset"> The offset of the icons on wheel from the origin till the end of the wheel. </param>
    /// <param name="sliceThickness"> The outline thickness arround each slice in the wheel. </param>
    /// <param name="pointerPosition"> The postion of the wheel pointer. Note that the pointer doesn't change its rotation relative to its postion. </param>
    /// <param name="pointerSize"> The scale size of the wheel pointer. </param>
    public FortuneWheel (

        WheelElement[] elements, 
        float wheelSpeed=200,
        float wheelSize=3,
        bool doubleChance=false,
        float labelOrientation=0,
        float labelOffset=0.5f,
        float iconOffset=0.5f,
        float sliceThickness=0,
        PointerPosition pointerPosition=PointerPosition.Top,
        float pointerSize=1

        )
    {
        slices = new List<WheelElement>(elements);
        this.wheelSize = wheelSize;
        this.wheelSpeed = wheelSpeed;
        this.doubleChance = doubleChance;
        this.labelRotation = labelOrientation;
        this.labelOffset = labelOffset;
        this.iconOffset = iconOffset;
        this.sliceThickness = sliceThickness;
        this.pointerPositionIndex = ((int)pointerPosition);
        this.pointerScaleSize = pointerSize;

    }
}
