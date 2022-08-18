using BlazorStrap;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManewryMorskieRazor
{
    public static class Extensions
    {
        public static IServiceCollection AddManewryMorskieGame(this IServiceCollection services)
        {
            services.AddBlazorStrap();
            services.AddScoped<BoardTransformService>();
            services.AddScoped<BootstrapInterop>();
            services.AddScoped<DialogService>();
            services.AddScoped<BoardService>();
            services.AddScoped<UserInterface>();
            services.AddScoped<GameService>();
            services.AddScoped<DragToScrollService>();
            services.AddScoped<PawnAnimatingService>();

            return services;
        }
    }
}
