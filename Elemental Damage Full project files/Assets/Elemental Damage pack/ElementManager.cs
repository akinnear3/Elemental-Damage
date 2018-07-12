using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

/// <summary>
/// manages the elements.
/// Meant to be a container to contain, load and save the elements.
/// </summary>
public class ElementManager
{
    #region fields
    /// <summary>
    /// the location to save new Elements
    /// </summary>
    public const string ElementFIlepath = "Assets/Elements/";

    /// <summary>
    /// the elements that have been loaded
    /// </summary>
    public List<Element> Elements;
    /// <summary>
    /// Indicates weather or not the elements have been loaded
    /// </summary>
    public bool HaveLoadedElements;
    #endregion

    #region properties
    /// <summary>
    /// the number of elements that are loaded
    /// </summary>
    public int ElementCount { get { return Elements.Count; } }
    #endregion

    /// <summary>
    /// default instatiation
    /// </summary>
    public ElementManager()
    {
        Elements = new List<Element>();
        HaveLoadedElements = false;
    }

    /// <summary>
    /// finds the element that is element[col], and returns it. It also outputs the multiplyer for using element[row] on element[col]
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <param name="multiplyer"></param>
    /// <returns></returns>
    public ElementEffectiveness FindElementEffectiveness(int row, int col, out float multiplyer)
    {
        //set up default returns
        multiplyer = 1;
        ElementEffectiveness e = new ElementEffectiveness(Elements[col], 1);

        //find the element
        bool foundElement = false;
        for (int i = Elements[row].EffectivenessOnElement.Count - 1; i >= 0 && !foundElement; i--)
        {
            //remove any effectiveness that have had the element deleted or removed
            while(Elements[row].EffectivenessOnElement[i].element == null && i < Elements[row].EffectivenessOnElement.Count && i >= 0)
            {
                if (Elements[row].EffectivenessOnElement[i].element == null)
                {
                    Elements[row].EffectivenessOnElement.RemoveAt(i);
                    i--;
                }
            }
            //compares the name of the element we're looking for with the next element in the row's effectiveness
            //if the element we're looking for is this one.
            if (i < Elements[row].EffectivenessOnElement.Count && i >= 0 && Elements[col].elementName == Elements[row].EffectivenessOnElement[i].element.elementName)
            {
                //inform that its found
                foundElement = true;

                //collect the values
                e = Elements[row].EffectivenessOnElement[i];
                multiplyer = e.Multiplyer;
            }
        }
        //if the element's relation doesn't exist; Create the default relation of 0
        if(!foundElement)
        {
            //creates an effectiveness
            Elements[row].EffectivenessOnElement.Add(e);

            //this lets the element save during the next asset save 
            EditorUtility.SetDirty(Elements[row]);
        }
        return e;
    }

    /// <summary>
    /// A command that can be delayed until its called next time.
    /// It creates the element as a scriptable object and adds it to the list
    /// </summary>
    /// <param name="ElementName">the new elements name</param>
    /// <param name="ElementTexture">the symbol for the element</param>
    /// <param name="ElementColor">the color for the element</param>
    /// <returns></returns>
    public IEnumerator CreateElement(string ElementName, Texture ElementTexture, Color ElementColor)
    {
        //The scriptable Object create instance
        Element newElementAsset = Element.CreateInstance<Element>();

        //pass in the values
        newElementAsset.SetValues(ElementName, ElementTexture, ElementColor);

        //add it to the list
        Elements.Add(newElementAsset);

        //create the asset
        AssetDatabase.CreateAsset(newElementAsset, ElementFIlepath + ElementName + ".asset");

        //because it needs to return something
        yield return false;
    }

    /// <summary>
    /// Updates the elements values
    /// </summary>
    /// <param name="index">the elements index</param>
    /// <param name="ElementName">the new name</param>
    /// <param name="ElementTexture">the new symbol</param>
    /// <param name="ElementColor">the new color</param>
    public void UpdateElement(int index, string ElementName, Texture ElementTexture, Color ElementColor)
    {
        //make the changes
        Elements[index].SetValues(ElementName, ElementTexture, ElementColor);

        //tell it its been changed
        EditorUtility.SetDirty(Elements[index]);

        //save any changes
        AssetDatabase.SaveAssets();
    }


    #region Saveing an loading

    /// <summary>
    /// causes all elements to be resaved
    /// </summary>
    public void Save()
    {
        //marks all element assets as 'changed'
        foreach(Element e in Elements)
        {
            EditorUtility.SetDirty(e);
        }

        //saves any assets that have been 'changed'
        AssetDatabase.SaveAssets();
    }
    /// <summary>
    /// Loads the list of elements from the resource folder
    /// </summary>
    public void Load()
    {
        //sets the indicator that elements have been loaded
        HaveLoadedElements = true;

        #region loading the data
        //create the list
        Elements = new List<Element>();

        //find any assets in the Elements folder that are of type element
        //collects their GUIDs as strings
        string[] foundGUIDs = AssetDatabase.FindAssets("t:Element", new string[] { ElementFIlepath.TrimEnd('/') });

        //goes through all of the found element GUIDs, finds their filepath, and loads them
        for (int i = 0; i < foundGUIDs.Length; i++)
        {
            Elements.Add(AssetDatabase.LoadAssetAtPath<Element>(AssetDatabase.GUIDToAssetPath(foundGUIDs[i])));
        }

        //a method to load all of the asset files in a folder
        //DirectoryInfo dir = new DirectoryInfo(ElementFIlepath);
        //FileInfo[] info = dir.GetFiles("*.asset");
        //foreach (FileInfo f in info)
        //{   
        //    Elements.Add(AssetDatabase.LoadAssetAtPath<Element>(ElementFIlepath + f.Name));
        //}
        #endregion

    }
    #endregion

    /// <summary>
    /// deletes an element.
    /// Both the reference in the list as well as the asset file
    /// </summary>
    /// <param name="elementIndex"></param>
    public void DeleteElement(int elementIndex)
    {
        //deletes the asset
        AssetDatabase.DeleteAsset(ElementFIlepath + Elements[elementIndex].elementName + ".asset");

        //removes it from the list
        Elements.RemoveAt(elementIndex);

        //cool; you can tell a for loop do do anything you want as long as it follows the 3 category rule
        //section 1; Variable declaration (i and x)
        //section 2; when do we stop this loop? (when i >= the number of elements)
        //section 3; what do we do every time we reach the bottom? (i++ and x = 0)
        for(int i = ElementCount - 1, x = Elements[i].EffectivenessOnElement.Count - 1; i >=0; i--, x = Elements[i].EffectivenessOnElement.Count - 1)
        {
            while(x >= 0)
            {
                //this gets rid of any element effectiveness that had its element deleted
                if(Elements[i].EffectivenessOnElement[x].element == null)
                {
                    Elements[i].EffectivenessOnElement.RemoveAt(x);
                }
                x--;
            }
        }
        //then save because it affects all of the elements
        Save();
    }

    /// <summary>
    /// get the innermost exception of an exception and return it
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    System.Exception GetInnerException(System.Exception ex)
    {
        while (ex.InnerException != null)
        {
            ex = ex.InnerException;
        }
        return ex;
    }

    /// <summary>
    /// it was going to do more... but basically it only needs to know that it might not have the latest list of elements
    /// </summary>
    public void StopManagement()
    {
        HaveLoadedElements = false;
    }
}

