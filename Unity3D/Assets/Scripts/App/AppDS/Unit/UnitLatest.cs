using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitLatest
{
    public Vector2 _p1;
    public Vector2 _p2;
    public float _magnitude;
    public string _url;
    public Texture2D _texture;
    public string dateKey;

    public UnitLatest()
    {
        FullReset();
    }

    public void VectorRest()
    {
        _p1 = Vector2.zero;
        _p2 = Vector2.zero;
        _magnitude = 0f;
    }

    public void FullReset()
    {
        _p1 = Vector2.zero;
        _p2 = Vector2.zero;
        _magnitude = 0f;
        _url = "";
        _texture = null;
        dateKey = "";
    }

    public bool IsInInitState() {
        if (_p1 == Vector2.zero &&
        _p2 == Vector2.zero &&
        _magnitude == 0f &&
        string.IsNullOrEmpty(_url) &&
        _texture == null &&
        string.IsNullOrEmpty(dateKey))
            return true;
        else
            return false;
    }

    public override string ToString()
    {
        return "date key: '"+ dateKey + "',  p1: " + _p1.ToString() + ",  p2: " + _p2.ToString() + ",  Magnitude: " + _magnitude + ", last-url: '" + _url + "',  is texture null ? " + (_texture == null);
    }
}