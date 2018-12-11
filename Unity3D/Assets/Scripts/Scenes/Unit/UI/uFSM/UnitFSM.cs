using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class UnitFSM : IUnitFSM
{
    public enum UnitState { UNIT_INIT, UNIT_START, UNIT_FINAL }
    UnitState _ustate;

    Vector2 _vorigin = Vector2.zero;
    Vector2 _vterminus = Vector2.zero;

    [Inject]
    SignalBus _signalBus;

    public UnitFSM()
    {
        _ustate = UnitState.UNIT_INIT;
    }

    public virtual void ClickFSMHandler(Vector2 mousePos, PointerEventData eventData, bool reset = false) {
        if (reset) {
            ResetFSM();
            return;
        }

        switch (_ustate) {
            case UnitState.UNIT_INIT:// init: LClick => First click state
                if (eventData.button.Equals(PointerEventData.InputButton.Left))
                    EnterStartState(mousePos);// XA
                break;
            case UnitState.UNIT_START:// XA: LClick => Last, RClick => init
                if (eventData.button.Equals(PointerEventData.InputButton.Left)) {
                    //do not accept exactly same x_y for second left click
                    if (mousePos.x == _vorigin.x && mousePos.y == _vorigin.y) return;
                    EnterFinalState(mousePos);// -> XB
                }
                if (eventData.button.Equals(PointerEventData.InputButton.Right))
                    ReturnInitState();//  Init <-
                break;
            case UnitState.UNIT_FINAL:// XB: RClick => XA
                if (eventData.button.Equals(PointerEventData.InputButton.Right))
                    ReturnStartState();// XA <-
                break;
        }
    }

    void EnterStartState(Vector2 vp1) {
        _ustate = UnitState.UNIT_START;
        _vorigin = vp1;
        UnitOriginPointEvt evt = new UnitOriginPointEvt();
        evt.vorigin = vp1;
        _signalBus.Fire(evt);
    }

    void EnterFinalState(Vector2 vp2) {
        _ustate = UnitState.UNIT_FINAL;// = XB and LineImg
        _vterminus = vp2;
        UnitTerminusPointEvt evt = new UnitTerminusPointEvt();
        evt.vterminus = vp2;
        _signalBus.Fire(evt);

        UpdateLineImg(_vterminus, _vorigin);
    }

    void ReturnInitState() {
        _ustate = UnitState.UNIT_INIT;// = Init
        _vorigin = Vector2.zero;
        UnitOriginPointEvt evt = new UnitOriginPointEvt();
        _signalBus.Fire(evt);

    }

    void ReturnStartState() {
        _ustate = UnitState.UNIT_START;// = XA
        _vterminus = Vector2.zero;
        UnitTerminusPointEvt evt1 = new UnitTerminusPointEvt();
        _signalBus.Fire(evt1);

        UnitLineImgUpdateEvt evt2 = new UnitLineImgUpdateEvt();
        _signalBus.Fire(evt2);
    }

    void ResetFSM() {
        ReturnStartState();
        ReturnInitState();
    }

    void UpdateLineImg(Vector2 vp2, Vector2 vp1) {
        Vector2 dir = (vp2 - vp1).normalized;
        float vecLineDist = Vector2.Distance(vp1, vp2);
        Vector2 vpos = vp1 + ((dir * vecLineDist) / 2);// >> 1
        float sign = (vp1.y < vp2.y) ? 1.0f : -1.0f;
        float angle = Vector2.Angle(Vector2.right, dir) * sign;
        UnitLineImgUpdateEvt evt2 = new UnitLineImgUpdateEvt();
        evt2.anchoredPosition = vpos;
        evt2.angle = angle;
        evt2.magnitude = vecLineDist;
        evt2.sizeDelta = new Vector2(vecLineDist, 5f);
        _signalBus.Fire(evt2);
    }

    public UnitState GetUFSMState() {
        return _ustate;
    }

    public Vector2 GetOrigin() {
        return _vorigin;
    }

    public Vector2 GetTerminus() {
        return _vterminus;
    }
}