using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Zenject;

public class HomeCtrl : MonoBehaviour, IInitializable, IDisposable
{
    [Inject]
    SignalBus _signalBus;

    [Inject]
    ZenjectSceneLoader _sceneLoader;

    [Inject]
    IRegionProvider _regionProvider;

    [Inject]
    IUnitProvider _unitProvider;

    [Inject]
    IImgProvider _imgProvider;

    public Text TxtDelImgName, TxtMsgImgDelConfirm;
    public RawImage scrollViewImgContent;
    public Image DialogPanels, DeleteImgPanel,
        DelDecisionPanel, DelConfirmPanel;
    public Dropdown homeDDN;
    public Button BtnToRegionScene, BtnToUnitScene;
    
    void Start()
    {
        homeDDN.onValueChanged.AddListener(delegate { OnChange_HomeDDN(homeDDN); });
        if (homeDDN.options.Count > 0) {
            homeDDN.value = 0;
            homeDDN.RefreshShownValue();
        }
    }

    public void Initialize()
    {
        _signalBus.Subscribe<UrlsUpdateEvt>(OnUrlsUpdateEvt);
        _signalBus.Subscribe<UnitLatestDSUpdateEvt>(OnUnitLatestDSUpdate);
        _signalBus.Subscribe<RegionsDownloaded>(OnRegionsDownloaded);

        _signalBus.Fire<HomeInitEvt>();
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<UrlsUpdateEvt>(OnUrlsUpdateEvt);
        _signalBus.Unsubscribe<UnitLatestDSUpdateEvt>(OnUnitLatestDSUpdate);
        _signalBus.Unsubscribe<RegionsDownloaded>(OnRegionsDownloaded);
        _signalBus.Fire<ImgUploadRevokePermissionEvt>();// disable JS img btn
    }

    public void OnBtnNewImg()
    {
        _signalBus.Fire<ImgUploadPermissionEvt>();
    }

    public void OnBtnDelDdnImg() {
        // delete cur sel hDDN img if not "--"
        if (homeDDN.options[homeDDN.value].text == "--") return;
        string dkey = homeDDN.options[homeDDN.value].text;
        string dUrl = _imgProvider.GetUrlDict()[dkey];
        string uUrl = _unitProvider.GetCurrentUrl();
        // do not let if latest unit url the same
        if (dUrl == uUrl) return;

        TxtDelImgName.text = homeDDN.options[homeDDN.value].text;
        TxtMsgImgDelConfirm.text = homeDDN.options[homeDDN.value].text;
        DialogPanels.gameObject.SetActive(true);
        DeleteImgPanel.gameObject.SetActive(true);
        DelDecisionPanel.gameObject.SetActive(true);
    }

    public void OnBtnDelImgConfirm() {
        string delDateKey = homeDDN.options[homeDDN.value].text;

        scrollViewImgContent.texture = null;

        BtnToRegionScene.interactable = false;
        BtnToUnitScene.interactable = false;

        _imgProvider.DeleteFromAppImgDSByKey(delDateKey);
        _imgProvider.ReqDeleteImgByDateKey(delDateKey);

        _regionProvider.ReNewRegionDS();

        RemoveOffHomeDDN(delDateKey);
        if (homeDDN.options.Count > 0) {
            homeDDN.value = 0;
            homeDDN.RefreshShownValue();
        }
    }

    public void OnBtnUnitScene()
    {
        _sceneLoader.LoadScene("UnitScene", LoadSceneMode.Single);
    }

    public void OnBtnToRegionScene()
    {
        _sceneLoader.LoadScene("RegionScene", LoadSceneMode.Single);
    }

    public void OnBtnExportCSV() {
        _regionProvider.ExportRegionsToCSV();
    }

    void OnUrlsUpdateEvt(UrlsUpdateEvt args) {
        Dictionary<string, string> _urlsDict = _imgProvider.GetUrlDict();
        string tmpText = SelectFromHomeDDN(args.LastUploadedImg);
        if (args.urlsDict != null && args.urlsDict.Count > 0) {
            ResetHomeDDN();
            int tmpVal = -1;
            int idx = 1;
            foreach (string t in _urlsDict.Keys) {
                Dropdown.OptionData d = new Dropdown.OptionData();
                d.text = t;
                homeDDN.options.Add(d);
                if (tmpText == d.text) {
                    tmpVal = idx;
                }
                idx++;
            }
            InitSelectHomeDDN(tmpVal);
        }
        TrySetImg(args);
        if (args.urlTexture != null) TryEnableBtnToUnitScene();
    }

