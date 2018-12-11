using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IUnitFSM
{
    void ClickFSMHandler(Vector2 mousePos, PointerEventData eventData, bool reset = false);
    UnitFSM.UnitState GetUFSMState();
    Vector2 GetOrigin();
    Vector2 GetTerminus();
}
