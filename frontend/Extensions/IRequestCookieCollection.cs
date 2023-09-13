using Eburgan.Frontend.Models;

namespace Eburgan.Frontend.Extensions;

public static class RequestCookieCollection
{
    public static Guid GetCurrentBasketId(this IRequestCookieCollection cookies, Settings settings)
    {
        Guid.TryParse(cookies[settings.BasketIdCookieName], out Guid basketId);
        return basketId;
    }
}
