using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Transform portal1;
    [SerializeField] Transform portal2;
        
    public LightPoint TeleportLight(Transform portalEntered)
    {
        if(portalEntered == portal1)
        {
            return new LightPoint(portal2.position, portal2.up);
        }
        else
        {
            return new LightPoint(portal1.position, portal1.up);
        }
    }
}