using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Zenject;

public class RegionCtrl : MonoBehaviour, IInitializable, IDisposable, IRegionCtrl
{

    [Inject]
    SignalBus _signalBus;

    [Inject]
    ZenjectSceneLoader _sceneLoader;

    [Inject]
    IRegionProvider _regionProvider;
    
    [Inject]
    IImgProvider _imgProvider;

    [Inject]
    SRenderUI _sRenderUI;
    
    [Inject]
    RRenderUI _rRenderUI;
    
    [Inject]
    IREnvFSM _rEnvFSM;

    public RawImage scrollViewImgContent;
    // regions, segments, segments references to draw points from
    public Dropdown rDDN, sDDN, cDDN;
    public Button BtnSave, BtnDiscard, BtnCommit, BtnDeleteRegion;
    public Image DialogPanels, RegionNewSavePanel, ScreenBlockerPanel,
        DeleteRegionPanel, DelDecisionPanel, DelConfirmPanel;
    public InputField InputTreeNumber, InputLeafNumber;
    public Text txt_scrBlockMsg, TxtDelRegionName, TxtMsgRegionDelConfirm;

    void Start () {
        
        if (_imgProvider.GetTexture2D() != null) {
            scrollViewImgContent.rectTransform.sizeDelta = new Vector2(_imgProvider.GetTexture2D().width, _imgProvider.GetTexture2D().height);
            scrollViewImgContent.texture = _imgProvider.GetTexture2D();
        }

        rDDN.onValueChanged.AddListener(delegate { OnChange_rDDN(rDDN); });
        sDDN.onValueChanged.AddListener(delegate { OnChange_sDDN(sDDN); });

        initRegions();
        ExecuteFSM();
    }

    public void Initialize()
    {
        _signalBus.Subscribe<RegionUpdatedEvt>(OnRegionUpdatedEvt);

        if (_regionProvider.GetRegions() != null)
            reset_rDDN(new List<string>(_regionProvider.GetRegions().Keys));
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<RegionUpdatedEvt>(OnRegionUpdatedEvt);
    }

    void OnChange_rDDN(Dropdown rDDN)
    {
        if (sDDN.value != 0) {//rDDN.value == 0 && 
            sDDN.value = 0;
            sDDN.RefreshShownValue();
            return;// because sDDN beign refreshed, will itself call exec-fsm
        }
        ExecuteFSM();
    }

    void OnChange_sDDN(Dropdown sDDN)
    {
        ExecuteFSM();
    }

    void reset_rDDN(List<string> rNameList){
        rDDN.ClearOptions();
        Dropdown.OptionData op = new Dropdown.OptionData();
        op.text = "--";
        rDDN.options.Add(op);
        rDDN.AddOptions(rNameList);
    }

    public void OnBtnDeleteRegion()
    {
        if (GetrDDN() == "--") return;

        TxtDelRegionName.text = "Delete: " + GetrDDN();
        TxtMsgRegionDelConfirm.text = "Delete: " + GetrDDN();
        DialogPanels.gameObject.SetActive(true);
        DeleteRegionPanel.gameObject.SetActive(true);
        DelDecisionPanel.gameObject.SetActive(true);
    }

    
    public void OnBtnDelSelectedRegion()
    {
        DelDecisionPanel.gameObject.SetActive(false);
        DelConfirmPanel.gameObject.SetActive(true);
    }
    public void OnBtnRegionDelConfirm()
    {
        Dropdown.OptionData x = null;
        foreach (Dropdown.OptionData d in rDDN.options) {
            if (d.text == GetrDDN()) x = d;
        }
        if (x != null) {
            rDDN.options.Remove(x);
            rDDN.value = 0;
            rDDN.RefreshShownValue();

            // request rUI and sUI to toggle off region and all its segments
            Region delReg = _regionProvider.GetRegionByName(x.text);
            _rRenderUI.RenderRLabelsOf(delReg, false);
            Dictionary<string, SegmentUI> sUIDict = _sRenderUI.SelectSegmentDict(delReg._rName);
            _sRenderUI.RenderSegmentsOfSDict(delReg, sUIDict, false);

            // send server req to del
            _regionProvider.ReqDeleteRegionByName(delReg._rName);
            delReg = null;
        }
        DelConfirmPanel.gameObject.SetActive(false);
        DeleteRegionPanel.gameObject.SetActive(false);
        DialogPanels.gameObject.SetActive(false);
    }

    public void OnBtnSave() {
        _rEnvFSM.GetCurrentStateEntity().OnSave();
    }

    public void OnBtnDiscard() {
        _rEnvFSM.GetCurrentStateEntity().OnDiscard();
    }

    public void OnBtnCommit() {
        _rEnvFSM.GetCurrentStateEntity().OnCommit();
    }

    public void OnBtnBackHome() {
        _sceneLoader.LoadScene("HomeScene", LoadSceneMode.Single);
    }

    public void initRegions() {
        Region[] rset;

        if (_regionProvider == null || _regionProvider.GetRegions() == null) {
            rset = new Region[0];
            Render(rset);
            return;
        }

        rset = new Region[_regionProvider.GetRegions().Count];
        int i = 0;
        foreach (Region r in _regionProvider.GetRegions().Values)
        {
            rset[i++] = r;
        }
        Render(rset);
    }

    void Render(Region[] regions)
    {
        _sRenderUI.RenderSegments(regions);
        _rRenderUI.RenderRLabels(regions);
    }

