//console.log("background.js");

let browserInfo = [];
let userActionInfo = [];

chrome.runtime.onMessage.addListener(function(message, sender1, sendResponse){
    //sendResponse('Hello from background.');

    const { sender, data } = message;
    switch(sender) {
        case 'content':
            userActionInfo.push(...data);
            sendResponse("content received")
            break;
        case 'browser': 
            browserInfo.push(...data);
            sendResponse("browser received")
            break;
        case 'exportall': 
            sendResponse({
                data: {
                    browserInfo,
                    userActionInfo,
                },
                cmd: 'exportall',
            })
            sendMsg();
            break;
        case 'exportlatest': 
            sendResponse({
                data: {
                    browserInfo,
                    userActionInfo,
                },
                cmd: 'exportall',
            })
            sendMsg();
            break;
        default: 
            break;
    }
});

function sendMsg() {
    var host_name = "127.0.0.1";
    var port = 40411;

    connectToNative();

    // userActionInfo browserInfo就是要传送的数据
    sendNativeMessage(JSON.stringify({
        userActionInfo, 
        browserInfo
    }));

    
}

function connectToNative(message) {
    console.log('Connecting to native host: ' + host_name);
    port = chrome.runtime.connectNative(host_name);
    port.onMessage.addListener(onNativeMessage);
    port.onDisconnect.addListener(onDisconnected);
    sendNativeMessage(message);
}

function sendNativeMessage(msg) {
    message = msg;
    console.log('Sending message to native app: ' + JSON.stringify(message));
    port.postMessage(message);
    console.log('Sent message to native app: ' + msg);
}

function onNativeMessage(message) {
    console.log('recieved message from native app: ' + JSON.stringify(message));
}

function onDisconnected() {
    console.log(chrome.runtime.lastError);
    console.log('disconnected from native app.');
    port = null;
}



