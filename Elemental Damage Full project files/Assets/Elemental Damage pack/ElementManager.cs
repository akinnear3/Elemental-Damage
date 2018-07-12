using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ElementManager
{
    #region fields
    /// <summary>
    /// the location to save new Elements
    /// </summary>
    public const string ElementFIlepath = "Assets/Resources/Elements/";

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
        for (int i = 0; i < Elements[row].EffectivenessOnElement.Count && !foundElement; i++)
        {
            //compares the name of the element we're looking for with the next element in the row's effectiveness
            //if the element we're looking for is this one.
            while(Elements[row].EffectivenessOnElement[i].element == null && i < Elements[row].EffectivenessOnElement.Count)
            {
                if (Elements[row].EffectivenessOnElement[i].element == null)
                {
                    Elements[row].EffectivenessOnElement.RemoveAt(i);
                }
            }
            if (i < Elements[row].EffectivenessOnElement.Count && Elements[col].elementName == Elements[row].EffectivenessOnElement[i].element.elementName)
            {
                foundElement = true;
                e = Elements[row].EffectivenessOnElement[i];
                multiplyer = e.Multiplyer;
            }
        }
        //if the element's relation doesn't exist; Create the default relation of 0
        if(!foundElement)
        {
            Elements[row].EffectivenessOnElement.Add(e);
            EditorUtility.SetDirty(Elements[row]);
        }
        return e;
    }
    public void CreateElement(string ElementName, Texture ElementTexture, Color ElementColor)
    {
        Element newElementAsset = Element.CreateInstance<Element>();
        newElementAsset.SetValues(ElementName, ElementTexture, ElementColor);
        Elements.Add(newElementAsset);
        AssetDatabase.CreateAsset(newElementAsset, ElementFIlepath + ElementName + ".asset");
    }

    public void UpdateElement(int index, string ElementName, Texture ElementTexture, Color ElementColor)
    {
        Elements[index].SetValues(ElementName, ElementTexture, ElementColor);
        EditorUtility.SetDirty(Elements[index]);
        AssetDatabase.SaveAssets();
    }


    #region Saveing an loading

    /// <summary>
    /// causes all elements to be resaved
    /// </summary>
    public void Save()
    {
        foreach(Element e in Elements)
        {
            EditorUtility.SetDirty(e);
        }
        AssetDatabase.SaveAssets();
    }
    /// <summary>
    /// Loads the list of elements from the resource folder
    /// </summary>
    /// <returns></returns>
    public string Load()
    {
        string errmsg = "";
        try
        {
            
            HaveLoadedElements = true;
            #region loading the data
            Elements = new List<Element>();
            var foundElements = Resources.FindObjectsOfTypeAll<Element>();
            foreach(Element e in foundElements)
            {
                Elements.Add(e);
            }
            #endregion
        }
        catch (System.Exception ex)
        {
            errmsg = GetInnerException(ex).Message;
        }
        //send back an error mesage if needed
        return errmsg;
    }
    #endregion

    public void DeleteElement(int elementIndex)
    {
        AssetDatabase.DeleteAsset(ElementFIlepath + Elements[elementIndex].elementName + ".asset");
        Elements.RemoveAt(elementIndex);

        //cool; you can tell a for loop do do anything you want as long as it follows the 3 category rule
        //section 1; Variable declaration (i and x)
        //section 2; when do we stop this loop? (when i >= the number of elements)
        //section 3; what do we do every time we reach the bottom? (i++ and x = 0)
        for(int i = 0, x = 0; i < ElementCount; i++, x = 0)
        {
            while(x < ElementCount)
            {
                //this gets rid of any element effectiveness that had its element deleted
                if(Elements[i].EffectivenessOnElement[x].element == null)
                {
                    Elements[i].EffectivenessOnElement.RemoveAt(x);
                }
                x++;
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

    public void StopManagement()
    {
        HaveLoadedElements = false;
    }
}

