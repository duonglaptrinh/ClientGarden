const NUMBER_FRIENDS = 5
const ICE_UPDATE_DELAY = 3000
const ICE_CONFIGURATION = {
    iceServers: [
        { urls: 'stun:stun.services.mozilla.com' },
        { urls: 'stun:stun.l.google.com:19302' },
        {
            urls: 'turn:localhost:3478',
            username: 'vrgDev',
            credential: 'mirabo@2050',
        },
    ],
}
