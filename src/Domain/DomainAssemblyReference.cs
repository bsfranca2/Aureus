using System.Reflection;

namespace Aureus.Domain;

public static class DomainAssemblyReference
{
    public static readonly Assembly Assembly = typeof(DomainAssemblyReference).Assembly;
}