using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace CLK.AspNetCore
{
    public interface MiddlewareLauncher
    {
        // Methods
        void ConfigureMiddleware(IApplicationBuilder app);

        void ConfigureServices(IServiceCollection services);
    }
}
