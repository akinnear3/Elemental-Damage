using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// an editor window for modifying Element Effectiveness.
/// Meant to be called by the Element Window
/// </summary>
public class EditingEffectivenessWindow : EditorWindow
{
    /// <summary>
    /// the element manager that was loaded in the Element Window
    /// </summary>
    static ElementManager EManager;

    /// <summary>
    /// the index of the element that is having its element effectiveness modified. (the attacking element)
    /// </summary>
    static int Row;

    /// <summary>
    /// the index of the element effectiveness. (the defending element)
    /// </summary>
    static int Col;

    /// <summary>
    /// this is used to determine the index of the effectiveness (the defending element)
    /// </summary>
    static string TargetElement;

    /// <summary>
    /// this is the temporary storage of the multiplyer effectiveness. This way changes can be saved (applied to the actual effectiveness) or canceled (not applied)
    /// </summary>
    static float newMultiplyer;

    /// <summary>
    /// quick reference to get and set the multiplyer from the effectivness
    /// </summary>
    static float CurrentMultiplyer
    {
        get
        {
            return EManager.Elements[Row].EffectivenessOnElement[Col].Multiplyer;
        }
        set
        {
            EManager.Elements[Row].EffectivenessOnElement[Col] = EManager.Elements[Row].EffectivenessOnElement[Col].ChangeMultiplyer(value);
        }
    }

    /// <summary>
    /// allows this window to be called while providing the necessary information
    /// </summary>
    /// <param name="emanager">the Element Manager that is being used</param>
    /// <param name="row">the index of the element that we are modifying</param>
    /// <param name="targetElement">the element that can be found in the effectiveness of the element we are modifying</param>
    public static void ShowWindow(ElementManager emanager, int row, string targetElement)
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(EditingEffectivenessWindow));

        //pass over the information
        EManager = emanager;
        Row = row;
        TargetElement = targetElement;

        //find which column the element's infrmation is on
        bool foundElement = false;
        for(int i = 0; i < EManager.ElementCount && !foundElement; i++)
        {
            if(EManager.Elements[row].EffectivenessOnElement[i].element.elementName == TargetElement)
            {
                Col = i;
                foundElement = true;
            }
        }

        //have the windows 'new' multiplyer start as the current multiplyer
        newMultiplyer = CurrentMultiplyer;
    }

    //this is how the window will look
    void OnGUI()
    {
        //Section Title
        GUILayout.Label("Editing Element Effectiveness", EditorStyles.boldLabel);

        //set the backgrount / box color to that of the element
        GUI.backgroundColor = EManager.Elements[Row].elementColor;

        //draw the attacking element
        GUILayout.Box(EManager.Elements[Row].symbol);

        //reset color
        GUI.backgroundColor = Color.white;

        //text
        GUILayout.Label("Will deal");

        //the multiplyer
         newMultiplyer = EditorGUILayout.FloatField(newMultiplyer);

        //text
        GUILayout.Label("Times damage to");

        //modify the box's background color to match the defending element's color and then draw the element's symbowl
        GUI.backgroundColor = EManager.Elements[Row].EffectivenessOnElement[Col].element.elementColor;
        GUILayout.Box(EManager.Elements[Row].EffectivenessOnElement[Col].element.symbol);

        //reset the color
        GUI.backgroundColor = Color.white;

        //a button to apply the change made to the effectiveness and return to the Element window
        if(GUILayout.Button("Save changes"))
        {
            CurrentMultiplyer = newMultiplyer;
            EditorUtility.SetDirty(EManager.Elements[Row]);
            ElementWindow.ShowWindow(EManager);
            AssetDatabase.SaveAssets();
            this.Close();
        }

        //a button to return to the element window without applying the changes
        if (GUILayout.Button("Camcel"))
        {
            ElementWindow.ShowWindow();
            this.Close();
        }

    }
}
