using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Zenject;
using UnityEngine.SceneManagement;

public class UnitCtrl : MonoBehaviour, IDisposable//IFSMReceiver, , IInitializable
{
    [Inject]
    SignalBus _signalBus;

    [Inject]
    ZenjectSceneLoader _sceneLoader;

    [Inject]
    IUnitProvider _unitProvider;

    [Inject]
    IImgProvider _imgProvider;

    [Inject]
    IUnitFSM _unitFSM;

    //UnitLatest _unitLatest = new UnitLatest();

    UnitLatest _uDraft = new UnitLatest();

    public RawImage scrollViewImgContent;
    public Text txt_xa, txt_xb, TxtSceneLocked;
    public Image LineImg, ScreenBlockerPanel, DialogPanels;
    public Button BtBtnBackHome;
    public Button BtnCommitUnit;

    void Start() {
        //_signalBus.Fire<UnitInitEvt>();
        _signalBus.Subscribe<UnitLatestDSUpdateEvt>(OnUnitLatestDSUpdate);
        _signalBus.Subscribe<UnitOriginPointEvt>(OnUnitOriginPointEvt);
        _signalBus.Subscribe<UnitTerminusPointEvt>(OnUnitTerminusPointEvt);
        _signalBus.Subscribe<UnitLineImgUpdateEvt>(OnUnitLineImgUpdateEvt);

        UnitContextSetup();
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<UnitLatestDSUpdateEvt>(OnUnitLatestDSUpdate);
        _signalBus.Unsubscribe<UnitOriginPointEvt>(OnUnitOriginPointEvt);
        _signalBus.Unsubscribe<UnitTerminusPointEvt>(OnUnitTerminusPointEvt);
        _signalBus.Unsubscribe<UnitLineImgUpdateEvt>(OnUnitLineImgUpdateEvt);
    }

    /* Btn handlers */

    /* IFSMReceiver implementation */
    void SetOriginPoint(Vector2 vp1) {
        if (vp1.Equals(Vector2.zero)) {
            txt_xa.gameObject.SetActive(false);
        }
        else
        {
            txt_xa.gameObject.GetComponent<RectTransform>().anchoredPosition = vp1;
            txt_xa.gameObject.SetActive(true);
        }
    }

    void SetTerminusPoint(Vector2 vp2) {
        if (vp2.Equals(Vector2.zero))
        {
            txt_xb.gameObject.SetActive(false);
        }
        else
        {
            txt_xb.gameObject.GetComponent<RectTransform>().anchoredPosition = vp2;
            txt_xb.gameObject.SetActive(true);
        }
    }

    void SetLineImgUpdate(Vector2 anchoredPosition, Vector2 sizeDelta, 
        float angle, float magnitude) {

        if (anchoredPosition.Equals(Vector2.zero))
        {
            LineImg.gameObject.SetActive(false);
        }
        else
        {
            RectTransform rt = LineImg.gameObject.GetComponent<RectTransform>();
            LineImg.gameObject.transform.rotation = Quaternion.identity;

            rt.anchoredPosition = anchoredPosition;
            rt.sizeDelta = sizeDelta;
            rt.Rotate(new Vector3(0, 0, angle));

            LineImg.gameObject.SetActive(true);
        }
    }

    void UnitContextSetup() {
        if (_unitProvider.UnitUrlImgAllAvailable())
            OnBtnLastUnit();
        else// we assume it now exists, or user could not be here
            OnBtnNewUnit();
    }

    bool IsAppUnitOnImg() {
        if (!_unitProvider.UnitUrlImgAllAvailable()) return false;
        if (scrollViewImgContent.texture == null) return false;
        if (scrollViewImgContent.texture != _unitProvider.GetTexture2D()) return false;
        return true;
    }
    
    bool IsAppImgOnImg() {
        if (!_imgProvider.CurrentImgUrlBothExist()) return false;
        if (scrollViewImgContent.texture == null) return false;
        if (scrollViewImgContent.texture != _imgProvider.GetTexture2D()) return false;
        return true;
    }

    void BlockScreen(bool toSet, string msg="") {
        TxtSceneLocked.text = msg;
        DialogPanels.gameObject.SetActive(toSet);
        ScreenBlockerPanel.gameObject.SetActive(toSet);
    }

