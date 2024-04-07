namespace BFFApi.Configurations
{
    public class OpenIdConnect
    {
        public string Name { get; set; } = "OpenIdConnect";
        public string Authority { get; set; } = "";
        public string ClientId { get; set; } = "";
        public string ClientSecret { get; set; } = "";
        public string Scopes { get; set; } = "";
        public bool UsePkce { get; set; } = false;
        public string AcrValues { get; set; } = "";
        public string UiLocales { get; set; } = "";
        public string CallBackPath { get; set; } = "";
        public string PostSignOutRedirect { get; set; } = "/";
        public string IdPSignOutPath { get; set; } = "";
    }
}