    void ExecuteFSM() {
        string r, s;
        r = rDDN.options[rDDN.value].text;
        s = sDDN.options[sDDN.value].text;

        string rNullText = "--";
        string sNullText = "--";

        if (r == rNullText && s == sNullText)
        {
            _rEnvFSM.ReqStateChange(REnvState.CreateRegion);
        }
        else if (r != rNullText && s == sNullText)
        {
            _rEnvFSM.ReqStateChange(REnvState.RegionUD);
        }
        else if (r != rNullText && s != sNullText)
        {
            _rEnvFSM.ReqStateChange(REnvState.SegmentCUD);
        }
    }

    public void RDDNEnable(bool toSet) {
        rDDN.interactable = toSet;
    }

    public void SDDNEnable(bool toSet) {
        sDDN.interactable = toSet;
    }

    public string GetrDDN()
    {
        return rDDN.options[rDDN.value].text;
    }

    public string GetsDDN()
    {
        return sDDN.options[sDDN.value].text;
    }

    public string GetcDDN()
    {
        return cDDN.options[cDDN.value].text;
    }

    public void EnableBtns() {
        BtnSave.interactable = true;
        BtnDiscard.interactable = true;
        BtnCommit.interactable = true;
        BtnDeleteRegion.interactable = true;
    }

    public void DisableBtns()
    {
        BtnSave.interactable = false;
        BtnDiscard.interactable = false;
        BtnCommit.interactable = false;
        BtnDeleteRegion.interactable = false;
    }

    public void DisableSave() { BtnSave.interactable = false; }
    public void DisableDiscard() { BtnDiscard.interactable = false; }
    public void DisableCommit() { BtnCommit.interactable = false; }
    public void DisableDeleteRegion() { BtnDeleteRegion.interactable = false; }

    public void EnableSave() { BtnSave.interactable = true; }
    public void EnableDiscard() { BtnDiscard.interactable = true; }
    public void EnableCommit() { BtnCommit.interactable = true; }
    public void EnableDeleteRegion() { BtnDeleteRegion.interactable = true; }

    public void NewRegionSavingDiag(bool ifShown) {
        NewRegionSavePanlel(ifShown);
    }

    public void RegionNewSavePanel_OnSave() {
        string rName = ToRegionName(InputTreeNumber.text, InputLeafNumber.text);
        if (string.IsNullOrEmpty(rName) || Has_rDDN(rName)) {
            TruncateRegionNameInputFields();
            return;
        }
        _regionProvider.OverwriteDraftName(rName);
        BlockScreen(true, "Waiting to save new region '" + rName + "' on server...");
        NewRegionSavePanlel(false);
        _rEnvFSM.GetCurrentStateEntity().OnSaveConfirmed();
        TruncateRegionNameInputFields();
    }

    public void RegionNewSavePanel_OnCancel() {
        TruncateRegionNameInputFields();
        NewRegionSavePanlel(false);
    }

    void NewRegionSavePanlel(bool ifShow) {
        DialogPanels.gameObject.SetActive(ifShow);
        RegionNewSavePanel.gameObject.SetActive(ifShow);
    }

    public void BlockScreen(bool ifBlock, string msg="")
    {
        txt_scrBlockMsg.text = msg;
        DialogPanels.gameObject.SetActive(ifBlock);
        ScreenBlockerPanel.gameObject.SetActive(ifBlock);
    }

    string ToRegionName(string name, string surname) {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname)) return "";
        return name + "_" + surname;
    }

    bool Has_rDDN(string rName) {
        foreach (Dropdown.OptionData d in rDDN.options) {
            if (d.text == rName) return true;
        }
        return false;
    }

    void TruncateRegionNameInputFields() {
        InputTreeNumber.text = "";
        InputLeafNumber.text = "";
    }

    void OnRegionUpdatedEvt(RegionUpdatedEvt args) {
        Region r = args.newRegion;

        if (!Has_rDDN(r._rName))
            rDDNAddByText(r._rName);

        Region[] rset = new Region[1];
        rset[0] = r;
        Render(rset);

        sDDN.value = 0;
        sDDN.RefreshShownValue();

        rDDNSelect(r._rName);

        BlockScreen(false);
    }

    void rDDNSelect(string sKey) {
        int idx = -1;
        foreach(Dropdown.OptionData d in rDDN.options)
        {
            if (d.text == sKey) {
                idx = rDDN.options.IndexOf(d);
            }
        }
        if (idx > -1) {
            rDDN.value = idx;
            rDDN.RefreshShownValue();
        }
    }

    void rDDNAddByText(string rName) {
        Dropdown.OptionData d = new Dropdown.OptionData();
        d.text = rName;
        rDDN.options.Add(d);
    }

    public void ToggleSegmentsOnBut(string sName) {
        // turn requested segment off, all others on
        _sRenderUI.ToggleSUI(_regionProvider.GetDraft()._rName, sName, false);
        
        string[] sNames = { SEGNAME.A1, SEGNAME.A2, SEGNAME.B1, SEGNAME.B2, SEGNAME.C1, SEGNAME.C2, SEGNAME.D1, SEGNAME.D2 };
        foreach (string sn in sNames) {
            if(sn != sName && !_regionProvider.ZeroDraftSegment(sn))
                _sRenderUI.ToggleSUI(_regionProvider.GetDraft()._rName, sn, true);
        }
    }

    public void Tmp__ReInitSignal(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<RegionUpdatedEvt>(OnRegionUpdatedEvt);
    }
}
