using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class ElementWindow : EditorWindow
{
    /// <summary>
    /// a string reserved for relaing any errors to the user
    /// </summary>
    string Errmsg;

    /// <summary>
    /// a reference to the container for the elements. this saves and loads the elements
    /// </summary>
    static ElementManager EManager;

    /// <summary>
    /// this is the name that will be added to an element when it is created. 
    /// It is modified by the user as a text input field
    /// </summary>
    string newElementName = "";

    /// <summary>
    /// this is the image/texture the element will have associated with it. 
    /// it is set by the user in an object texture field
    /// </summary>
    Texture newElementTexture = null;

    /// <summary>
    /// this is the color the new element will receive. 
    /// It is set by the user with an input color field
    /// </summary>
    Color newElementColor = Color.white;

    /// <summary>
    /// this is a bool to control the appearence and action of the 'create' button (create new element).
    /// </summary>
    bool ShowCreateButton;

    /// <summary>
    /// this is the values that are used by the element's scroll view to allow scrolling.
    /// Value is modified as the user scrolles in the scroll view.
    /// </summary>
    Vector2 elementsViewPosition = Vector2.zero;

    /// <summary>
    /// This is used to identify witch element is being edited / updated.
    /// this is the index of the element that is being edited.
    /// </summary>
    int EditingElementIndex = -1;


    /// <summary>
    /// display the ElementWindow
    /// </summary>
    [MenuItem("My Windows/Element Manager")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one    
        EditorWindow.GetWindow(typeof(ElementWindow));
    }

    public static void ShowWindow(ElementManager emanager)
    {
        EManager = emanager;
        ShowWindow();
    }

    /// <summary>
    /// what happens when its drawing the Window
    /// </summary>
    void OnGUI()
    {
        //the Bolded title
        GUILayout.Label("Element Window", EditorStyles.boldLabel);

        //dsiplays error messages
        GUILayout.Label("Errs: " + Errmsg);

        //moves everything down 50 pixles
        GUILayout.Space(50);

        //cause the Element Manager to load if it doesn't exist
        if (EManager == null)
        {
            EManager = new ElementManager();
            Errmsg = EManager.Load();
        }
        //shows the elements if its loaded
        if (EManager != null && EManager.HaveLoadedElements)
        {
            #region Allow Elements to be added
            
            //a title / label for this section
            GUILayout.Label("Element Modification");

            //add some space for viibility
            GUILayout.Space(5);

            //get the items to be a row
            GUILayout.BeginHorizontal();

            //Take in the user's input for the name (also displays the text input field)
            newElementName = GUILayout.TextField(newElementName, 200, GUILayout.Width(100));

            //Take in the user's input for the texture (also displays the texture input field)
            newElementTexture = (Texture) EditorGUI.ObjectField(new Rect(new Vector2(70, 115), new Vector2(100, 50)), "", newElementTexture, typeof(Texture), false);

            //Take in the user's input for the color (also displays the color input field)
            newElementColor = EditorGUI.ColorField(new Rect(new Vector2(5, 145), new Vector2(110, 20)), newElementColor);

            //determine if the user has entered enough information to be able to properly create an element. (Color is not nullable so it doesn't need to be checked)
            bool contentReady = !string.IsNullOrEmpty(newElementName) && newElementTexture != null;
            ShowCreateButton = contentReady  && EditingElementIndex == -1;

            //some space
            GUILayout.Space(80);

            //change the color so that the create button is grayed out when it shouldn't be used (and won't function)
            GUI.backgroundColor = ShowCreateButton? Color.white : Color.gray;
           
            //display the button, and if its pressed create the element (by executing the if statement)
            if (GUILayout.Button("Create", GUILayout.Width(100)) && ShowCreateButton)
            {
                EManager.CreateElement(newElementName, newElementTexture, newElementColor);
            }

            GUI.backgroundColor = EditingElementIndex == -1? Color.gray : contentReady ? Color.white : Color.red;
            if(GUILayout.Button("Update", GUILayout.Width(100)) && EditingElementIndex > -1 && contentReady)
            {
                EManager.UpdateElement(EditingElementIndex, newElementName, newElementTexture, newElementColor);

                newElementColor = Color.white;
                newElementName = "";
                newElementTexture = null;
                EditingElementIndex = -1;
            }

            GUI.backgroundColor = EditingElementIndex == -1 ? Color.gray :Color.white;
            if (GUILayout.Button("Cancel Update", GUILayout.Width(100)) && EditingElementIndex > -1)
            {
                newElementColor = Color.white;
                newElementName = "";
                newElementTexture = null;
                EditingElementIndex = -1;
            }

            if(GUILayout.Button("Delete Element", GUILayout.Width(100)) && EditingElementIndex > -1)
            {
                EManager.DeleteElement(EditingElementIndex);
                EditingElementIndex = -1;
            }

            //change the color back
            GUI.backgroundColor = Color.white;

            //end of the row
            GUILayout.EndHorizontal();
            #endregion

            #region display the current elements
            //some space
            GUILayout.Space(50);

            GUILayout.BeginHorizontal();
            //this groups label / title
            GUILayout.Label("Element Effectiveness");
            if(GUILayout.Button("Reload Elements"))
            {
                EManager.Load();
            }
            if(GUILayout.Button("Save Changes"))
            {
                EManager.Save();
            }
            GUILayout.EndHorizontal();

            //more space
            GUILayout.Space(5);

            //if there are elemnts to display, display them
            if(EManager.ElementCount > 0)
            {
                #region draw the elements for the column

                //this allows the elements created to be scrolled through; that way a smaller window can still show all of the elements
                elementsViewPosition = GUILayout.BeginScrollView(elementsViewPosition);

                //gets the elements to go in a row
                GUILayout.BeginHorizontal();

                //the top left spot; it needed to be something manual so that the rest of the elements would line up with each other
                GUILayout.Button("elements", GUILayout.Height(80), GUILayout.Width(250));

                //the top row of elements
                foreach(Element e in EManager.Elements)
                {
                    //change the color to match the element's color
                    GUI.backgroundColor = e.elementColor;

                    //if they press the elements I want to give them the option of modifying the element
                    if(GUILayout.Button(new GUIContent(e.elementName, e.symbowl), GUILayout.Height(80), GUILayout.Width(250)))
                    {
                        StartEdit(e);
                    }
                }

                //end the row
                GUILayout.EndHorizontal();
                #endregion

                //display one element and its relationship with the other elements
                for (int row = 0; row < EManager.ElementCount; row++)
                {
                    //set it up nicely by having one row per element
                    GUILayout.BeginHorizontal();

                    //change its color to match the elements
                    GUI.backgroundColor = EManager.Elements[row].elementColor;

                    //name the element used & let the user change the element's information if they click it
                    if(GUILayout.Button(new GUIContent(EManager.Elements[row].elementName, EManager.Elements[row].symbowl), GUILayout.Height(80), GUILayout.Width(250)))
                    {
                        StartEdit(EManager.Elements[row]);
                    }

                    //its relation with the other elements
                    for (int col = 0; col < EManager.ElementCount; col++)
                    {
                        //the multiplyer to use when attacking this element
                        float multiplyer = 1;

                        //get the relation's information (creates if it doesn't exist)
                        EManager.FindElementEffectiveness(row, col, out multiplyer);

                        //set the color to that of the attacking element
                        GUI.backgroundColor = EManager.Elements[row].elementColor;

                        //display's the information and allows the relationship to be modified
                        if(GUILayout.Button(new GUIContent("Takes " + multiplyer.ToString() + "x dmg", EManager.Elements[col].symbowl), GUILayout.Height(80), GUILayout.Width(250)))
                        {
                            EditingEffectivenessWindow.ShowWindow(EManager, row, EManager.Elements[col].elementName);
                        }
                    }
                    //end the row
                    GUILayout.EndHorizontal();
                }
                //end the area that will scroll
                GUILayout.EndScrollView();
            }
           
            #endregion
        }
        //loads the elements because they havn't loaded yet or need to be reloaded
        else
        {
            Errmsg = EManager.Load();
        }
    }

    private void StartEdit(Element e)
    {
        newElementColor = e.elementColor;
        newElementName = e.elementName;
        newElementTexture = e.symbowl;
        EditingElementIndex = e.searchForSelf(EManager.Elements);
    }

    #region what to do when the window is closed, minimized or not used
    private void OnDestroy()
    {
        EManager.StopManagement();
    }
    private void OnDisable()
    {
        EManager.StopManagement();
    }
    #endregion
}
