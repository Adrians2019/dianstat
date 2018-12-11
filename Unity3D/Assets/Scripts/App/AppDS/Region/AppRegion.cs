using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AppRegion : MonoBehaviour, IRegionServiceReceiver, IRegionProvider
{

    [Inject]
    SignalBus _signalBus;

    [Inject]
    IRegionProxy _regionProxy;

    [Inject]
    IImgProvider _imgProvider;

    [Inject]
    IUnitProvider _unitProvider;

    RegionDS _rDS = new RegionDS();

    public const float qToMM = 10f;

    public void RegionsByUrl_CB(string jsonRegionsDS) {
        RegionSet rset = JsonUtility.FromJson<RegionSet>(jsonRegionsDS);
        _rDS.curUrl = rset.url;

        RegionsDownloaded evt = new RegionsDownloaded();
        evt.url = rset.url;
        _signalBus.Fire(evt);

        if (rset != null && rset.regions != null) {
            UpdateRDictBy(rset.regions);
        }
    }

    public void UpdatedRegion_CB(string jsonSingleRegionDS) {
        Region r = JsonUtility.FromJson<Region>(jsonSingleRegionDS);
        Region[] rs = new Region[1];
        rs[0] = r;
        UpdateRDictBy(rs);
        RegionUpdatedEvt evt = new RegionUpdatedEvt();
        evt.newRegion = r;
        _signalBus.Fire(evt);
    }

    public Dictionary<string, Region> GetRegions() {
        return _rDS._rDict;
    }

    public void OverwriteSDraft(Vector2 p1, Vector2 p2, string sName) {
        float mNormalized = setMagnitude(p1, p2, _unitProvider.GetCurrentUnitMagnitude());
        switch (sName) {
            case SEGNAME.A1:
                _rDS.rDraft._a1p1 = p1;
                _rDS.rDraft._a1p2 = p2;
                _rDS.rDraft._a1m = mNormalized;
                break;
            case SEGNAME.A2:
                _rDS.rDraft._a2p1 = p1;
                _rDS.rDraft._a2p2 = p2;
                _rDS.rDraft._a2m = mNormalized;
                break;
            case SEGNAME.B1:
                _rDS.rDraft._b1p1 = p1;
                _rDS.rDraft._b1p2 = p2;
                _rDS.rDraft._b1m = mNormalized;
                break;
            case SEGNAME.B2:
                _rDS.rDraft._b2p1 = p1;
                _rDS.rDraft._b2p2 = p2;
                _rDS.rDraft._b2m = mNormalized;
                break;
            case SEGNAME.C1:
                _rDS.rDraft._c1p1 = p1;
                _rDS.rDraft._c1p2 = p2;
                _rDS.rDraft._c1m = mNormalized;
                break;
            case SEGNAME.C2:
                _rDS.rDraft._c2p1 = p1;
                _rDS.rDraft._c2p2 = p2;
                _rDS.rDraft._c2m = mNormalized;
                break;
            case SEGNAME.D1:
                _rDS.rDraft._d1p1 = p1;
                _rDS.rDraft._d1p2 = p2;
                _rDS.rDraft._d1m = mNormalized;
                break;
            case SEGNAME.D2:
                _rDS.rDraft._d2p1 = p1;
                _rDS.rDraft._d2p2 = p2;
                _rDS.rDraft._d2m = mNormalized;
                break;
        }
    }

    float setMagnitude(Vector2 p1, Vector2 p2, float unit) {
        unit = Mathf.Abs(unit);
        if (unit == 0f)
            return 0f;
        float magnitude = Mathf.Abs(Vector2.Distance(p1, p2) / unit) * qToMM;
        return Mathf.Round(magnitude);
    }

    void UpdateRDictBy(Region[] regions) {
        if(_rDS._rDict == null) _rDS._rDict = new Dictionary<string, Region>();
        if (regions.Length > 0)
        {
            for (int i = 0; i < regions.Length; i++)
            {
                if (!_rDS._rDict.ContainsKey(regions[i]._rName))
                    _rDS._rDict.Add(regions[i]._rName, regions[i]);
                else
                    _rDS._rDict[regions[i]._rName] = regions[i];
            }
        }
    }

    public void ReqRegionsByUrl(string url) {
        _regionProxy.ReqRegionsByUrl(url);
    }

    public void NewDraft(string rName) {
        // TODO: separate methods, in one not allowed rName empty/null
        if (!string.IsNullOrEmpty(rName))
        {
            if (_rDS._rDict.ContainsKey(rName))
                _rDS.rDraft = _rDS._rDict[rName];
            else
                // TODO: throw exception!
                Debug.Log("Region does not exist!");
        }
        else
            _rDS.rDraft = new Region();
    }

    public void OverwriteDraftName(string rName) {
        _rDS.rDraft._rName = rName;
    }

    public void OverwriteDraftDiagonal(Vector2 p1, Vector2 p2) {
        _rDS.rDraft._p1 = p1;
        _rDS.rDraft._p2 = p2;
    }

    public void ServerSaveDraft() {
        // init server save by proxy calling
        string rJASON = JsonUtility.ToJson(_rDS.rDraft);
        _regionProxy.ReqSaveNewRegionDS(_rDS.rDraft._rName, rJASON);
    }

    public void OverwriteDraftUrl(string url) {
        _rDS.rDraft._imgUrl = url;
    }

    public void OverwriteDraftCenter(Vector2 vcenter) {
        _rDS.rDraft._center = vcenter;
    }

    public void OverwriteDraftEffDist(Vector2 veffDist) {
        _rDS.rDraft._effectDistance = veffDist;
    }

    public void OverwriteDraftSegment(Vector2 toP1, Vector2 toP2, string sName) {
        _rDS.OwDraftSegment(toP1, toP2, sName, _unitProvider.GetCurrentUnitMagnitude());
    }

    public Vector2[] GetDrafSegmentVertices(string sName) {
        return _rDS.GetDrafSegmentVertices(sName);
    }

    public bool ZeroDraftSegment(string sName) {
        return _rDS.IsZeroDraftSegment(sName);
    }

    public float ComputeAngleOf(Region r, string sName1, string sName2) {
        if (!_rDS.SingleCommonVertexExists(r, sName1, sName2)) return 0f;
        Vector2[] vs1 = _rDS.GetDrafSegmentVertices(sName1);
        Vector2[] vs2 = _rDS.GetDrafSegmentVertices(sName2);
        Vector2 n1 = (vs1[0] - vs1[1]).normalized;
        Vector2 n2 = (vs2[0] - vs2[1]).normalized;
        float angle = Vector2.Angle(n1, n2);
        return Mathf.Round(angle);
    }

    public Region GetDraft() {
        return _rDS.rDraft;
    }

    public Vector2[] GetRegionSegmentVertices(string rName, string sName) {
        return _rDS.GetRegionSegmentVertices(rName, sName);
    }

    public Region GetRegionByName(string rName) {
        return _rDS._rDict[rName];
    }

    public void ReqDeleteRegionByName(string rName) {
        _regionProxy.ReqDeleteRegionByName(rName);
    }

    public void ExportRegionsToCSV() {
        _regionProxy.ExportRegionsToCSV();
    }

    public SignalBus Tmp__SignalBus()
    {
        return _signalBus;
    }

    public string GetCurrentUrl() {
        return _rDS.curUrl;
    }

    public void ReNewRegionDS() {
        _rDS = new RegionDS();
    }
}


