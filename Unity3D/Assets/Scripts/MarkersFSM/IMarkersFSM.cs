using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public interface IMarkersFSM {

    void FullFSMReset();
    void MarkerClickHandler(Vector2 mousePos, PointerEventData eventData, bool reset = false);
    Vector2 GetOrigin();
    Vector2 GetTerminus();
    MarkersFSM.MarkerState GetCurMarkerState();
    Vector2 GetRegionMarkerEffDist();
    Vector2 GetRegionMarkerCenter();
}
