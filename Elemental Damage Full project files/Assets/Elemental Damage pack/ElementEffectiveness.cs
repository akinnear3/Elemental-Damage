using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The effectiveness of an attack on an element.
/// Meant to be stored as a list in the attacking element to determine its effectiveness on other elements.
/// </summary>
[System.Serializable]
public struct ElementEffectiveness
{
    /// <summary>
    /// the defending element
    /// </summary>
    public Element element;

    /// <summary>
    /// the multiplyer
    /// </summary>
    public float Multiplyer;

    /// <summary>
    /// Creates a new Element Effectiveness
    /// </summary>
    /// <param name="e">the defending element</param>
    /// <param name="multiplyer">the multiplyer to use when attacking this element</param>
    public ElementEffectiveness(Element e, float multiplyer)
    {
        element = e;
        Multiplyer = multiplyer;
    }

    /// <summary>
    /// Creates and returns an element effectivness with the current element and new multipler.
    /// </summary>
    /// <param name="multiplyer"></param>
    /// <returns></returns>
    public ElementEffectiveness ChangeMultiplyer(float multiplyer)
    {
        return new ElementEffectiveness(element, multiplyer);
    }
}