[Serializable]
public class RegionDS {
    public Dictionary<string, Region> _rDict = null;
    public Region rDraft = new Region();
    public string curUrl = "";

    public bool IsZeroDraftSegment(string sName) {
        Vector2 p1 = GetDrafSegmentVertices(sName)[0];
        Vector2 p2 = GetDrafSegmentVertices(sName)[1];
        if (p1.Equals(Vector2.zero) || p2.Equals(Vector2.zero)) return true;
        if (p1.x == p2.x && p1.y == p2.y) return true;
        return false;
    }

    public bool SingleCommonVertexExists(Region r, string sName1, string sName2) {
        Vector2[] vs1 = GetRegionSegmentVertices(r._rName, sName1);
        Vector2[] vs2 = GetRegionSegmentVertices(r._rName, sName2);
        if (vs1[0].Equals(vs2[0]) && !vs1[1].Equals(vs2[1])) return true;
        else if (!vs1[0].Equals(vs2[0]) && vs1[1].Equals(vs2[1])) return true;
        else if (vs1[1].Equals(vs2[0]) && !vs1[1].Equals(vs2[0])) return true;
        else if (!vs1[0].Equals(vs2[1]) && vs1[0].Equals(vs2[1])) return true;
        else return false;
    }

    public Vector2[] GetRegionSegmentVertices(string rName, string sName)
    {
        Vector2[] vertices = new Vector2[2];
        Region r = _rDict[rName];
        switch (sName)
        {
            case SEGNAME.A1:
                vertices[0] = r._a1p1;
                vertices[1] = r._a1p2;
                return vertices;
            case SEGNAME.A2:
                vertices[0] = r._a2p1;
                vertices[1] = r._a2p2;
                return vertices;
            case SEGNAME.B1:
                vertices[0] = r._b1p1;
                vertices[1] = r._b1p2;
                return vertices;
            case SEGNAME.B2:
                vertices[0] = r._b2p1;
                vertices[1] = r._b2p2;
                return vertices;
            case SEGNAME.C1:
                vertices[0] = r._c1p1;
                vertices[1] = r._c1p2;
                return vertices;
            case SEGNAME.C2:
                vertices[0] = r._c2p1;
                vertices[1] = r._c2p2;
                return vertices;
            case SEGNAME.D1:
                vertices[0] = r._d1p1;
                vertices[1] = r._d1p2;
                return vertices;
            case SEGNAME.D2:
                vertices[0] = r._d2p1;
                vertices[1] = r._d2p2;
                return vertices;
        }

        return new Vector2[] { Vector2.zero, Vector2.zero };
    }

