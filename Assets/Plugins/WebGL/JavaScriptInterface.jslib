mergeInto(LibraryManager.library,
{
    OpenUrlWithData : function (url, data)
    {
        openSplashScreen(Pointer_stringify(url), Pointer_stringify(data));
    },

    NativeMessage : function (message, data)
    {
        nativeMessage(Pointer_stringify(message), Pointer_stringify(data));
    }
});