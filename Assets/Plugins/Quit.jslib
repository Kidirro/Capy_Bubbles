 mergeInto(LibraryManager.library, {
 

  Quit: function () {
    console.log("Quit pressed  (JS)");

    var userAgent = navigator.userAgent || navigator.vendor || window.opera;

    if (/android/i.test(userAgent)) {
        app.quitHandler("exit");
    }

    if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
        window.webkit.messageHandlers.quitHandler.postMessage("exit");
    }

    console.log("Quit sended  (JS)");
  },

    GetTokenFromParameters: function () {
        console.log("Unity вызвал GetTokenFromParameters");

        function getCookie(name) {
            const matches = document.cookie.match(new RegExp(
                "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
            ));
            return matches ? decodeURIComponent(matches[1]) : undefined;
        }

        const token = getCookie('authToken');
        console.log('Token from cookie:', token);
        
        
            const lengthBytes = lengthBytesUTF8(token) + 1;


    	const stringOnWasmHeap = unityInstance.Module.asm.malloc(lengthBytes);


    	stringToUTF8(token, stringOnWasmHeap, lengthBytes);


    	return stringOnWasmHeap;
    },
    
  GetToken: function () {
    console.log("GetToken called  (JS)");

    var userAgent = navigator.userAgent || navigator.vendor || window.opera;

    if (/android/i.test(userAgent)) {
        app.getTokenHandler("GetToken");
    }

    if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
        window.webkit.messageHandlers.getTokenHandler.postMessage("GetToken");
    }

    console.log("GetToken sended  (JS)");
  },

  
  GetQuery: function(){
    const queryString = window.location.search;
    const urlParams = new URLSearchParams(queryString);
    let query = urlParams.get('game_version');
    console.log("Query game_version param "+ query);
    if(query == null)
    {
    	console.log("Query is null");
    	query = "";
    }
    window.unityInstance.SendMessage('MegafonShopTJS', 'IsTest', query);
  },
  
   
  
  CallConfirmPurchase: function(str) {
    console.log("Call confirm purchase (JS)");

    console.log(UTF8ToString(str));

    var userAgent = navigator.userAgent || navigator.vendor || window.opera;

    if (/android/i.test(userAgent)) {
        app.confirmPurchaseHandler(UTF8ToString(str));
    }

    if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
        window.webkit.messageHandlers.confirmPurchaseHandler.postMessage(UTF8ToString(str));
    }

    console.log("Call confirm purchase sended (JS)");
  },
  
  


  CallConfirmPurchaseNew: function(str) {
    console.log("Call confirm purchase new (JS)");

    console.log(UTF8ToString(str));

    var userAgent = navigator.userAgent || navigator.vendor || window.opera;

    if (/android/i.test(userAgent)) {
        app.purchaseAlertHandler(UTF8ToString(str));
    }

    if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
        window.webkit.messageHandlers.purchaseAlertHandler.postMessage(UTF8ToString(str));
    }

    console.log("Call confirm purchase new sended (JS)");
  },
  CallConfirmVip: function(str) {
    console.log("Call confirm subscription (JS)");

    console.log(UTF8ToString(str));

    var userAgent = navigator.userAgent || navigator.vendor || window.opera;

    if (/android/i.test(userAgent)) {
        app.subscriptionAlertTurnOnHandler(UTF8ToString(str));
    }

    if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
        window.webkit.messageHandlers.subscriptionAlertTurnOnHandler.postMessage(UTF8ToString(str));
    }

    console.log("Call confirm subscription sended (JS)");
  },  
  CallCanselVip: function(str) {
    console.log("Call cansel subscription (JS)");

    console.log(UTF8ToString(str));

    var userAgent = navigator.userAgent || navigator.vendor || window.opera;

    if (/android/i.test(userAgent)) {
        app.subscriptionAlertTurnOffHandler(UTF8ToString(str));
    }

    if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
        window.webkit.messageHandlers.subscriptionAlertTurnOffHandler.postMessage(UTF8ToString(str));
    }

    console.log("Call cansel subscription sended (JS)");
  },
  
  
  OpenURLInSameTab: function (url) {
		window.open(UTF8ToString(url), "_self");
	},
	
	PauseSound:function () {
window.pauseAllAudio();
  },
	
	ResumeSound:function () {
   window.resumeAllAudio(); 
  },
	
  OpenLink: function(str) {
    console.log("Try open link");
    console.log(UTF8ToString(str));

    url = UTF8ToString(str);
    window.open(url,'_blank');
  }
});
