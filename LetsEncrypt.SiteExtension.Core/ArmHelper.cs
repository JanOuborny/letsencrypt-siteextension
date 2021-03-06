﻿using LetsEncrypt.Azure.Core.Models;
using Microsoft.Azure.Management.Dns;
using Microsoft.Azure.Management.WebSites;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Rest;
using Microsoft.Rest.Azure.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace LetsEncrypt.Azure.Core
{
    public static class ArmHelper
    {
        public static WebSiteManagementClient GetWebSiteManagementClient(IAzureWebAppEnvironment model)
        {            
            AuthenticationResult token = GetToken(model);
            var creds = new TokenCredentials(token.AccessToken);

            var websiteClient = new WebSiteManagementClient(model.ManagementEndpoint, creds);
            websiteClient.SubscriptionId = model.SubscriptionId.ToString();
            return websiteClient;
        }

        public static DnsManagementClient GetDnsManagementClient(IAzureDnsEnvironment model)
        {
            AuthenticationResult token = GetToken(model);
            var creds = new TokenCredentials(token.AccessToken);

            var dnsClient = new DnsManagementClient(model.ManagementEndpoint, creds);
            dnsClient.SubscriptionId = model.SubscriptionId.ToString();
            return dnsClient;
        }

        private static AuthenticationResult GetToken(IAzureEnvironment model)
        {
            var authContext = new AuthenticationContext(model.AuthenticationEndpoint + model.Tenant);

            var token = authContext.AcquireToken(model.TokenAudience.ToString(), new ClientCredential(model.ClientId.ToString(), model.ClientSecret));
            return token;
        }

        public static HttpClient GetHttpClient(IAzureWebAppEnvironment model)
        {        
            AuthenticationResult token = GetToken(model);

            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token.AccessToken);
            client.BaseAddress = model.ManagementEndpoint;

            return client;
        }
    }
}