using Microsoft.Identity.Client;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace CodeBug.CDS.Net5.Connector
{
    public class XrmConnector
    {
        private const string ClientId = "This_is_a_guid";
        private const string Secret = "A_SECRET";
        private const string Authority = "https://login.microsoftonline.com/<<tenantid>>";
        private const string OrganizationUrl = "https://yourorg.crm11.dynamics.com/";
        public async Task<string> Connect()
        {
            var impersonationScope = $"{OrganizationUrl}.default";
            var app = ConfidentialClientApplicationBuilder.Create(ClientId)
                .WithClientSecret(Secret)
                .WithAuthority(Authority)
                .Build();
            var scopes = new[] { impersonationScope };
            var authenticationResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();
            return authenticationResult.AccessToken;
        }

        public async Task<Persona> ExecuteWhoAmI()
        {
            var url = $"{OrganizationUrl}/api/data/v9.1/WhoAmI";
            var accessToken = await Connect();

            using (var httpClient = new HttpClient())
            {
                AddDefaultHeaders(httpClient, accessToken);
                var result = await httpClient.GetAsync(new Uri(url));

                if (result.IsSuccessStatusCode)
                {
                    var unDeserialisedPersona = await result.Content.ReadAsStringAsync();
                    return ConverResultToPersona(unDeserialisedPersona);
                }
                else
                {
                    throw new HttpRequestException("Something went wrong with the request");
                }
            }
        }

        private Persona ConverResultToPersona(string contentString)
        {
            if (string.IsNullOrEmpty(contentString))
            {
                throw new ArgumentNullException(contentString);
            }
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            return JsonSerializer.Deserialize<Persona>(contentString, options);

        }

        private void AddDefaultHeaders(HttpClient httpClient, string accessToken)
        {
            httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
            httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
            httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }
}
