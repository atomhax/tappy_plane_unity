var ApplixirPlugin = {

    $impl: {},

    adStatusCallbackX: function (status) {

        //console.log('Ad Status:', status);       
        var iresult = 0;
        switch (status)       
        {
            case "ad-watched":
                iresult = 1;
            break;
            case "network-error":
                iresult = 2;
            break;
            case "ad-blocker":
                iresult = 3;
            break;
            case "ad-interrupted":
                iresult = 4;
            break;
            case "ads-unavailable":
                iresult = 5;
            break;
            case "ad-fallback":
                iresult = 6;
            break;
            case "cors-error":
                iresult = 7;
            break;
            case "no-zoneId":
                iresult = 8;
            break
            case "ad-started":
                iresult = 9;
            break;
            case "sys-closing": // never block on this status callback
                iresult = 10;
            break;
            default:
                iresult = 0;
            break;
        }

        Runtime.dynCall('vi', window.applixirCallback, [iresult]);
        //console.log('Ad Status done:', status);
    },

    ShowVideo: function (devId, gameId, zoneId, fallback, callback) {

        var local_options = {
            zoneId: zoneId, // the zone ID from the "Games" page
            devId: devId, // your developer ID from the "Account" page
            gameId: gameId, // the ID for this game from the "Games" page
            adStatusCb: _adStatusCallbackX, // optional
            fallback: fallback, // 0|1
            verbosity: 0 // 0..5
        };

        //console.log(local_options);
        window.applixirCallback = callback;
        invokeApplixirVideoUnit(local_options);
    },

    ShowVideo__deps: [
        '$impl',       
        'adStatusCallbackX'
    ]
};

autoAddDeps(ApplixirPlugin, '$impl');
mergeInto(LibraryManager.library, ApplixirPlugin);