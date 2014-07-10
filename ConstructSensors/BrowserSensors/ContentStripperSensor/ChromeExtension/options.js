function add_constants() {
    localStorage["protocol"] = "http";
    localStorage["windows_SensorHost_ID"] = "EDA0FF3E-108B-45D5-BF58-F362FABF2EFE";
    localStorage["dataTypeSource_ID"] = "CCB6CCA4-06F6-4ED4-8356-CB9585E5DCBF";
}

function save_options() {
    var enteredConstructURIField = document.getElementById("constructURI");
    var enteredConstructURI = enteredConstructURIField.value;
    localStorage["construct_URI"] = enteredConstructURI;

    var enteredSourceIDField = document.getElementById("sourceIDGUID");
    var enteredSourceID = enteredSourceIDField.value;
    localStorage["source_ID_GUID"] = enteredSourceID;

    var enteredMachineNameField = document.getElementById("machineName");
    var enteredMachineName = enteredMachineNameField.value;
    localStorage["machine_Name"] = enteredMachineName;

    var enteredSensorHostIDField = document.getElementById("sensorHostID");
    var enteredSensorHostID = enteredSensorHostIDField.value;
    localStorage["sensorHost_ID_GUID"] = enteredSensorHostID;

    // Update status to let user know options were saved.
    var status = document.getElementById("status");
    status.innerHTML = "Options Saved.";
    setTimeout(function () {
        status.innerHTML = "";
    }, 750);
    var installButton = document.getElementById("installSensor");
    add_constants();
    installButton.disabled = false;
}

// Restores select box state to saved value from localStorage.
function restore_options() {
    var storedConstructURI = localStorage["construct_URI"];
    var storedSourceIDGUID = localStorage["source_ID_GUID"];
    var storedMachineName = localStorage["machine_Name"];
    var storedSensorHostIDGUID = localStorage["sensorHost_ID_GUID"];
    console.log(storedConstructURI);
    if (!storedConstructURI) {
    }
    else {
        document.getElementById("constructURI").value = storedConstructURI;
    }
    if (!storedSourceIDGUID) {
    }
    else {
        document.getElementById("sourceIDGUID").value = storedSourceIDGUID;
    }
    if (!storedMachineName) {
    }
    else {
        document.getElementById("machineName").value = storedMachineName;
    }
    if (!storedSensorHostIDGUID) {
    }
    else {
        document.getElementById("sensorHostID").value = storedSensorHostIDGUID;
    }
}

function MyRequest() {
        var xmlhttp = new XMLHttpRequest();

        xmlhttp.open('POST', localStorage["construct_URI"], false); xmlhttp.setRequestHeader('SOAPAction', 'http://tempuri.org/ITransponder/AddObject');

        xmlhttp.setRequestHeader('Content-Type', 'text/xml');
        var data = '';

        data += '<s:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/">'; data += ' <s:Body>';

        data += ' <AddObject xmlns="http://tempuri.org/">';

        data += ' <jsonObject>' + '{\"Name\":\"Test\",\"Args\":{' +
        '\"Protocol\":\"' + localStorage["protocol"] + '\",' +
        '\"HostName\":\"' + localStorage["machine_Name"] + '\",' +
        '\"HostID\":\"' + localStorage["sensorHost_ID_GUID"] + '\",' +
        '\"SensorHostTypeID\":\"' + localStorage["windows_SensorHost_ID"] + '\",' +
        '\"DataTypeSourceID\":\"' + localStorage["dataTypeSource_ID"] + '\",' +
        '\"SourceID\": \"' + localStorage["dataTypeSource_ID"] + '\"' +
        '},' +
        '\"DataTypeSourceID\":\"'+ localStorage["dataTypeSource_ID"] +'\",' +
        '\"BrokerID\":\"' + localStorage["source_ID_GUID"] + '\",' +
        '\"TimeStamp\":\"\\/Date(1349907898159-0500)\\/\",\"Latitude\":0.0,\"Longitude\":0.0}' + ' </jsonObject>';

        data += ' </AddObject>';

        data += ' </s:Body>';

        data += '</s:Envelope>'; xmlhttp.send(data);

        alert("status = " + xmlhttp.status); alert("status text = " + xmlhttp.statusText);

        alert(xmlhttp.responseText);
}


$(function () {
    document.querySelector("#save").addEventListener('click', save_options);
    document.querySelector("#installSensor").addEventListener('click', MyRequest);
    restore_options();
});