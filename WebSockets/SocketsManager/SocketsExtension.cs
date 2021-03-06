using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WebSockets.SocketsManager
{
    public static class SocketsExtension
    {
        public static IServiceCollection AddWebSocketManager(this IServiceCollection service)
        {
            service.AddTransient<ConnectionManager>();
            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(SocketHandler))
                    service.AddSingleton(type);
            }

            return service;
        }

        public static IApplicationBuilder MapSockets(this IApplicationBuilder app,
            PathString path, SocketHandler socketHandler)
        {
            return app.Map(path, (x) => x.UseMiddleware<SocketMiddleware>(socketHandler));
        }
    }
}
