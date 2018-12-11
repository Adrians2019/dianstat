using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RRenderEngine : MonoBehaviour, RRenderUI
{
    public Transform parent;
    public Text txt_xa;
    public Image RegionMarkerImg;
    Color rColor = new Color(0f, 0f, 0.75f);
    Vector2 vSeizeLBL = new Vector2(200f, 40f);

    enum LABELTYPE { ANGLE_FIRST, ANGLE_SECOND, RNAME };

    RRenderDS _rUI = new RRenderDS();

    public void RenderRLabels(Region[] regions)
    {
        for (int i = 0; i < regions.Length; i++)
        {
            if (!_rUI.rUIDict.ContainsKey(regions[i]._rName))
                _rUI.rUIDict[regions[i]._rName] = new RegionUI();
            RenderRLabelsOf(regions[i], true);
        }
    }

    // source of the region may be either rDict, if need to render original, 
    // or rDraft, to render the user-updated region
    public void RenderRLabelsOf(Region r, bool toSet) {
        RegionUI rui = _rUI.rUIDict[r._rName];
        if (!toSet) {
            DeActivateRUI(rui);
            return;
        }
        RenderRLabelsON(r, rui);
    }

    void RenderRLabelsON(Region r, RegionUI rui) {
        DoInstantiation(rui);

        rui.E1.GetComponent<RectTransform>().anchoredPosition = CalcLablePos(r, LABELTYPE.ANGLE_FIRST);
        rui.E1.text = "E1: " + r._e1.ToString() + " deg.";
        rui.E1.gameObject.SetActive(true);

        rui.E2.GetComponent<RectTransform>().anchoredPosition = CalcLablePos(r, LABELTYPE.ANGLE_SECOND);
        rui.E2.text = "E2: " + r._e2.ToString() + " deg.";
        rui.E2.gameObject.SetActive(true);

        rui.rNamelabel.GetComponent<RectTransform>().anchoredPosition = CalcLablePos(r, LABELTYPE.RNAME);
        rui.rNamelabel.text = "ID: " + r._rName;
        rui.rNamelabel.gameObject.SetActive(true);

        rui.RegionMarkerImg.GetComponent<RectTransform>().anchoredPosition = r._center;
        rui.RegionMarkerImg.GetComponent<Outline>().effectDistance = r._effectDistance;
        rui.RegionMarkerImg.gameObject.SetActive(true);
    }

    Vector2 CalcLablePos(Region r, LABELTYPE lblPosType) {
        Vector2 rCorner = new Vector2(r._p2.x, r._p1.y);//top-right
        Vector2 dir = (rCorner - r._p1).normalized;
        float vecLineDist = Vector2.Distance(rCorner, r._p1);
        Vector2 vpos = r._p1 + ((dir * vecLineDist) / 2);// >> 1

        switch (lblPosType) {
            case LABELTYPE.ANGLE_FIRST:
                return new Vector2(vpos.x, vpos.y - 15);
            case LABELTYPE.ANGLE_SECOND:
                return new Vector2(vpos.x, vpos.y - 35);
            case LABELTYPE.RNAME:
                return new Vector2(r._p1.x + 20, r._p1.y - 15);
        }
        return Vector2.zero;
    }

    void DoInstantiation(RegionUI rui) {
        if (rui.E1 == null) {
            rui.E1 = Instantiate(txt_xa, Vector3.zero, Quaternion.identity) as Text;
            rui.E1.transform.SetParent(parent);
            rui.E1.GetComponent<RectTransform>().localScale = Vector3.one;
            rui.E1.GetComponent<RectTransform>().sizeDelta = vSeizeLBL;
        }
        if (rui.E2 == null) {
            rui.E2 = Instantiate(txt_xa, Vector3.zero, Quaternion.identity) as Text;
            rui.E2.transform.SetParent(parent);
            rui.E2.GetComponent<RectTransform>().localScale = Vector3.one;
            rui.E2.GetComponent<RectTransform>().sizeDelta = vSeizeLBL;
        }
        if (rui.rNamelabel == null) {
            rui.rNamelabel = Instantiate(txt_xa, Vector3.zero, Quaternion.identity) as Text;
            rui.rNamelabel.transform.SetParent(parent);
            rui.rNamelabel.GetComponent<RectTransform>().localScale = Vector3.one;
            rui.rNamelabel.GetComponent<RectTransform>().sizeDelta = vSeizeLBL;
        }
        if (rui.RegionMarkerImg == null) {
            rui.RegionMarkerImg = Instantiate(RegionMarkerImg, Vector3.zero, Quaternion.identity) as Image;
            rui.RegionMarkerImg.transform.SetParent(parent);
            rui.RegionMarkerImg.GetComponent<RectTransform>().localScale = Vector3.one;
        }
    }

    void DeActivateRUI(RegionUI rui) {
        if (rui.E1 != null)
            rui.E1.gameObject.SetActive(false);
        if (rui.E2 != null)
            rui.E2.gameObject.SetActive(false);
        if (rui.RegionMarkerImg != null)
            rui.RegionMarkerImg.gameObject.SetActive(false);
        if (rui.rNamelabel != null)
            rui.rNamelabel.gameObject.SetActive(false);
    }

    public void UpdateAngles(string rName, bool bE1, float E1, bool bE2, float E2) {
        if (_rUI.rUIDict.ContainsKey(rName)) {
            RegionUI rui = _rUI.rUIDict[rName];
            if (bE1)
                rui.E1.text = Mathf.Round(E1).ToString();
            if (bE2)
                rui.E2.text = Mathf.Round(E2).ToString();
        }
    }
}

public class RRenderDS
{
    public Dictionary<string, RegionUI> rUIDict = new Dictionary<string, RegionUI>();
}

public class RegionUI
{
    public Text rNamelabel = null, E1 = null, E2 = null;
    public Image RegionMarkerImg = null;
}