    void TrySetImg(UrlsUpdateEvt args) {
        Dictionary<string, string> _urlsDict = _imgProvider.GetUrlDict();
        if (args.urlTexture != null && _urlsDict != null && _urlsDict.Count > 0)
        {
            string url = _urlsDict[homeDDN.options[homeDDN.value].text];
            if (args.lastReqUrl == url)// url was requested
                SetImg(args.urlTexture);
        }
    }

    void SetImg(Texture2D texture2D) {
        scrollViewImgContent.rectTransform.sizeDelta = new Vector2(texture2D.width, texture2D.height);
        scrollViewImgContent.texture = texture2D;
    }

    void OnChange_HomeDDN(Dropdown hddn) {
        // on every chg disable, enable when texture back
        BtnToRegionScene.interactable = false;
        _regionProvider.ReNewRegionDS();

        string url = GetCurrentUrl();
        if (string.IsNullOrEmpty(url)) return;

        _regionProvider.ReqRegionsByUrl(url);

        QueryTextureEvt qt = new QueryTextureEvt();
        qt.url = url;
        _signalBus.Fire(qt);
    }

    string GetCurrentUrl() {
        Dictionary<string, string> _urlsDict = _imgProvider.GetUrlDict();
        if (homeDDN.options.Count <= 1) return "";
        return _urlsDict[homeDDN.options[homeDDN.value].text];
    }

    void OnUnitLatestDSUpdate(UnitLatestDSUpdateEvt args)
    {
        if (args.unitLatest._magnitude > 0f)
        {
            TryEnableBtnRegionScene();
            TryEnableBtnToUnitScene();
            if (!string.IsNullOrEmpty(args.unitLatest._url) && args.unitLatest._texture == null)
            {
                if (_imgProvider.GetCurrentUrl() == args.unitLatest._url && _imgProvider.GetTexture2D() != null)
                    return;
            }
        }
    }

    string SelectFromHomeDDN(string LastUploadedImg) {
        if (!String.IsNullOrEmpty(LastUploadedImg))
           return LastUploadedImg;
        else
            return homeDDN.options[homeDDN.value].text;
    }

    void InitSelectHomeDDN(int tmpVal) {
        if (tmpVal >= 1)// if one was already selected, reselect it
            homeDDN.value = tmpVal;
        else if (homeDDN.options.Count > 1)// else if more than 1, select next to default one
            homeDDN.value = 1;
        else
            homeDDN.value = 0;// else, select the default "--" one

        homeDDN.RefreshShownValue();
    }

    void ResetHomeDDN() {
        homeDDN.ClearOptions();
        Dropdown.OptionData dummyOD = new Dropdown.OptionData("--");
        homeDDN.options.Add(dummyOD);
    }

    void TryEnableBtnRegionScene() {
        int iVote = 0;
        if (_unitProvider.GetCurrentUnitMagnitude() > 0f) ++iVote;
        if (_imgProvider.CurrentImgUrlBothExist()) ++iVote;

        if(_regionProvider.GetCurrentUrl() == GetCurrentUrl()) ++iVote;

        if (iVote >= 3) BtnToRegionScene.interactable = true;
        else BtnToRegionScene.interactable = false;
    }

    void TryEnableBtnToUnitScene() {
        if(_unitProvider.UnitUrlImgAllAvailable() 
            || _imgProvider.CurrentImgUrlBothExist())
            BtnToUnitScene.interactable = true;
        else
            BtnToUnitScene.interactable = false;
    }

    void OnRegionsDownloaded(RegionsDownloaded args) {
        TryEnableBtnRegionScene();
    }

    void RemoveOffHomeDDN(string dKey) {
        foreach (Dropdown.OptionData d in homeDDN.options) {
            if (d.text == dKey) {
                homeDDN.options.Remove(d);
                return;
            }
        }
    }
}
