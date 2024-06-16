using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorLocalization.Controllers;

//code copied from https://github.com/claudiobernasconi/BlazorLocalization

/// <summary>
/// Provides infrustructure to manipulate with culture settings.
/// </summary>

[Route("[controller]/[action]")]
public class CultureController : Controller
{
    /// <summary>
    /// Sets language in application. Save this option to Cookie.
    /// </summary>
    /// <param name="culture">Selected culture, string is in language code based on ISO 639</param>
    /// <param name="redirectUri">Url of the page of origin</param>
    /// <returns></returns>
    public IActionResult Set(string culture, string redirectUri)
    {
        if (culture != null)
        {
            var requestCulture = new RequestCulture(culture, culture);
            var cookieName = CookieRequestCultureProvider.DefaultCookieName;
            var cookieValue = CookieRequestCultureProvider.MakeCookieValue(requestCulture);

            HttpContext.Response.Cookies.Append(cookieName, cookieValue);
        }

        return LocalRedirect(redirectUri);
    }
}