namespace DncIds4.ApiGatewayOcelot.Config
{
    public class IdentityServerConfig
    {
        public string IdentityServerUrl { get; set; }
        public string ApiName { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string AuthorizeUrl => $"{this.IdentityServerUrl}/connect/authorize";
        public string TokenUrl => $"{this.IdentityServerUrl}/connect/token";
    }
}
