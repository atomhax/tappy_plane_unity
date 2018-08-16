mergeInto(LibraryManager.library,
{
    OpenUrlWithData : function (url, data)
    {
        openSplashScreen(Pointer_stringify(url), Pointer_stringify(data));
    },

    NativeMessage : function (message, data)
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