let contentVideo = document.getElementById('content-video');
let boxChat = document.getElementById('box-chat');
let arrStatusMicrophone = [];
let arrStatusCamera = [];

function makeid(length) {
    let result = '';
    let characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let charactersLength = characters.length;
    for (let i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
}

function getNumberItemFriendVideo(id) {
    let arrayFriendVideos = document.getElementsByClassName("friend-box");
    let n = 0;
    for (let i = 0; i < arrayFriendVideos.length; i++) {
        if (arrayFriendVideos[i].hasAttribute("id")) {
            if (arrayFriendVideos[i].getAttribute("id") === id) {
                n = i;
            }
        }
    }
    return n + 1;
}

function addItemVideo(id, name, statusMic, statusCamera) {

    arrStatusMicrophone.push({ id: id, status: statusMic });
    arrStatusCamera.push({ id: id, status: statusCamera });

    let elementHtml = `
        <div class="friend-box" id="${id}">
        <video class="video" id="video-${id}" autoplay></video>

            <div class="info">
                <div class="name" style="display:flex;justify-content:flex-start;padding: 1px 20px 1px 2px;">
                    <button onclick="ReloadUpStream()" style="border:none;background:none;"><img src="TemplateData/reload.png"></button>
                    <span class="name-user" style="flex: 1;text-align: center;">
                        ${name}
                    </span>
                </div>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <div class="button-group">
                    <span class="button-voice button-voice-${id}"  onclick="toggleVoice('${id}')">
                        <i class="fa fa-microphone" aria-hidden="true"></i>
                        <i class="fa fa-microphone-slash" aria-hidden="true"></i>
                    </span>
                    <span class="button-video button-video-${id}" onclick="toggleVideo('${id}')">
                        <i class="fa fa-video-camera" aria-hidden="true"></i>
                        <i class="fa  fa-eye-slash" aria-hidden="true"></i>
                    </span>
                </div>
            </div>
        </div>
    `;

    contentVideo.insertAdjacentHTML('afterbegin', elementHtml);
    setUIButtonCamera(id, statusCamera);
    setUIButtonMicrophone(id, statusMic);
}

function addOtherVideo(id, name, statusMic, statusCamera) {

    let elementHtml = `
        <div class="friend-box" id="${id}">
        <video class="video" id="video-${id}" autoplay></video>

            <div class="info">
                <div class="name">
                    <span class="name-user">
                        ${name}
                    </span>
                </div>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <div class="button-group">
                    <span class="button-voice button-voice-${id}"  onclick="toggleOtherVoice('${id}')">
                        <i class="fa fa-microphone" aria-hidden="true"></i>
                        <i class="fa fa-microphone-slash" aria-hidden="true"></i>
                    </span>
                    <span class="button-video button-video-${id}" onclick="toggleOtherVideo('${id}')">
                        <i class="fa fa-video-camera" aria-hidden="true"></i>
                        <i class="fa  fa-eye-slash" aria-hidden="true"></i>
                    </span>
                </div>
            </div>
        </div>
    `;

    contentVideo.insertAdjacentHTML('beforeend', elementHtml);
}

function removeItem(id) {
    let element = document.getElementById(id);
    element.remove();
}

// addItem("manh", true, false)
// removeItem("friend-box-uoOWZ")

function removeItemById(id) {
    let item = contentVideo.getElementById(id)
    if (item) {
        item.remove()
        removeCameraStatusById(id);
        removeMicrophoneStatusById(id);
    }
}

function getIDRoomFromUrl() {

    var url = window.location.href;//"https://acountID.gld-lab.link/?world=worldID";
    var worldID = new URL(url).searchParams.get("world");
    console.log(worldID);
    if (worldID == null) worldID = "1";
    console.log("worldID ----------------- " + worldID);
    MyGameInstance.SendMessage('WebGLAdapter', 'SetIDRoomOnCallFromWebGL', worldID);
}

function setHoverBoxChat() {
    boxChat.addEventListener('mouseenter', function (event) {
        MyGameInstance.SendMessage('WebGLAdapter', 'SetOverlayMenuVideoChat', 'true');
    });

    boxChat.addEventListener('mouseleave', function (event) {
        MyGameInstance.SendMessage('WebGLAdapter', 'SetOverlayMenuVideoChat', 'false');
    });
}
function CheckMobileDevice() {
    const userAgent = navigator.userAgent;
    let isMobile = /Android|iPhone|iPad|iPod/i.test(userAgent);
    let isSafariOnIPad = /Macintosh/i.test(userAgent) && 'ontouchend' in document;
    let isCheckMobile = isSafariOnIPad || isMobile;
    MyGameInstance.SendMessage('WebGLAdapter', 'SetIsMobile', isCheckMobile.toString());
}
function StartApp() {
    getIDRoomFromUrl();
    setHoverBoxChat();
    CheckMobileDevice();
}

function DownloadImageByUrl(imageUrl) {
    var currentTimeInMillis = new Date().getTime();
    fetch(imageUrl)
        .then(response => response.blob())
        .then(blob => {
            const url = URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.target = "_blank"; // Open new tab
            a.download = "Vrg_image" + currentTimeInMillis + ".jpg"; //Name default when download image
            a.click();
            URL.revokeObjectURL(url);
        })
        .catch(error => {
            console.error('Error downloading image:', error);
        });
}