    public Vector2[] GetDrafSegmentVertices(string sName)
    {
        Vector2[] vertices = new Vector2[2];

        switch (sName)
        {
            case SEGNAME.A1:
                vertices[0] = rDraft._a1p1;
                vertices[1] = rDraft._a1p2;
                return vertices;
            case SEGNAME.A2:
                vertices[0] = rDraft._a2p1;
                vertices[1] = rDraft._a2p2;
                return vertices;
            case SEGNAME.B1:
                vertices[0] = rDraft._b1p1;
                vertices[1] = rDraft._b1p2;
                return vertices;
            case SEGNAME.B2:
                vertices[0] = rDraft._b2p1;
                vertices[1] = rDraft._b2p2;
                return vertices;
            case SEGNAME.C1:
                vertices[0] = rDraft._c1p1;
                vertices[1] = rDraft._c1p2;
                return vertices;
            case SEGNAME.C2:
                vertices[0] = rDraft._c2p1;
                vertices[1] = rDraft._c2p2;
                return vertices;
            case SEGNAME.D1:
                vertices[0] = rDraft._d1p1;
                vertices[1] = rDraft._d1p2;
                return vertices;
            case SEGNAME.D2:
                vertices[0] = rDraft._d2p1;
                vertices[1] = rDraft._d2p2;
                return vertices;
        }
        return new Vector2[] { Vector2.zero, Vector2.zero };
    }

    public void OwDraftSegment(Vector2 p1, Vector2 p2, string sName, float unit) {

        float magnitude = 0f;
        if (unit > 0f)
            magnitude = Mathf.Round(Mathf.Abs(Vector2.Distance(p1, p2) / unit) * AppRegion.qToMM);

        switch (sName) {
            case SEGNAME.A1:
                rDraft._a1p1 = p1;
                rDraft._a1p2 = p2;
                rDraft._a1m = magnitude;
                return;
            case SEGNAME.A2:
                rDraft._a2p1 = p1;
                rDraft._a2p2 = p2;
                rDraft._a2m = magnitude;
                return;
            case SEGNAME.B1:
                rDraft._b1p1 = p1;
                rDraft._b1p2 = p2;
                rDraft._b1m = magnitude;
                return;
            case SEGNAME.B2:
                rDraft._b2p1 = p1;
                rDraft._b2p2 = p2;
                rDraft._b2m = magnitude;
                return;
            case SEGNAME.C1:
                rDraft._c1p1 = p1;
                rDraft._c1p2 = p2;
                rDraft._c1m = magnitude;
                return;
            case SEGNAME.C2:
                rDraft._c2p1 = p1;
                rDraft._c2p2 = p2;
                rDraft._c2m = magnitude;
                return;
            case SEGNAME.D1:
                rDraft._d1p1 = p1;
                rDraft._d1p2 = p2;
                rDraft._d1m = magnitude;
                return;
            case SEGNAME.D2:
                rDraft._d2p1 = p1;
                rDraft._d2p2 = p2;
                rDraft._d2m = magnitude;
                return;
        }        
    }
}
