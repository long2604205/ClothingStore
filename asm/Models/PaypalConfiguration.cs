using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PayPal.Api;

namespace asm.Models
{
    public static class PaypalConfiguration
    {
        // Variables for storing the clientId and clientSecret key  
        public static readonly string ClientId;
        public static readonly string ClientSecret;

        // Constructor  
        static PaypalConfiguration()
        {
            var configuration = GetConfiguration();
            ClientId = configuration["PaypalSettings:ClientId"];
            ClientSecret = configuration["PaypalSettings:Secret"];
        }

        // Getting configuration  
        private static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");

            return builder.Build();
        }

        // Getting properties from the configuration  
        public static Dictionary<string, string> GetConfig()
        {
            var config = new Dictionary<string, string>
            {
                { "clientId", ClientId },
                { "clientSecret", ClientSecret }
            };
            return config;
        }

        private static string GetAccessToken()
        {
            // Getting accesstoken from paypal  
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }

        public static APIContext GetAPIContext()
        {
            // Return apicontext object by invoking it with the accesstoken  
            APIContext apiContext = new APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}
