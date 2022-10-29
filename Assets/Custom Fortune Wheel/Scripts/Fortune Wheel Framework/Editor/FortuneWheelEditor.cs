using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using TMPro;

[CustomEditor(typeof(FortuneWheel))]
public class FortuneWheelEditor : Editor
{
    private ReorderableList list;
    private float lineSpace;
    private float classSpace;
    bool showWheelProperties = true;
    bool showContentProperties = true;
    bool showSliceOutline = false;
    bool showPointerProperties = false;

    FortuneWheel fortuneWheel;
	
	private void OnEnable() {
        
        fortuneWheel = (FortuneWheel) target;
        list = new ReorderableList(serializedObject, 
                serializedObject.FindProperty("slices"), 
                true, true, true, true);

        lineSpace = EditorGUIUtility.singleLineHeight;
        classSpace = EditorGUIUtility.singleLineHeight * 9;
        list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) => 
        {

            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            //EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineSpace), "New Slice");


                                        /* SLICE PROPERTIES */
            EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + (lineSpace), rect.width, lineSpace),
                    element.FindPropertyRelative("fillColor"), new GUIContent("Fill Color"));
            EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + (lineSpace * 2), rect.width, lineSpace),
                    element.FindPropertyRelative("icon"), new GUIContent("Icon"));

                                        /* TEXT PROPERTIES */

            EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + (lineSpace * 4), rect.width, lineSpace),
                    element.FindPropertyRelative("label"), new GUIContent("Label"));

            EditorGUILayout.Separator();

            EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + (lineSpace * 5), rect.width, lineSpace),
                    element.FindPropertyRelative("showLabelInWheel"), new GUIContent("Visible"));


            EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + (lineSpace * 6), rect.width, lineSpace),
                    element.FindPropertyRelative("labelColor"), new GUIContent("Label Color"));
            EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y + (lineSpace * 7), rect.width, lineSpace),
                    element.FindPropertyRelative("fontSize"), new GUIContent("Font Size"));


            string currLabelVal = element.FindPropertyRelative("label").stringValue;
            if(currLabelVal != "")
            {
				EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineSpace), currLabelVal);
            }
        };

        list.elementHeightCallback = (int index) => 
        {
			return classSpace;
        };

        list.drawHeaderCallback = (Rect rect) => 
        {
			EditorGUI.LabelField(rect, "Wheel Slices");
        };     
        list.onCanRemoveCallback = list =>
        {
			return list.count > 2;
        };

        }
	
	public override void OnInspectorGUI() 
	{
		
		showWheelProperties = EditorGUILayout.Foldout(showWheelProperties, "Wheel Properties");
		if(showWheelProperties)
		{
			EditorGUI.indentLevel += 1;
			fortuneWheel.btn = EditorGUILayout.ObjectField( fortuneWheel.btn, typeof(GameObject),true);
			fortuneWheel.pnt = EditorGUILayout.ObjectField(fortuneWheel.pnt, typeof(GameObject), true);
			fortuneWheel.wheelSize = EditorGUILayout.Slider("Wheel Size", fortuneWheel.wheelSize, 1, 20);
			fortuneWheel.wheelSpeed = EditorGUILayout.Slider("Wheel Speed", fortuneWheel.wheelSpeed, 100, 1000);
			fortuneWheel.doubleChance = EditorGUILayout.Toggle("Double Chance", fortuneWheel.doubleChance);
			fortuneWheel.createOnStart = EditorGUILayout.Toggle("Create on Start", fortuneWheel.createOnStart);
			if(fortuneWheel.createOnStart == false) EditorGUILayout.HelpBox("Warning: You disabled the auto creation of the wheel. You can create it manually using the \"CreateFortune()\" funtion.", MessageType.Warning);
			fortuneWheel.font = (TMP_FontAsset) EditorGUILayout.ObjectField("Font", fortuneWheel.font, typeof(TMP_FontAsset), true);
			GuiLine();
		}
		EditorGUI.indentLevel = 0;


		showContentProperties = EditorGUILayout.Foldout(showContentProperties, "Content Properties");
		if(showContentProperties)
		{
			EditorGUI.indentLevel += 1;
			fortuneWheel.labelRotation = EditorGUILayout.Slider("Labels Orientation", fortuneWheel.labelRotation, 0, 360);
			fortuneWheel.labelOffset = EditorGUILayout.Slider("Label Offset", fortuneWheel.labelOffset, .1f, 1f);
			fortuneWheel.iconOffset = EditorGUILayout.Slider("Icon Offset", fortuneWheel.iconOffset, 0f, 1f);
			GuiLine();
		}
		EditorGUI.indentLevel = 0;


		showSliceOutline = EditorGUILayout.Foldout(showSliceOutline, "Slice Outline");
		if(showSliceOutline)
		{
			EditorGUI.indentLevel += 1;
			fortuneWheel.sliceThickness = EditorGUILayout.Slider("Slice Tickness", fortuneWheel.sliceThickness, 0, 5);
			fortuneWheel.sliceOutlineColor = EditorGUILayout.ColorField("Ouline Color", fortuneWheel.sliceOutlineColor);
			GuiLine();
		}        
		EditorGUI.indentLevel = 0;
		

		showPointerProperties = EditorGUILayout.Foldout(showPointerProperties, "Pointer Properties");
		EditorGUILayout.HelpBox(@"Note that the pointer sprite is not changing its rotation relative to the position. 
		(If the pointer sprite is down, it won't rotate)", MessageType.Info);

		if(showPointerProperties)
		{
			EditorGUI.indentLevel += 1;
			EditorGUILayout.LabelField("Pointer Position");
			fortuneWheel.pointerPositionIndex =  EditorGUILayout.Popup(fortuneWheel.pointerPositionIndex, new string[] {"Top", "Bottom", "Right", "Left", "Center"}, GUILayout.Width(150));
			fortuneWheel.pointerScaleSize = EditorGUILayout.Slider("Scale Size", fortuneWheel.pointerScaleSize, 0, 3);
			fortuneWheel.pointerIcon = (Sprite) EditorGUILayout.ObjectField("Icon", fortuneWheel.pointerIcon, typeof(Sprite), true);
		}
		EditorGUI.indentLevel = 0;
		Undo.RecordObject(fortuneWheel, "Fortune Wheel Edit");

		serializedObject.Update();
		list.DoLayoutList();
		serializedObject.ApplyModifiedProperties();
	}
    void GuiLine( int i_height = 1 )
	{
		Rect rect = EditorGUILayout.GetControlRect(false, i_height );
		rect.height = i_height;
		EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
	}

}
