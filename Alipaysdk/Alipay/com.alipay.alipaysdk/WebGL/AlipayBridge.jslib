mergeInto(LibraryManager.library, {
    unityCallJs: function(eventId, paramJson) {
        if (typeof UnityJsBridge === 'object') {
            console.log("platform Android");
            UnityJsBridge.handleMsgFromUnity(UTF8ToString(eventId), UTF8ToString(paramJson));
        } else if (window.webkit && typeof window.webkit.messageHandlers !== 'undefined') {
            console.log("platform iOS");
            if (typeof window.webkit.messageHandlers.handleMsgFromUnity === 'object') {
                console.log("unity send message to native: " + UTF8ToString(eventId));
                window.webkit.messageHandlers.handleMsgFromUnity.postMessage({ "apiName": UTF8ToString(eventId), "apiParam": UTF8ToString(paramJson) });
            } else {
                console.error("unity send message failed");
            }
        } else {
            console.error('platform not support');
        }
    }
});