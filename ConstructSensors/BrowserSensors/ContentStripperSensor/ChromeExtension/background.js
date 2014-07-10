chrome.extension.onRequest.addListener(function (request, sender, sendResponse) {
    if (request.method == "getConstructServerUri")
        sendResponse({ status: localStorage['construct_URI'] });
    if (request.method == "getSourceID")
        sendResponse({ status: localStorage['source_ID_GUID'] });
    else
        sendResponse({});
});