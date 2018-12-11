using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AppImg : MonoBehaviour, IImgServiceReceiver, IImgProvider, IDisposable
{
    [Inject]
    SignalBus _signalBus;

    [Inject]
    IImgProxy _proxy;

    AppImgDS appImgDS = new AppImgDS();

    void Start()
    {
        _signalBus.Subscribe<ImgUploadPermissionEvt>(OnImgUploadPermission);
        _signalBus.Subscribe<ImgUploadRevokePermissionEvt>(OnImgUploadRevokePermissionEvt);

        _signalBus.Subscribe<QueryTextureEvt>(OnQueryTextureEvt);
        _signalBus.Subscribe<HomeInitEvt>(OnHomeInit);
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<ImgUploadPermissionEvt>(OnImgUploadPermission);
        _signalBus.Unsubscribe<ImgUploadRevokePermissionEvt>(OnImgUploadRevokePermissionEvt);

        _signalBus.Unsubscribe<QueryTextureEvt>(OnQueryTextureEvt);
        _signalBus.Unsubscribe<HomeInitEvt>(OnHomeInit);
    }

    public virtual void LastUrlsCB(string urls) {
        LastUrlsFromJSON urlJson = JsonUtility.FromJson<LastUrlsFromJSON>(urls);
        if (urlJson != null && urlJson.urls != null) {
            int isSingle = 0;
            string LastUploadedImg = "";
            foreach (Url aurl in urlJson.urls) {
                if (!appImgDS._dateToUrlDict.ContainsKey(aurl.date))// => is-a new Img
                {
                    appImgDS._dateToUrlDict.Add(aurl.date, aurl.url);
                    LastUploadedImg = aurl.date;
                    isSingle++;
                }
            }
            // if only 1 new url - this is new uploaded photo, select it
            if (isSingle == 1) PublishUrlsUpdateEvt("", null, appImgDS._dateToUrlDict, LastUploadedImg);
            // otherwise, it is 'init' event, leave selection to user (LastUploadedImg="")
            else PublishUrlsUpdateEvt("", null, appImgDS._dateToUrlDict);
        }
    }

    public virtual void TextureCB(Texture2D urlTexture, string url) {
        if (url == appImgDS._lastReqUrl && urlTexture != null)
        {
            appImgDS._urlTexture = urlTexture;
            PublishUrlsUpdateEvt(url, urlTexture);
        }
    }

    /* IImgProvider implementation */
    public bool CurrentImgUrlBothExist() {
        if (string.IsNullOrEmpty(appImgDS._lastReqUrl) || appImgDS._urlTexture == null)
            return false;
        else
            return true;
    }

    public string GetCurrentUrl() {
        return appImgDS._lastReqUrl;
    }

    public Texture2D GetTexture2D() {
        return appImgDS._urlTexture;
    }

    void ReqLastUrls()
    {
        _proxy.ReqLastUrls();
    }

    void OnQueryTextureEvt(QueryTextureEvt args)
    {
        // current url is requested and texture is available
        if (args.url == appImgDS._lastReqUrl && appImgDS._urlTexture != null)
        {
            // urlsDict = null, so only update texture at the destination
            PublishUrlsUpdateEvt(appImgDS._lastReqUrl, appImgDS._urlTexture);
        }
        else// different url is requested or texture is not available
        {
            appImgDS._lastReqUrl = args.url; // always update to the latest query
            _proxy.ReqTexture(args.url, ImgProxy.TextureCB_CallerID.APP_IMG);
        }
    }

    void OnHomeInit() {
        PublishUrlsUpdateEvt(appImgDS._lastReqUrl, appImgDS._urlTexture, appImgDS._dateToUrlDict);

        if (!(appImgDS._dateToUrlDict.Count > 0))
            ReqLastUrls();
    }

    void PublishUrlsUpdateEvt(string lastReqUrl = "", Texture2D urlTexture = null,
            Dictionary<string, string> urlsDict = null, string LastUploadedImg = "") {
        UrlsUpdateEvt evt = new UrlsUpdateEvt();
        evt.urlsDict = urlsDict;
        evt.lastReqUrl = lastReqUrl;
        evt.urlTexture = urlTexture;
        evt.LastUploadedImg = LastUploadedImg;
        _signalBus.Fire(evt);
    }

    void OnImgUploadPermission() {
        _proxy.PrepareImgUpload(true);
    }

    void OnImgUploadRevokePermissionEvt() {
        _proxy.PrepareImgUpload(false);
    }

    public Dictionary<string, string> GetUrlDict() {
        return appImgDS._dateToUrlDict;
    }

    public void Tmp__ReInitSignal(SignalBus signalBus)
    {
        _signalBus = signalBus;
        _signalBus.Subscribe<ImgUploadPermissionEvt>(OnImgUploadPermission);
        _signalBus.Subscribe<ImgUploadRevokePermissionEvt>(OnImgUploadRevokePermissionEvt);

        _signalBus.Subscribe<QueryTextureEvt>(OnQueryTextureEvt);
        _signalBus.Subscribe<HomeInitEvt>(OnHomeInit);
    }

    public void ReNewAppImgDS() {
        appImgDS = new AppImgDS();
    }

    public void ReqDeleteImgByDateKey(string imgDateKey) {
        _proxy.ReqDeleteImgByDateKey(imgDateKey);
    }

    public void DeleteFromAppImgDSByKey(string imgDateKey) {
        if (appImgDS._dateToUrlDict.ContainsKey(imgDateKey))
        {
            appImgDS._dateToUrlDict.Remove(imgDateKey);
            appImgDS._lastReqUrl = "";
            appImgDS._urlTexture = null;
            if (!(appImgDS._dateToUrlDict.Count > 0))
            {
                ReqLastUrls();
            }
        }
        else// TODO: remove this debug section
        {
            foreach (string s in appImgDS._dateToUrlDict.Keys) {
                Debug.Log(s);
            }
        }
    }
}

public class AppImgDS {
    public string _lastReqUrl = "";
    public Texture2D _urlTexture = null;
    // key: unix timestamp--ms, value: url
    public Dictionary<string, string> _dateToUrlDict = new Dictionary<string, string>();
}

[Serializable]
public class LastUrlsFromJSON
{
    public Url[] urls;
}

[Serializable]
public class Url
{
    public string url;
    public string date;
}