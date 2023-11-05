let closeBtn = document.getElementById("close-button");
let featureBtn = document.getElementById("feature");
let colorOn = "#28A398";
let colorOff = "#888888";

closeBtn.addEventListener("click", function () {
    document.getElementById("box-chat").style.display = 'none';
    document.getElementById("feature").style.display = 'inline-block';
});

featureBtn.addEventListener("click", function () {
    document.getElementById("box-chat").style.display = 'inline-block';
    document.getElementById("feature").style.display = 'none';
});

function toogleFeatureBtn(bool) {
    if (!bool) {
        document.getElementById("feature").style.display = 'none';
    } else {
        document.getElementById("feature").style.display = 'inline-block';
    }
}

function toogleBoxChat(bool) {
    if (!bool) {
        document.getElementById("box-chat").style.display = 'none';
    } else {
        document.getElementById("box-chat").style.display = 'inline-block';
    }
}

function setUIButtonCamera(id, bool) {
    setButtonAction("button-video-" + id, bool);
    for (var i = 0; i < arrStatusCamera.length; ++i) {
        if (arrStatusCamera[i].id == id) {
            arrStatusCamera[i].status = bool;
            break;
        }
    }
}
function setUIButtonMicrophone(id, bool) {
    setButtonAction("button-voice-" + id, bool);
    for (var i = 0; i < arrStatusMicrophone.length; ++i) {
        if (arrStatusMicrophone[i].id == id) {
            arrStatusMicrophone[i].status = bool;
            break;
        }
    }
}
function setButtonAction(name_class, bool) {
    let buttonVoice = document.getElementsByClassName(name_class);
    var btn = buttonVoice[0];
    btn.style.backgroundColor = bool ? colorOn : colorOff;
    // console.log(btn);
    var mic = btn.children[0];
    mic.style.display = GetStyle(bool);
    // console.log(mic);
    var mic2 = btn.children[1];
    mic2.style.display = GetStyle(!bool);
    // console.log(mic2);
}

function GetStyle(bool) {
    return bool ? "inline-block" : "none";
}

function getStatusButtonCamera(id) {
    for (var i = 0; i < arrStatusCamera.length; ++i) {
        if (arrStatusCamera[i].id == id)
            return arrStatusCamera[i].status;
    }
}
function getStatusButtonMicrophone(id) {
    for (var i = 0; i < arrStatusMicrophone.length; ++i) {
        if (arrStatusMicrophone[i].id == id)
            return arrStatusMicrophone[i].status;
    }
}
function removeMicrophoneStatusById() {
    for (var i = 0; i < arrStatusMicrophone.length; ++i) {
        if (arrStatusMicrophone[i].id == id) {
            arrStatusMicrophone.remove(i);
            break;
        }
    }
}
function removeCameraStatusById() {
    for (var i = 0; i < arrStatusCamera.length; ++i) {
        if (arrStatusCamera[i].id == id) {
            arrStatusCamera.remove(i);
            break;
        }
    }
}

function setColorVideo(id, color) {
    //color = "#234327"
    let item = document.getElementById("video-" + id);
    if (item) {
        item.style.border = "3px solid " + color;
    }
    else {
        console.log("Cannot find item id " + id);
    }
}

function setQuestTag(id) {
    let item = document.getElementById("video-" + id);
    item.style.backgroundImage = "url('TemplateData/Icon_VR.png')";
    item.style.backgroundSize = "50%";
    item.style.backgroundRepeat = "no-repeat";
    item.style.backgroundPosition = "center";
}

function fullScreenDocument() {
    var elem = document.getElementById('unity-container');
    if (document.fullscreenElement) {
        document.exitFullscreen();
    } else {
        elem.webkitRequestFullscreen();
    }
}