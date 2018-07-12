using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Element", menuName ="Create new Element", order = 0)]
public class Element : ScriptableObject
{
    public string elementName;
    public Color elementColor;
    public Texture symbowl;
    public List<ElementEffectiveness> EffectivenessOnElement;
    public Element()
    {
        EffectivenessOnElement = new List<ElementEffectiveness>();
        elementName = "none";
        elementColor = Color.white;
        symbowl = null;
    }

    public void CopyValues(Element newInfo)
    {
        elementName = newInfo.elementName;
        elementColor = newInfo.elementColor;
        symbowl = newInfo.symbowl;
    }

    public void SetValues(string eName, Texture texture, Color color)
    {
        elementName = eName;
        symbowl = texture;
        elementColor = color;
        EffectivenessOnElement = new List<ElementEffectiveness>();
    }

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
