function SendString(json) {
    console.log(json);
    window.unityInstance.SendMessage('JavaScriptHandler', 'GetString', json);
}


function SendConfirmedStatus(isConfirmed, message) {
    console.log("SendConfirmedStatus called");

    window.unityInstance.SendMessage('MegafonShopTJS', 'SlotConfirmed', '{"status": ' + isConfirmed + ', "data": "' + message + '"}');

    console.log("SendConfirmedStatus sended");
}

function SendToken(token)
{
	window.unityInstance.SendMessage('Model', 'Token', token);
}

function SendPurchaseData(isConfirmed, message) {
    console.log("SendConfirmedStatus called");

    window.unityInstance.SendMessage('MegafonShopTJS', 'SlotConfirmed', '{"status": ' + isConfirmed + ', "data": "' + message + '"}');

    console.log("SendConfirmedStatus sended");
}

function NeedCloseButton() {
    var link = window.location.href;
    var substringMegafon = 'tj-megafon';
    console.log(link);
    return link.includes(substringMegafon);
}

function GetRandomText(mascot) {
    var text = texts[Math.floor(Math.random() * texts.length)];
    
    return text.replace("MASCOT", mascot);;
}
function pauseAllAudio() {
	
    window.unityInstance.SendMessage('Audio.AudioManager', 'OnApplicationFocus', false);
}

function resumeAllAudio() {

    window.unityInstance.SendMessage('Audio.AudioManager', 'OnApplicationFocus', true);
}

function SendQuit() {
    console.log("Quit called (JS)");

    var userAgent = navigator.userAgent || navigator.vendor || window.opera;

    if (/android/i.test(userAgent)) {
        app.quitHandler("exit");
    }

    if (/iPad|iPhone|iPod/.test(userAgent) && !window.MSStream) {
        window.webkit.messageHandlers.quitHandler.postMessage("exit");
    }

    console.log("Quit sended  (JS)");
}
