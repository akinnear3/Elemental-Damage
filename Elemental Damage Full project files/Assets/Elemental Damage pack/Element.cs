using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// an element that can be assigned to a skill or equipment.
/// </summary>
[CreateAssetMenu(fileName ="New Element", menuName ="Create new Element", order = 0)]
public class Element : ScriptableObject
{
    /// <summary>
    /// the name of the element
    /// </summary>
    public string elementName;

    /// <summary>
    /// the color associated with the element
    /// </summary>
    public Color elementColor;

    /// <summary>
    /// the symbol or image that represents the element
    /// </summary>
    public Texture symbol;

    /// <summary>
    /// the elements effectiveness on other elements
    /// </summary>
    public List<ElementEffectiveness> EffectivenessOnElement;

    /// <summary>
    /// default Creation;
    /// Scriptable objects required a special declatation / instatiation
    /// </summary>
    public Element()
    {
        EffectivenessOnElement = new List<ElementEffectiveness>();
        elementName = "none";
        elementColor = Color.white;
        symbol = null;
    }

    /// <summary>
    /// a means of passing in all of the values at once.
    /// Also ensures that the Effectiveness On Element has been created
    /// </summary>
    /// <param name="eName">the element's name</param>
    /// <param name="texture">te element's symbol</param>
    /// <param name="color">the color of the element</param>
    public void SetValues(string eName, Texture texture, Color color)
    {
        elementName = eName;
        symbol = texture;
        elementColor = color;
        if (EffectivenessOnElement == null)
            EffectivenessOnElement = new List<ElementEffectiveness>();
    }

    /// <summary>
    /// find the index of itself from a list of elements.
    /// Returns -1 if it didn't find itself
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    public int searchForSelf(List<Element> e)
    {
        int self = -1;
        for(int i = 0; i < e.Count && self == -1; i++)
        {
            if(elementName == e[i].elementName)
            {
                self = i;
            }
        }
        return self;
    }
}
