using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AppUnit : MonoBehaviour, IUnitServiceReceiver, IUnitProvider, IDisposable
{
    [Inject]
    SignalBus _signalBus;

    [Inject]
    IUnitProxy _unitProxy;

    [Inject]
    IImgProvider _imgProvider;

    [Inject]
    IImgProxy _iImgProxy;

    UnitLatest _unitLatest = new UnitLatest();

    void Start() {
        _signalBus.Subscribe<HomeInitEvt>(OnInit);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<HomeInitEvt>(OnInit);
    }

    public void LastUnitDS_CB(string jsonUnitDS)
    {
        _unitLatest = JsonUtility.FromJson<UnitLatest>(jsonUnitDS);

        if (_unitLatest._url == _imgProvider.GetCurrentUrl() && _imgProvider.GetTexture2D() != null)
        {
            _unitLatest._texture = _imgProvider.GetTexture2D();
            PublishUnit();
            return;
        }
        _iImgProxy.ReqTexture(_unitLatest._url, ImgProxy.TextureCB_CallerID.APP_UNIT);
    }

    public void TextureU_CB(Texture2D unitTexture, string utUrl) {
        if (_unitLatest._url == utUrl) {
            _unitLatest._texture = unitTexture;
            PublishUnit();
        }
    }

    void OnInit() {
        if (_unitLatest.IsInInitState()) {
            QueryUnit();
        }
        PublishUnit();
    }

    void PublishUnit() {
        UnitLatestDSUpdateEvt evt = new UnitLatestDSUpdateEvt();
        evt.unitLatest = _unitLatest;
        _signalBus.Fire(evt);
    }

    public void QueryUnit() {
            _unitProxy.ReqLastUnitDS();
    }

    public float GetCurrentUnitMagnitude() {
        return _unitLatest._magnitude;
    }

    public bool UnitUrlImgAllAvailable() {
        if (!string.IsNullOrEmpty(_unitLatest._url)
            && _unitLatest._texture != null
            && _unitLatest._magnitude > 0f)
            return true;
        else return false;
    }

    public Texture2D GetTexture2D() {
        return _unitLatest._texture;
    }

    public UnitLatest GetUnitLatestDS() {
        return _unitLatest;
    }

    public void SaveUnit_CB(string uJSON) {
        _unitLatest = JsonUtility.FromJson<UnitLatest>(uJSON);
        if (!string.IsNullOrEmpty(_unitLatest._url)) {
            if (_unitLatest._url == _imgProvider.GetCurrentUrl() && _imgProvider.GetTexture2D() != null) {
                _unitLatest._texture = _imgProvider.GetTexture2D();
                PublishUnit();
                return;
            }
            _iImgProxy.ReqTexture(_unitLatest._url, ImgProxy.TextureCB_CallerID.APP_UNIT);
        }
    }

    public void ReqSaveUnitJSON(string uJSON) {
        _unitProxy.ReqSaveUnitJSON(uJSON);
    }

    public SignalBus Tmp__SignalBus()
    {
        return _signalBus;
    }

    public void Tmp__ReInitSignal(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<HomeInitEvt>(OnInit);
    }

    public string GetCurrentUrl() {
        return _unitLatest._url;
    }

    
    public void ReNewUnitLatestDS() {
        _unitLatest = new UnitLatest();
    }
}