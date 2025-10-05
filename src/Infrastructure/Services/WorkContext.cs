using Aureus.Domain.Shared.Interfaces;
using Aureus.Domain.Stores;

using Microsoft.AspNetCore.Http;

namespace Aureus.Infrastructure.Services;

public class WorkContext(IHttpContextAccessor httpContextAccessor) : IWorkContext
{
    public StoreId GetStoreId()
    {
        StoreId? storeId = httpContextAccessor.HttpContext?.Items["StoreId"] as StoreId?;
        return storeId ?? throw new NullReferenceException(nameof(storeId));
    }
}