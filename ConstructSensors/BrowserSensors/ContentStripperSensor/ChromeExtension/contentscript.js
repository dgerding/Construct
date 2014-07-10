
var targetConstructServerURIReady = false;
var sourceIDReady = false;

function _stripTags(text) {
    text = text.replace(/<\/?[a-z][a-z0-9]*[^<>]*>/ig, '');
    text = text.replace(/['"&{}\251\175\174\n\t]/g, '');
    return text = text.replace(/\s{2,}/g, ' ');
    //return text.replace(/<(?:.|\n)*?>/gm, '');
}
//;(){}+%:=.$&,-_?!*/
//function _secondStripTags(text) {

//}

var htmlSource = document.getElementsByTagName('html')[0].innerHTML;

var strippedSource = _stripTags(htmlSource);

//var strippedSource = _secondStripTags(stripSource);

console.log(strippedSource);



var targetConstructServerURI;

chrome.extension.sendRequest({ method: "getConstructServerUri" }, function (response) {
    console.log(response.status);
    targetConstructServerURI = response.status;
    targetConstructServerURIReady = true;
    MyRequest();
//xmlhttp.setRequestHeader('SOAPAction', 'http://tempuri.org/ITransponder/AddObject');

});

var datatypeSourceID = "CCB6CCA4-06F6-4ED4-8356-CB9585E5DCBF";
var sourceID;

chrome.extension.sendRequest({ method: "getSourceID" }, function (response) {
    console.log(response.status);
    sourceID = response.status;
    sourceIDReady = true;
    MyRequest();
    //xmlhttp.setRequestHeader('SOAPAction', 'http://tempuri.org/ITransponder/AddObject');
});


function MyRequest() {
    if (targetConstructServerURIReady && sourceIDReady) {
        var xmlhttp = new XMLHttpRequest();

        xmlhttp.open('POST', targetConstructServerURI, false); xmlhttp.setRequestHeader('SOAPAction', 'http://tempuri.org/ITransponder/AddObject');

        xmlhttp.setRequestHeader('Content-Type', 'text/xml');
        var data = '';

        data += '<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">'; data += ' <s:Body>';

        data += ' <AddObject xmlns="http://tempuri.org/">';

        data += ' <jsonObject>' + '{\"Name\":\"Test\",\"Args\":{\"Uri\":\"http://johnny5/f721f879-9f84-412f-ae00-632cfea5a1f3/2339bd10-a933-41cc-b684-34b15d7516e6\", \"HTMLOutput\": \"' + strippedSource + '\"},\"DataTypeSourceID\":\"ae9e3c31-09c8-4835-8e2d-286922adb3f6\",\"BrokerID\":\"70c1a214-05eb-4f58-9328-1cc8ad7d6c66\",\"TimeStamp\":\"\\/Date(1349907898159-0500)\\/\",\"Latitude\":0.0,\"Longitude\":0.0}' + ' </jsonObject>';

        data += ' </AddObject>';

        data += ' </s:Body>';

        data += '</s:Envelope>'; xmlhttp.send(data);

        alert("status = " + xmlhttp.status); alert("status text = " + xmlhttp.statusText);

        alert(xmlhttp.responseText);
    }
    else {
        return;
    }

}





//var Proxy = new serviceProxy("http://localhost/F721F879-9F84-412F-AE00-632CFEA5A1F3/2339bd10-a933-41cc-b684-34b15d7516e6/web/");

//function onPageError() {
//    return;
//}

//Proxy.invoke("AddObject",
//    '\"Name\":\"Test\",\"Args\":{\"Uri\":\"http://johnny5/f721f879-9f84-412f-ae00-632cfea5a1f3/2339bd10-a933-41cc-b684-34b15d7516e6\"},\"DataTypeSourceID\":\"ae9e3c31-09c8-4835-8e2d-286922adb3f6\",\"BrokerID\":\"70c1a214-05eb-4f58-9328-1cc8ad7d6c66\",\"TimeStamp\":\"\\/Date(1349907898159-0500)\\/\",\"Latitude\":0.0,\"Longitude\":0.0',
//    function (result)
//    {
//    return;
//    }, 
//onPageError);