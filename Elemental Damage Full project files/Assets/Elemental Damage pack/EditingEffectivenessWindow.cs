using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditingEffectivenessWindow : EditorWindow
{
    static ElementManager EManager;
    static int Row;
    static int Col;
    static string TargetElement;
    static float newMultiplyer;

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

    public static void ShowWindow(ElementManager emanager, int row, string targetElement)
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(EditingEffectivenessWindow));
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

        newMultiplyer = CurrentMultiplyer;
    }

    void OnGUI()
    {
        GUILayout.Label("Editing Element Effectiveness", EditorStyles.boldLabel);

        GUI.backgroundColor = EManager.Elements[Row].elementColor;
        GUILayout.Box(EManager.Elements[Row].symbowl);
        GUI.backgroundColor = Color.white;

        GUILayout.Label("Will deal");

         newMultiplyer = EditorGUILayout.FloatField(newMultiplyer);

        GUILayout.Label("Times damage to");

        GUI.backgroundColor = EManager.Elements[Row].EffectivenessOnElement[Col].element.elementColor;
        GUILayout.Box(EManager.Elements[Row].EffectivenessOnElement[Col].element.symbowl);
        GUI.backgroundColor = Color.white;

        if(GUILayout.Button("Save changes"))
        {
            CurrentMultiplyer = newMultiplyer;
            EditorUtility.SetDirty(EManager.Elements[Row]);
            ElementWindow.ShowWindow(EManager);
            AssetDatabase.SaveAssets();
            this.Close();
        }
        if (GUILayout.Button("Camcel"))
        {
            ElementWindow.ShowWindow();
            this.Close();
        }

    }
}
