using Aureus.Domain.Stores;
using Aureus.Infrastructure.EntityFramework;

using Microsoft.EntityFrameworkCore;

namespace Aureus.WebApi.Middlewares;

public class StoreTenantMiddleware(RequestDelegate next)
{
    private const string StoreIdHeader = "X-Store-Id";

    public async Task InvokeAsync(HttpContext context, DatabaseContext dbContext)
    {
        var storeIdRaw = GetStoreIdFromRequest(context);

        if (storeIdRaw == null)
        {
            context.Response.StatusCode = 404;
            return;
        }

        var storeId = new StoreId(storeIdRaw.Value);
        var exists = await dbContext.Stores.AnyAsync(s => s.Id == storeId);
        if (!exists)
        {
            context.Response.StatusCode = 404;
            return;
        }

        context.Items["StoreId"] = storeId;
        
        await next(context);
    }

    private static int? GetStoreIdFromRequest(HttpContext context)
    {
        // TODO: Improve
        return 1;
        // if (context.Request.Host.Host == "seguro.lojaoculos.localhost")
        //     return "";

        // if (context.Request.Headers.TryGetValue(StoreIdHeader, out var headerValue))
        //     return headerValue;

        // return null;
    }
}