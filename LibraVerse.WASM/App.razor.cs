using System.Reflection;

namespace LibraVerse.WASM;

public partial class App
{
    private readonly List<Assembly> _lazyLoadedAssemblies = [];

    protected override void OnInitialized()
    {
        _lazyLoadedAssemblies.Add(typeof(App).Assembly);
    }

    private bool IsAnonymousRoute(Type pageType)
    {
        return pageType.CustomAttributes.Any(attr =>
            attr.AttributeType == typeof(Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute));
    }
}