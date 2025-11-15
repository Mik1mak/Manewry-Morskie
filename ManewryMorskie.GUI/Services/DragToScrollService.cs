using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskie.GUI
{
    public class DragToScrollService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

        public DragToScrollService(IJSRuntime jsRuntime)
        {
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/ManewryMorskie.GUI/DragToScroll.js").AsTask());
        }

        public async ValueTask RegisterEvent(ElementReference element)
        {
            var module = await moduleTask.Value;
            await module.InvokeVoidAsync("RegisterDragToScroll", element);
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
