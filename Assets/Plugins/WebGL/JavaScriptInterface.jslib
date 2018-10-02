mergeInto(LibraryManager.library,
{
    GetBrowserInfoJS : function ()
    {
        var returnStr = getBrowserInfo();
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    },

    GivePlayerFocusJS : function ()
    {
        givePlayerFocus();
    },

    OpenUrlInNewWindowJS : function (url)
    {
        openUrlInNewWindow(Pointer_stringify(url));
    },

    OpenSplashScreenUrlJS : function (url, data)
    {
        openSplashScreen(Pointer_stringify(url), Pointer_stringify(data));
    },

    NativeMessageJS : function (message, data)
    {
        nativeMessage(Pointer_stringify(message), Pointer_stringify(data));
    },

    GetDeviceIdJS : function ()
    {
        var returnStr = getDeviceId();
        var bufferSize = lengthBytesUTF8(returnStr) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(returnStr, buffer, bufferSize);
        return buffer;
    }
});