using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Entities.Characters;
using Assets.Scripts.Entities.Interfaces;
using Assets.Scripts.Model;
using UnityEngine;

public interface IObstacle
{
    EdgeType EdgeType { get; }
    IInteractable InteractableObject { get; }
    bool IsOpen { get; }
    void Open(BaseCharacter character);
}
