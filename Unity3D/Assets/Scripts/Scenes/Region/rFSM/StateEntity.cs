using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;


public abstract class StateEntity: IInitializable, ITickable, IDisposable
{
    public virtual void MarkerClickHandler(Vector2 mousePos, PointerEventData eventData, bool lCtrlHold, bool lAltHold, bool reset =false)
    { }

    public virtual void OnSave() { }
    public virtual void OnSaveConfirmed() { }

    public virtual void OnCommit() { }
    public virtual void OnDiscard() { }

    public virtual void Initialize()
    {
    }

    public virtual void Start()
    {
    }
    public virtual void Tick()
    {
    }
    public virtual void Dispose()
    {
    }
}
