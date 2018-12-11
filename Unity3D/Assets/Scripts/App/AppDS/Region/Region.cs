using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Region  {
    // on db side, save the unix timestamp of saving region too
    public string _rName = "";
    public string _imgUrl = "";

    // diagonal vertices
    public Vector2 _p1 = Vector2.zero;
    public Vector2 _p2 = Vector2.zero;

    // marker center and outline effective distance (Outline cmp on game obj)
    public Vector2 _center = Vector2.zero;
    public Vector2 _effectDistance = Vector2.zero;

    public float _e1 = 0f;
    public float _e2 = 0f;

    public string dateKey;// the date of creation or update
    public Vector2 _a1p1 = Vector2.zero;
    public Vector2 _a1p2 = Vector2.zero;
    public float _a1m = 0f;

    public Vector2 _a2p1 = Vector2.zero;
    public Vector2 _a2p2 = Vector2.zero;
    public float _a2m = 0f;

    public Vector2 _b1p1 = Vector2.zero;
    public Vector2 _b1p2 = Vector2.zero;
    public float _b1m = 0f;

    public Vector2 _b2p1 = Vector2.zero;
    public Vector2 _b2p2 = Vector2.zero;
    public float _b2m = 0f;

    public Vector2 _c1p1 = Vector2.zero;
    public Vector2 _c1p2 = Vector2.zero;
    public float _c1m = 0f;

    public Vector2 _c2p1 = Vector2.zero;
    public Vector2 _c2p2 = Vector2.zero;
    public float _c2m = 0f;

    public Vector2 _d1p1 = Vector2.zero;
    public Vector2 _d1p2 = Vector2.zero;
    public float _d1m = 0f;

    public Vector2 _d2p1 = Vector2.zero;
    public Vector2 _d2p2 = Vector2.zero;
    public float _d2m = 0f;
}

[System.Serializable]
public class RegionSet {
    public Region[] regions;
    public string url;
}