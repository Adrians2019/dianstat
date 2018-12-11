using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

// on convas, so auto-bound by scene context
public class SRenderEngine : MonoBehaviour, SRenderUI {
    public Transform parent;
    public Text txt_xa;
    public Image LineImg;
    Color sColor = new Color(1f, 0f, 0f);// red
    Color tColor = new Color(0.52f, 0.94f, 0.98f);// lbl: very light blue
    Vector2 vLOffset = new Vector2(5f, -20f);
    Vector2 vSeizeLBL = new Vector2(200f, 40f);
    float qToUnitMM = 1f;

    SRenderDS _sUI = new SRenderDS();

    public SRenderDS SegmentDS() {
        return _sUI;
    }

    public void ToggleSUI(string rName, string sName, bool toSet) {
        if(_sUI.sUIDict[rName][sName].LineImg != null)
            _sUI.sUIDict[rName][sName].LineImg.gameObject.SetActive(toSet);
        if (_sUI.sUIDict[rName][sName].Measurelabel != null)
            _sUI.sUIDict[rName][sName].Measurelabel.gameObject.SetActive(toSet);
    }

    public void RenderSegments(Region[] regions) {
        Dictionary<string, SegmentUI> sDict = null;
        for (int i = 0; i < regions.Length; i++) {
            if (!_sUI.sUIDict.ContainsKey(regions[i]._rName))
                _sUI.sUIDict[regions[i]._rName] = new Dictionary<string, SegmentUI>();
            sDict = _sUI.sUIDict[regions[i]._rName];
            RenderSegmentsOfSDict(regions[i], sDict, true);
        }
    }

    public Dictionary<string, SegmentUI> SelectSegmentDict(string rName) {
        return _sUI.sUIDict[rName];
    }

    // source of the region may be either rDict, if need to render original, 
    // or rDraft, to render the user-updated region
    public void RenderSegmentsOfSDict(Region region, Dictionary<string, SegmentUI> sDict, bool toSet) {
        string[] sNames = { SEGNAME.A1, SEGNAME.A2, SEGNAME.B1, SEGNAME.B2, SEGNAME.C1, SEGNAME.C2, SEGNAME.D1, SEGNAME.D2 };
        foreach (string sn in sNames) {
            UpdateSegmentUI(region, sDict, sn, toSet);
        }
    }

    public void UpdateSegmentUI(Region region, Dictionary<string, SegmentUI> sDict, string sName, bool toSet) {
        SegmentUI s = null;
        if (!sDict.ContainsKey(sName))
        {
            sDict[sName] = new SegmentUI();
        }
        s = sDict[sName];
        if (s.Measurelabel == null)
        {
            s.Measurelabel = Instantiate(txt_xa, Vector3.zero, Quaternion.identity) as Text;
            s.Measurelabel.transform.SetParent(parent);
        }
        if (s.LineImg == null)
        {
            s.LineImg = Instantiate(LineImg, Vector3.zero, Quaternion.identity) as Image;
            s.LineImg.transform.SetParent(parent);
        }

        ConfigSegmentUI(s.LineImg, s.Measurelabel, region, sName, toSet);
    }


    public void ConfigSegmentUI(Image LineImg, Text Measurelabel, Region region, string sName, bool toSet) {
        switch (sName) {
            case SEGNAME.A1:
                VisualizeSegment(region._a1p1, region._a1p2, region._a1m, Measurelabel, LineImg, sName, toSet);
                break;
            case SEGNAME.A2:
                VisualizeSegment(region._a2p1, region._a2p2, region._a2m, Measurelabel, LineImg, sName, toSet);
                break;
            case SEGNAME.B1:
                VisualizeSegment(region._b1p1, region._b1p2, region._b1m, Measurelabel, LineImg, sName, toSet);
                break;
            case SEGNAME.B2:
                VisualizeSegment(region._b2p1, region._b2p2, region._b2m, Measurelabel, LineImg, sName, toSet);
                break;
            case SEGNAME.C1:
                VisualizeSegment(region._c1p1, region._c1p2, region._c1m, Measurelabel, LineImg, sName, toSet);
                break;
            case SEGNAME.C2:
                VisualizeSegment(region._c2p1, region._c2p2, region._c2m, Measurelabel, LineImg, sName, toSet);
                break;
            case SEGNAME.D1:
                VisualizeSegment(region._d1p1, region._d1p2, region._d1m, Measurelabel, LineImg, sName, toSet);
                break;
            case SEGNAME.D2:
                VisualizeSegment(region._d2p1, region._d2p2, region._d2m, Measurelabel, LineImg, sName, toSet);
                break;
        }
    }

    void VisualizeSegment(Vector2 p1, Vector2 p2, float CM_magnitude, Text txtLabel, Image LineImg, string sName, bool toSet)
    {
        if (p1.x == p2.x && p1.y == p2.y)
        {
            txtLabel.gameObject.SetActive(false);
            LineImg.gameObject.SetActive(false);
            return;
        }

        float MM_magnitude = CM_magnitude * qToUnitMM;

        Vector2 dir = (p1 - p2).normalized;
        float vecLineDist = Vector2.Distance(p2, p1);// magnitude
        Vector2 vpos = p2 + ((dir * vecLineDist) / 2);

        txtLabel.text = sName + ": " + MM_magnitude.ToString() + " mm";
        txtLabel.GetComponent<RectTransform>().localScale = Vector3.one;
        txtLabel.GetComponent<RectTransform>().anchoredPosition = vpos + vLOffset;
        txtLabel.GetComponent<RectTransform>().sizeDelta = vSeizeLBL;
        txtLabel.color = tColor;
        txtLabel.gameObject.SetActive(toSet);
        
        LineImg.GetComponent<RectTransform>().localScale = Vector3.one;
        LineImg.GetComponent<RectTransform>().anchoredPosition = vpos;
        LineImg.GetComponent<RectTransform>().sizeDelta = new Vector2(vecLineDist, 2f);
        LineImg.gameObject.transform.rotation = Quaternion.identity;
        LineImg.color = sColor;

        float sign = (p2.y < p1.y) ? 1.0f : -1.0f;
        float angle = Vector2.Angle(Vector2.right, dir) * sign;
        LineImg.GetComponent<RectTransform>().Rotate(new Vector3(0, 0, angle));
        LineImg.gameObject.SetActive(toSet);
    }
}

public class SRenderDS
{
    public Dictionary<string, Dictionary<string, SegmentUI>> sUIDict = new Dictionary<string, Dictionary<string, SegmentUI>>();
}

public class SegmentUI
{
    public Text Measurelabel = null;
    public Image LineImg = null;
}

public static class SEGNAME
{
    public const string A1 = "A1";
    public const string A2 = "A2";

    public const string B1 = "B1";
    public const string B2 = "B2";

    public const string C1 = "C1";
    public const string C2 = "C2";

    public const string D1 = "D1";
    public const string D2 = "D2";
};