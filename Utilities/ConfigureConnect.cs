using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using BFFApi.Configurations;

namespace BFFApi.Utilities
{
    public static class OpenIdConnectConfigurator
    {
        public static void ConfigureOpenIdConnect(OpenIdConnectOptions options, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            var oidcOptions = new OpenIdConnect();
            configuration.GetSection("OpenIdConnect").Bind(oidcOptions);

            // Set the authority to configured IdP domain
            options.Authority = oidcOptions.Authority;

            // Configure the IdP Client ID and Client Secret
            options.ClientId = oidcOptions.ClientId;
            options.ClientSecret = oidcOptions.ClientSecret;

            // Set response type to code
            options.ResponseType = OpenIdConnectResponseType.Code;

            options.ResponseMode = OpenIdConnectResponseMode.FormPost;

            // Configure the scope
            options.Scope.Clear();
            options.Scope.Add(oidcOptions.Scopes);

            // Set the callback path, so the IdP will call back to it
            // Also ensure that you have added the URL as an Allowed Callback URL in your IdP config or IdP will throw an exception
            options.CallbackPath = new PathString(oidcOptions.CallBackPath);

            // This saves the tokens in the session cookie
            options.SaveTokens = true;

            // Pkce
            options.UsePkce = oidcOptions.UsePkce;

            options.Events.OnRedirectToIdentityProvider = (context) =>
            {
                context.ProtocolMessage.SetParameter("state", oidcOptions.Name);
                context.ProtocolMessage.SetParameter("acr_values", oidcOptions.AcrValues);
                context.ProtocolMessage.UiLocales = oidcOptions.UiLocales;
                return Task.CompletedTask;
            };

            options.Events.OnRedirectToIdentityProviderForSignOut = async context =>
            {
                var idToken = await context.HttpContext.GetTokenAsync("id_token");
                var logoutUri = oidcOptions.Authority + oidcOptions.IdPSignOutPath + "?post_logout_redirect_uri=" + Uri.EscapeDataString(oidcOptions.PostSignOutRedirect) + "&id_token_hint=" + idToken + "&client_id=" + oidcOptions.ClientId;
                context.Response.Redirect(logoutUri);
                context.HandleResponse();
            };
        }
    }
}