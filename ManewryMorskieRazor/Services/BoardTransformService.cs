using ManewryMorskieRazor.Util;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public class BoardTransformService : IDisposable
    {
        public const int MAX_ZOOM = 210;
        public const int MIN_ZOOM = 40;

        private readonly IJSRuntime js;
        private DotNetObjectReference<BoardTransformService>? thisService;

        public BoardTransformations CurrentState { get; private set; } = new();

        [JSInvokable]
        public Task<BoardTransformations> GetCurrentState() => Task.FromResult(CurrentState);

        public event Func<BoardTransformations, Task>? TransformationChanged;


        public BoardTransformService(IJSRuntime jsRuntime)
        {
            js = jsRuntime;
        }

        public async ValueTask InitializeAsync()
        {
            if (thisService is not null)
                return;

            IJSObjectReference module = await js.InvokeAsync<IJSObjectReference>("import", "./_content/ManewryMorskieRazor/ScrollEventListener.js").AsTask();
            thisService = DotNetObjectReference.Create(this);
            await module.InvokeVoidAsync("registerScrollEventListener", thisService);
        }

        public async Task SetOrientation(bool horizontal)
        {
            await ChangeState(CurrentState with { IsHorizontal = horizontal });
        }

        public async Task Zoom(int newZoom)
        {
            await ChangeState(CurrentState with { Zoom = newZoom });
        }

        [JSInvokable]
        public async Task ChangeZoom(int step)
        {
            await ChangeState(CurrentState with { Zoom = CurrentState.Zoom + step });
        }

        [JSInvokable]
        public async Task ChangeState(BoardTransformations newState)
        {
            if (newState.Zoom < MIN_ZOOM)
                newState = newState with { Zoom = MIN_ZOOM };
            else if (newState.Zoom > MAX_ZOOM)
                newState = newState with { Zoom = MAX_ZOOM };

            CurrentState = newState;

            if (TransformationChanged is not null)
                await TransformationChanged.Invoke(CurrentState);
        }

        public void Dispose()
        {
            thisService?.Dispose();
        }
    }
}
