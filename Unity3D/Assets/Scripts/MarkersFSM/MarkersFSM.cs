using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;


public class MarkersFSM : MonoBehaviour, IMarkersFSM
{
    [Inject]
    IREnvFSM _rEnvFSM;// state=create ? s-final=>RMI : s-final=>LI

    public Text txt_xa, txt_xb;
    public Image LineImg, RegionMarkerImg;

    public enum MarkerState { MARKER_INIT, MARKER_START, MARKER_FINAL }
    MarkerState _mstate;

    public virtual void MarkerClickHandler(Vector2 mousePos, PointerEventData eventData, bool reset=false)
    {
        switch (_mstate)
        {
            case MarkerState.MARKER_INIT:// init: LClick => First click state
                if (eventData.button.Equals(PointerEventData.InputButton.Left))
                    EnterStartState(mousePos);// XA
                break;
            case MarkerState.MARKER_START:// XA: LClick => Last, RClick => init
                if (eventData.button.Equals(PointerEventData.InputButton.Left))
                    EnterFinalState(mousePos);// -> XB
                if (eventData.button.Equals(PointerEventData.InputButton.Right))
                    ReturnInitState();//  Init <-
                break;
            case MarkerState.MARKER_FINAL:// XB: RClick => XA
                if (eventData.button.Equals(PointerEventData.InputButton.Right))
                    ReturnStartState();// XA <-
                break;
        }
    }

    void EnterStartState(Vector2 vp1)
    {
        if (vp1.Equals(Vector2.zero))
        {
            _mstate = MarkerState.MARKER_INIT;
            txt_xa.gameObject.SetActive(false);
        }
        else
        {
            _mstate = MarkerState.MARKER_START;
            txt_xa.gameObject.GetComponent<RectTransform>().anchoredPosition = vp1;
            txt_xa.gameObject.SetActive(true);
        }
    }

    void ReturnInitState()
    {
        EnterStartState(Vector2.zero);
    }

    void EnterFinalState(Vector2 vp2)
    {
        if (vp2.Equals(Vector2.zero))
        {
            _mstate = MarkerState.MARKER_START;
            txt_xb.gameObject.SetActive(false);
            LineImg.gameObject.SetActive(false);
            RegionMarkerImg.gameObject.SetActive(false);
        }
        else
        {
            _mstate = MarkerState.MARKER_FINAL;
            txt_xb.gameObject.GetComponent<RectTransform>().anchoredPosition = vp2;
            txt_xb.gameObject.SetActive(true);
            if (_rEnvFSM.GetCurrentStateName() == REnvState.SegmentCUD)
            {
                UpdateLineImg(vp2, GetOrigin());
            }
            else if (_rEnvFSM.GetCurrentStateName() == REnvState.CreateRegion)
            {
                UpdateRegionMarkerImg(vp2, GetOrigin());
            }
        }
    }

    void ReturnStartState()
    {
        EnterFinalState(Vector2.zero);
    }

    void UpdateLineImg(Vector2 vp2, Vector2 vp1)
    {
        Vector2 dir = (vp2 - vp1).normalized;
        // magnitude of segment:
        float vecLineDist = Vector2.Distance(vp1, vp2);
        Vector2 vpos = vp1 + ((dir * vecLineDist) / 2);// >> 1
        float sign = (vp1.y < vp2.y) ? 1.0f : -1.0f;
        float angle = Vector2.Angle(Vector2.right, dir) * sign;

        RectTransform rt = LineImg.gameObject.GetComponent<RectTransform>();
        LineImg.gameObject.transform.rotation = Quaternion.identity;

        rt.anchoredPosition = vpos;
        // magnitude of segment: rt.sizeDelta.x=vecLineDist
        rt.sizeDelta = new Vector2(vecLineDist, 2f);
        rt.Rotate(new Vector3(0, 0, angle));

        LineImg.gameObject.SetActive(true);
    }

    void UpdateRegionMarkerImg(Vector2 vp2, Vector2 vp1)
    {
        Vector2 dir = (vp2 - vp1).normalized;
        float vecLineDist = Vector2.Distance(vp1, vp2);
        Vector2 vpos = vp1 + ((dir * vecLineDist) / 2);

        float width = vp2.x - vp1.x;
        float height = vp2.y - vp1.y;
        // needed for the Unity's Outline cmp
        Vector2 effectDistance = new Vector2(width / 2, height / 2);

        RegionMarkerImg.gameObject.transform.rotation = Quaternion.identity;
        //RegionMarkerImg.transform.SetParent(scrollViewImgContent.transform);
        RegionMarkerImg.transform.localScale = Vector3.one;
        RegionMarkerImg.GetComponent<RectTransform>().anchoredPosition = vpos;
        RegionMarkerImg.GetComponent<Outline>().effectDistance = effectDistance;
        RegionMarkerImg.gameObject.SetActive(true);
    }

    void AllMarkersOff()
    {
        txt_xa.gameObject.SetActive(false);
        txt_xb.gameObject.SetActive(false);
        LineImg.gameObject.SetActive(false);
        RegionMarkerImg.gameObject.SetActive(false);
    }

    public void FullFSMReset()
    {
        AllMarkersOff();
        _mstate = MarkerState.MARKER_INIT;
    }

    public Vector2 GetOrigin() {
        if (txt_xa.gameObject.activeSelf)
            return txt_xa.GetComponent<RectTransform>().anchoredPosition;
        else
            return Vector2.zero;
    }

    public Vector2 GetTerminus() {
        if (txt_xb.gameObject.activeSelf)
            return txt_xb.GetComponent<RectTransform>().anchoredPosition;
        else
            return Vector2.zero;
    }

    public MarkersFSM.MarkerState GetCurMarkerState() {
        return _mstate;
    }
    
    public Vector2 GetRegionMarkerCenter() {
        if (!RegionMarkerImg.gameObject.activeSelf)
            return Vector2.zero;
        return RegionMarkerImg.GetComponent<RectTransform>().anchoredPosition;
    }

    public Vector2 GetRegionMarkerEffDist() {
        if (!RegionMarkerImg.gameObject.activeSelf)
            return Vector2.zero;
        return RegionMarkerImg.GetComponent<Outline>().effectDistance;
    }
}
