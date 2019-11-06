using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DoorBase : Interactable
{
    public virtual bool Locked { get; set; }
    public virtual bool IsOpen { get; protected set; }
    public abstract void Open();
    public abstract void Close();
    [SerializeField] protected Transform FirstDoorStep;
    [SerializeField] protected Transform SecondDoorStep;
    public enum Direction {FirstToSecond, SecondToFirst };
    public abstract void PassThroughPassive(Transform objectToPass, Direction dir ,params Component[] componentsToHandle);
    public abstract void PassThroughActive(Transform objectToPass, Direction dir);
}
