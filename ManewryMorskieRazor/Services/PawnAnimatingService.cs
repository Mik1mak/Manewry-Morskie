using CellLib;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public class PawnAnimatingService : IAsyncDisposable
    {
        private readonly Lazy<Task<IJSObjectReference>> moduleTask;
        private readonly BoardTransformService boardTransformService;

        public PawnAnimatingService(IJSRuntime jsRuntime, BoardTransformService boardTransformService)
        {
            this.boardTransformService = boardTransformService;
            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
               "import", "./_content/ManewryMorskieRazor/PawnAnimations.js").AsTask());
        }

        public async ValueTask MovePawn(ElementReference element, IEnumerable<CellLocation> path, int duration)
        {
            var module = await moduleTask.Value;

            List<object> diffPaths = new();

            CellLocation last = path.ElementAt(0);
            foreach (CellLocation l in path.Skip(1))
            {
                if(boardTransformService.CurrentState.IsHorizontal)
                    diffPaths.Add(new { x = l.Row-last.Row, y = l.Column-last.Column });
                else
                    diffPaths.Add(new { x = l.Column-last.Column, y = last.Row-l.Row });
                last = l;
            }

            await module.InvokeVoidAsync("Animate", element, diffPaths, duration);
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