    void ReqLastUnit() {
        _unitProvider.QueryUnit();
    }

    void AppUnitToImg() {
        SetImg(_unitProvider.GetTexture2D());
    }

    void AppUnitToDraft() {
        _uDraft = _unitProvider.GetUnitLatestDS();
    }

    void DraftToUFSM() {
        if (_uDraft._p1.Equals(Vector2.zero) || _uDraft._p2.Equals(Vector2.zero))
        {
            _unitFSM.ClickFSMHandler(Vector2.zero, null, true);
        }
        else
        {
            PointerEventData ed = new PointerEventData(EventSystem.current);
            ed.button = PointerEventData.InputButton.Left;
            _unitFSM.ClickFSMHandler(Vector2.zero, null, true);
            _unitFSM.ClickFSMHandler(_uDraft._p1, ed);
            _unitFSM.ClickFSMHandler(_uDraft._p2, ed);
        }
    }

    void AppImgToImg() {
        SetImg(_imgProvider.GetTexture2D());
    }

    void ReNewDraft() {
        _uDraft = new UnitLatest();
        _uDraft._texture = scrollViewImgContent.texture as Texture2D;
        if (_imgProvider.GetTexture2D() == _uDraft._texture)
            _uDraft._url = _imgProvider.GetCurrentUrl();
        else if (_unitProvider.GetTexture2D() == _uDraft._texture)
            _uDraft._url = _unitProvider.GetCurrentUrl();
        // TODO: else, throw exception
    }

    void SetImg(Texture2D t2D) {
        scrollViewImgContent.rectTransform.sizeDelta = new Vector2(t2D.width, t2D.height);
        scrollViewImgContent.texture = t2D;
    }

    /* Btn Handlers */

    public void OnBtnBackHome()
    {
        BackHome();
    }

    void BackHome()
    {
        _sceneLoader.LoadScene("HomeScene", LoadSceneMode.Single);
    }

    public void OnBtnCommitUnit() {
        if (_unitFSM.GetUFSMState() == UnitFSM.UnitState.UNIT_FINAL)
        {
            _uDraft._p1 = _unitFSM.GetOrigin();
            _uDraft._p2 = _unitFSM.GetTerminus();
            _uDraft._magnitude = Vector2.Distance(_uDraft._p1, _uDraft._p2);
        }
        else
        {
            _uDraft._p1 = Vector2.zero;
            _uDraft._p2 = Vector2.zero;
            _uDraft._magnitude = 0f;
        }
    }

    public void OnBtnDiscard() {
        if (IsAppUnitOnImg())
            AppUnitToDraft();
        else
            ReNewDraft();

        DraftToUFSM();
    }

    public void OnBtnSave() {
        // TODO: on err, inform abt err and unblock
        if (_uDraft._magnitude > 0f)
        {
            BlockScreen(true, "Please wait while saving unit on server is going on...");
            string uJSON = JsonUtility.ToJson(_uDraft);
            _unitProvider.ReqSaveUnitJSON(uJSON);
        }
    }

    public void OnBtnLastUnit() {
        // home-init requests latest unit, so if it does not exist, nothind to do
        if (_unitProvider.UnitUrlImgAllAvailable())
        {
            AppUnitToImg();
            AppUnitToDraft();
            DraftToUFSM();
        }
    }

    public void OnBtnNewUnit() {
        AppImgToImg();
        ReNewDraft();
        DraftToUFSM();
    }

    /* End: Btn Handlers */


    /* Event Handlers */
    void OnUnitLatestDSUpdate(UnitLatestDSUpdateEvt args)
    {
        BlockScreen(false);
    }

    void OnUnitOriginPointEvt(UnitOriginPointEvt args) {
        SetOriginPoint(args.vorigin);
    }

    void OnUnitTerminusPointEvt(UnitTerminusPointEvt args) {
        SetTerminusPoint(args.vterminus);
    }

    void OnUnitLineImgUpdateEvt(UnitLineImgUpdateEvt args) {
        SetLineImgUpdate(args.anchoredPosition, args.sizeDelta, args.angle, args.magnitude);
    }

    /* End: Event Handlers */
}