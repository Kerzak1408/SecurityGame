using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Constants;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Extensions;
using UnityEngine;

public static class InteractHelper
{
    public static bool AreCloseToInteract(GameObject interacting, GameObject interacted)
    {
        if (!interacted.HasScriptOfType(typeof(IInteractable))) return false;
        return AreCloseToInteract(interacting.transform.position, interacted.transform.position);
    }

    public static bool AreCloseToInteract(Vector3 vector1, Vector3 vector2)
    {
        var vector1In2D = new Vector3(vector1.x, vector1.z);
        var vector2In2D = new Vector3(vector2.x, vector2.z);
        return Vector3.Distance(vector1In2D, vector2In2D) <= Constants.INTERACTABLE_DISTANCE;
    }
}
