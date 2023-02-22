using System;
using Microsoft.Extensions.Caching.Distributed;

namespace UI.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication EnsureDistributedCacheIsUsed(this WebApplication app, bool enforceDistributedCache)
        {
            var logger = app.Logger;

            var typeOfCache = app.Services.GetService<IDistributedCache>()?.GetType();

            logger.LogInformation("Distributed cache implementation is {TypeOfCache}", typeOfCache);

            if (typeOfCache == typeof(MemoryDistributedCache))
            {
                if (enforceDistributedCache)
                {
                    logger.LogCritical("In-memory distributed cache must not be used");
                    throw new ApplicationException("In-memory distributed cache must not be used");
                }
                else
                {
                    logger.LogWarning("In-memory distributed cache in use");
                }
            }

            return app;
        }
    }
}

