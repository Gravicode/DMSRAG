using Microsoft.JSInterop;
using static System.Net.Mime.MediaTypeNames;

namespace DMSRAG.Web.Helpers
{

    public class Clipboard
    {
        public static IJSRuntime JSRuntime { set; get; }

        public void Add(IJSRuntime js)
        {
            JSRuntime = js;
        }
        public static async Task SetTextAsync(string Text)
        {
            await JSRuntime.InvokeVoidAsync("clipboardCopy.copyText", Text);
        }
    }
}
