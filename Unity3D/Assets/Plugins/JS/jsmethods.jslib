mergeInto(LibraryManager.library, {

  JSUnitSave: function (x1, y1, x2, y2, uMagnitude, uUrl){
    uUrl = Pointer_stringify(uUrl);
    if(window.unitSave)  window.unitSave(x1, y1, x2, y2, uMagnitude, uUrl);
  },

  JS_ReqDeleteRegionByName: function(rName){
    rName = Pointer_stringify(rName);
    if(window.reqDeleteRegionByName) window.reqDeleteRegionByName(rName);
  },

  JS_ReqDeleteImgByDateKey: function(imgDateKey){
    imgDateKey = Pointer_stringify(imgDateKey);
    if(window.reqDeleteImgByDateKey) window.reqDeleteImgByDateKey(imgDateKey);
  },

  JS_ExportRegionsToCSV: function(){
    if(window.exportRegionsToCSV) window.exportRegionsToCSV();
  },

  JS_ReqRegionsByUrl: function(url){
    url = Pointer_stringify(url);
    if(window.reqRegionsByUrl) window.reqRegionsByUrl(url);
  },

  JS_ReqSaveUnitJSON: function(ujson){
    ujson = Pointer_stringify(ujson);
    if(window.reqSaveUnitJSON) window.reqSaveUnitJSON(ujson);
  },

  JS_ReqLastUrls: function(){
    if(window.latestXImg) window.latestXImg();
  },

  JS_ReqLastUnitDS: function(){
    if(window.latestUnitQuery) window.latestUnitQuery();
  },

  JS_ReqSaveUnitDS: function(x1, y1, x2, y2, magnitude, url){
    url = Pointer_stringify(url);
    if(window.reqSaveUnitDS) window.reqSaveUnitDS(x1, y1, x2, y2, magnitude, url);
  },

  JS_ReqSaveNewRegionDS: function(rName, jsonRegion){
    rName = Pointer_stringify(rName);
    jsonRegion = Pointer_stringify(jsonRegion);
    if(window.reqSaveNewRegionDS) window.reqSaveNewRegionDS(rName, jsonRegion);
  },

  JSLogin: function (username, password) {
    if(window.login){
	username = Pointer_stringify(username);
	password = Pointer_stringify(password);
	window.login(username, password);
    }
  },

  JSLogout: function(){
    if(window.logout) window.logout();
  },

  JS_PrepareImgUpload: function(){
    if(window.showFileUpDialog) window.showFileUpDialog();
  },

  JS_UnPrepareImgUpload: function(){
    if(window.hideFileUpDialog) window.hideFileUpDialog();
  }

});