﻿using System;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

using AspNetCoreJwtAuthentication.Data.Context;
using AspNetCoreJwtAuthentication.Services.Configuration;

namespace AspNetCoreJwtAuthentication.Middleware
{
    public class JwtTokenIssuerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IConfigurationService configurationService;
        private readonly Func<HttpContext, Task<GenericPrincipal>> principalResolver;

        public JwtTokenIssuerMiddleware(
            RequestDelegate next,
            IConfigurationService configurationService,
            Func<HttpContext, Task<GenericPrincipal>> principalResolver)
        {
            this.next = next;
            this.configurationService = configurationService;
            this.principalResolver = principalResolver;
        }

        public async Task InvokeAsync(
            HttpContext httpContext, 
            ApplicationDbContext applicationDbContext, 
            IJwtHandler jwtHandler)
        {
            string urlPath = this.configurationService.JwtSettings?.Path ?? "/token";

            if (!httpContext.Request.Path.Equals(urlPath, StringComparison.OrdinalIgnoreCase))
            {
                await this.next(httpContext);
                return;
            }

            if (httpContext.Request.Method.Equals("POST") && httpContext.Request.ContentType == "application/json")
            {
                GenericPrincipal genericPrincipal = await this.principalResolver(httpContext);

                if (genericPrincipal == null)
                {
                    httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await httpContext.Response.WriteAsync("wrong username or password");
                    return;
                }

                var tokenResponse = new
                {
                    token = jwtHandler.CreateToken(genericPrincipal)
                };

                string tokenResponseJson = JsonConvert.SerializeObject(tokenResponse);
                httpContext.Response.StatusCode = StatusCodes.Status200OK;
                httpContext.Response.ContentType = "application/json";
                await httpContext.Response.WriteAsync(tokenResponseJson, Encoding.UTF8);
            }
            else
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await httpContext.Response.WriteAsync("Bad request");
            }
        }

        private string GetApplicationUrl(HttpContext context)
        {
            string hostValue = context.Request.Host.Value.ToString();

            string requestScheme = context.Request.Scheme;

            string applicationUrl = string.Format("{0}://{1}", requestScheme, hostValue);

            return applicationUrl;
        }
    }
}
