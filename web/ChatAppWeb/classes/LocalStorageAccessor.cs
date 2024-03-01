using Microsoft.JSInterop;

namespace ChatAppWeb.classes;
public class LocalStorageAccessor : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private Lazy<IJSObjectReference> _accessorJsRef = new();

    public LocalStorageAccessor(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task WaitForReference()
    {
        if (_accessorJsRef.IsValueCreated is false)
        {
            _accessorJsRef = new(await _jsRuntime.InvokeAsync<IJSObjectReference>("import", "/js/LocalStorageAccessor.js"));
        }
    }
    public async ValueTask DisposeAsync()
    {
        if (_accessorJsRef.IsValueCreated)
        {
            await _accessorJsRef.Value.DisposeAsync();
        }
    }

    //public async Task<T> GetValueAsync<T>(string key)
    //{
    //    await WaitForReference();
    //    var result = await _accessorJsRef.Value.InvokeAsync<T>("get", key);
    //    return result;
    //}

    public async Task<T> GetValueAsync<T>(string key)
    {
        await WaitForReference();
        T result;
        try
        {
            // Retrieve the value from local storage
            result = await _accessorJsRef.Value.InvokeAsync<T>("get", key);
            return result;

        }
        catch (Exception ex)
        {
            if (typeof(T) == typeof(Guid))
            {
                return (T)(object)Guid.Empty;
            }
            else
                return default(T);

        }

        // Check if the value is null and handle accordingly

    }


    public async Task SetValueAsync<T>(string key, T value)
    {
        await WaitForReference();
        await _accessorJsRef.Value.InvokeVoidAsync("set", key, value);
    }

    public async Task Clear()
    {
        await WaitForReference();
        await _accessorJsRef.Value.InvokeVoidAsync("clear");
    }

    public async Task RemoveAsync(string key)
    {
        await WaitForReference();
        await _accessorJsRef.Value.InvokeVoidAsync("remove", key);
    }
}