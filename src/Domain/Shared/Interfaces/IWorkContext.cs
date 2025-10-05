using Aureus.Domain.Stores;

namespace Aureus.Domain.Shared.Interfaces;

public interface IWorkContext
{
    StoreId GetStoreId();
}