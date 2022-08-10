using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ManewryMorskieRazor
{
    public class BootstrapInterop : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public BootstrapInterop(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/ManewryMorskieRazor/BootstrapInterop.js").AsTask());
        }

        public async ValueTask ToogleModal(string modalId, bool? show = null)
        {
            var module = await moduleTask.Value;

            if(show.HasValue)
            {
                if (show.Value)
                    await module.InvokeVoidAsync("showModal", modalId);
                else
                    await module.InvokeVoidAsync("hideModal", modalId);
            }   
            else
            {
                await module.InvokeVoidAsync("toggleModal", modalId);
            }
        }

        public async ValueTask ShowDropdown(ElementReference elemenet) => await InvokeAsync("showDropdown", elemenet);

        public async ValueTask HideDropdown(ElementReference elemenet) => await InvokeAsync("hideDropdown", elemenet);

        public async ValueTask UpdateDropdown(ElementReference elemenet) => await InvokeAsync("updateDropdown", elemenet);

        public async ValueTask ToggleDropdown(ElementReference elemenet) => await InvokeAsync("updateDropdown", elemenet);

        private async ValueTask InvokeAsync<T>(string methodId, T arg)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync(methodId, arg);
        }

        public async ValueTask DisposeAsync()
        {
            if (moduleTask.IsValueCreated)
            {
                var module = await moduleTask.Value;
                await module.DisposeAsync();
            }
        }
    }
}