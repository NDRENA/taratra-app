using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Google.Apis.Auth.OAuth2;

namespace EmailOAuthBlazorApp.Services
{
    public class EmailService
    {
        // Définissez ici l'adresse et le nom de l'expéditeur.
        private const string SenderEmail = "votreadresse@gmail.com";
        private const string SenderName = "Votre Nom";

        private UserCredential _userCredential = null;

        // Stocke le credential obtenu après authentification.
        public void SetUserCredential(UserCredential credential)
        {
            _userCredential = credential;
        }

        public bool IsAuthenticated() => _userCredential != null;

        // Méthode d'envoi d'e-mail utilisant MailKit et le jeton OAuth2.
        public async Task SendEmailAsync(string recipientEmail, string subject, string message)
        {
            if (_userCredential == null)
                throw new System.Exception("L'utilisateur n'est pas authentifié. Veuillez vous authentifier en premier.");

            var accessToken = _userCredential.Token.AccessToken;

            var mimeMessage = new MimeMessage();
            // Pour l'expéditeur, on utilise le constructeur à deux paramètres.
            mimeMessage.From.Add(new MailboxAddress(SenderName, SenderEmail));
            // Pour le destinataire, on utilise également un constructeur à deux paramètres, même si le nom est vide.
            mimeMessage.To.Add(new MailboxAddress(string.Empty, recipientEmail));
            mimeMessage.Subject = subject;
            mimeMessage.Body = new TextPart("plain")
            {
                Text = message
            };

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            // "user" est utilisé par convention ; le token OAuth2 sert ici de mot de passe.
            await client.AuthenticateAsync("user", accessToken);
            await client.SendAsync(mimeMessage);
            await client.DisconnectAsync(true);
        }
    }
}
