using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Util.Store;

namespace EmailOAuthBlazorApp.Services
{
    public static class GmailOAuthHelper
    {
        // Remplacez avec vos informations réelles.
        private const string ClientId = "VOTRE_CLIENT_ID";
        private const string ClientSecret = "VOTRE_CLIENT_SECRET";
        private const string RedirectUri = "https://localhost:7087/callback";

        // Génère l'URL d'authentification pour l'utilisateur.
        public static string GenerateAuthorizationUrl()
        {
            var flow = CreateFlow();
            // La méthode Build() peut renvoyer un objet Uri dans certaines versions, donc on convertit en string.
            var authUrl = flow.CreateAuthorizationCodeRequest(RedirectUri).Build().ToString();
            return authUrl;
        }

        // Échange le code d'autorisation contre un jeton OAuth2.
        public static async Task<UserCredential> ExchangeCodeForTokenAsync(string code)
        {
            var flow = CreateFlow();
            var token = await flow.ExchangeCodeForTokenAsync("user", code, RedirectUri, CancellationToken.None);
            return new UserCredential(flow, "user", token);
        }

        private static GoogleAuthorizationCodeFlow CreateFlow()
        {
            return new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
                    ClientId = ClientId,
                    ClientSecret = ClientSecret
                },
                // Ici, le scope complet pour Gmail
                Scopes = new[] { "https://mail.google.com/" },
                // On stocke le jeton dans "token.json". Le booléen false désactive le verrouillage exclusif.
                DataStore = new FileDataStore("token.json", false)
            });
        }
    }
}
