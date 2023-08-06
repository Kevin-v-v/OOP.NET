using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Util;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace BankConsole;

public static class EmailService{
    async public static Task<bool> SendMail(){
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Kevin", "verify.shopi@gmail.com"));
        message.To.Add(new MailboxAddress("Kevin", "kevin.villalobosvl@uanl.edu.mx"));
        message.Subject = "BankConsole: Usuarios nuevos";

        message.Body = new TextPart("plain"){
            Text = GetEmailText()
        };

        //Se utiliza autenticación tipo OAuth2 
        string email = "verify.shopi@gmail.com";
        //Se abre el archivo que contiene los secretos para la conexion
        var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read);

        //Se generan las credenciales mediante el servicio de autorización de google para acceder a la API de Gmail
        var googleCredentials = await GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleClientSecrets.FromStream(stream).Secrets, new[] { GmailService.Scope.MailGoogleCom }, email, CancellationToken.None);
        //Si el token está vencido se renueva
        if (googleCredentials.Token.IsExpired(SystemClock.Default))
        {
            await googleCredentials.RefreshTokenAsync(CancellationToken.None);
        }

        //Se cierra el stream del archivo de los secretos
        stream.Close();


        using (var client = new SmtpClient())
        {
            client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            //Se genera el objeto de autenticación OAuth2 para el cliente
            var oauth2 = new SaslMechanismOAuth2(googleCredentials.UserId, googleCredentials.Token.AccessToken);
            client.Authenticate(oauth2);

            await client.SendAsync(message);
            client.Disconnect(true);
            return true;
        }   

    }
    private static string GetEmailText(){
        List<User> newUsers = Storage.GetNewUsers();

        if(newUsers.Count == 0){
            return "No hay usuarios nuevos";
        }

        string emailText = "Usuarios agregados hoy:\n";

        foreach (User user in newUsers)
        {
            emailText += $"\t+ {user.ShowData()}\n";
        }
        return emailText;
    }
}