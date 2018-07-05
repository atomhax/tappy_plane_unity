using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

public class WebGLJavaScriptInterface
{
    public static void OpenUrl(string url, JSONObject data)
    {
        Dictionary<string, string> urlParams = data.ToDictionary();
        string urlString = url.Replace("\"", "") + (data.keys.Count > 0 ? "?" : "");
        foreach(KeyValuePair<string, string> kvp in urlParams)
        {
            urlString += (kvp.Key == urlParams.First().Key ? "" : "&") + kvp.Key + "=" + System.Uri.EscapeDataString(kvp.Value);
        }
        SpilLogging.Log("OpenUrl: " + urlString);
        OpenUrl(urlString);
    }

    [DllImport("__Internal")]
    public static extern void OpenUrl(string url);

    /*
    // Example code

    [DllImport("__Internal")]
    public static extern void PrintFloatArray(float[] array, int size);

    [DllImport("__Internal")]
    public static extern int AddNumbers(int x, int y);

    [DllImport("__Internal")]
    public static extern string StringReturnValueFunction();

    [DllImport("__Internal")]
    public static extern void BindWebGLTexture(int texture);
    */
}