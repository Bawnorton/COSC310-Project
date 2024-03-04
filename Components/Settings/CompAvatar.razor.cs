using Bamboozlers.Classes.AppDbContext;
using Blazorise;
using Microsoft.AspNetCore.Components;

namespace Bamboozlers.Components.Settings;

public partial class CompAvatar : CompProfile
{
    private async Task OnFileUpload(FileUploadEventArgs e)
    {
        if (User == null)
        {
            await StatusCallback.InvokeAsync(StatusCallbackArgs.BasicStatusArgs);
            return;
        }
        MemoryStream result;
        try
        {
            using (result = new MemoryStream())
            {
                await e.File.OpenReadStream(long.MaxValue).CopyToAsync(result);
            }
            var rawImage = new ArraySegment<byte>();
            
            var success = result.TryGetBuffer(out rawImage);
            if (!success)
            {
                await StatusCallback.InvokeAsync(new StatusCallbackArgs(
                    Color.Danger,
                    true,
                    "Could not change avatar.",
                    "An error was encountered while processing uploaded avatar."
                    ));
                return;
            }
            /* I'm assuming we're going to be changing to base64 strings at some point, makes it easier to use in image elements
             
            string b64 = Convert.ToBase64String(result.GetBuffer());
            User.Avatar = b64;
             */
            User.Avatar = rawImage.ToArray();
        }
        catch (Exception exc)
        {
            Logger.LogError(exc.Message);
            await StatusCallback.InvokeAsync(new StatusCallbackArgs(
                Color.Danger,
                true,
                "Could not change avatar.",
                "An error was encountered while changing avatar."
                ));
        }
        finally
        {
            StateHasChanged();
        }
    }

    // TODO: Remove this once database has Base64 strings for avatar
    private string GetBase64Avatar()
    {
        return $"data:image/png;base64,{(User?.Avatar == null ? "" : Convert.ToBase64String(User.Avatar))}";
    }
}