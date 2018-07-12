using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ElementEffectiveness
{
    public Element element;
    public float Multiplyer;

    public ElementEffectiveness(Element e, float multiplyer)
    {
        element = e;
        Multiplyer = multiplyer;
    }

    public ElementEffectiveness ChangeMultiplyer(float multiplyer)
    {
        return new ElementEffectiveness(element, multiplyer);
    }
}
