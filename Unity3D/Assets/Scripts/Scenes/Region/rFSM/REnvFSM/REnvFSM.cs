using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class REnvFSM : MonoBehaviour, IREnvFSM, IPointerClickHandler
{
    [Inject]
    StateFactory _stateFactoryRE;

    [Inject]
    IMarkersFSM _markersFSM;

    public RectTransform rectMain;

    // Current state name and entity
    StateEntity _curStateEntity = null;
    REnvState _curStateName;

    // keyCode for segment reference point
    bool lCtrlHold = false;
    bool lAltHold = false;

    void Update() {
        if (Input.GetKey(KeyCode.LeftControl))
            lCtrlHold = true;
        else if (!Input.GetKey(KeyCode.LeftControl))
            lCtrlHold = false;

        if (Input.GetKey(KeyCode.LeftAlt))
            lAltHold = true;
        else if (!Input.GetKey(KeyCode.LeftAlt))
            lAltHold = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector2 mousePos = new Vector2();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectMain, Input.mousePosition, Camera.main, out mousePos);

        Debug.Log("CLICK POS: " + mousePos);

        // origin and terminus must not be the same, disallowed in every state
        if (_markersFSM.GetCurMarkerState() == MarkersFSM.MarkerState.MARKER_START
            && _markersFSM.GetOrigin().x == mousePos.x && _markersFSM.GetOrigin().y == mousePos.y)
            return;

        _curStateEntity.MarkerClickHandler(mousePos, eventData, lCtrlHold, lAltHold);
    }

    public void ReqStateChange(REnvState nextState) {
        if (_curStateEntity != null) {
            _curStateEntity.Dispose();
            _curStateEntity = null;
        }
        _curStateName = nextState;
        _curStateEntity = _stateFactoryRE.CreateState(nextState);
        _curStateEntity.Start();
    }

    public REnvState GetCurrentStateName() {
        return _curStateName;
    }

    public StateEntity GetCurrentStateEntity()
    {
        return _curStateEntity;
    }
    
}
