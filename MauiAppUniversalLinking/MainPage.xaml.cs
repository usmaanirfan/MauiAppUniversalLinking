namespace MauiAppUniversalLinking;
using System.Security.Cryptography;
using System.Text;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string verifier = UrlUtils.RandomString(43);
        Dictionary<string, string> queryParams = GetLoginParams(verifier);
   
        Uri startUri = new Uri(UrlUtils.BuildUrlWithQueryStringUsingUriBuilder(".............", queryParams));
           
        WebAuthenticatorResult authResult = await WebAuthenticator.Default.AuthenticateAsync(
            startUri,
            new Uri("...................."));
        
        string code = authResult.Properties["code"];
            

    }
    
    private Dictionary<string, string> GetLoginParams(string verifier)
    {
        Dictionary<string, string> queryParams = [];
        queryParams.Add("client_id", "....");
        queryParams.Add("response_type", "code");
        queryParams.Add("scope", "iam:read iam:write openid profile email");
        queryParams.Add("redirect_uri", ".......");

        var sha256 = SHA256.Create();
        // Here we create a hash of the code verifier
        var challengeBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(verifier));

        // and produce the "Code Challenge" from it by base64Url encoding it.
        string codeChallenge = Convert.ToBase64String(challengeBytes)
            .TrimEnd('=').Replace('+', '-').Replace('/', '_');
       
            
        queryParams.Add("code_challenge", codeChallenge);
        queryParams.Add("code_challenge_method", "S256");
        queryParams.Add("reset-session", "true");
        return queryParams;
    }
}

public class UrlUtils {

    private static Random random = new Random();

    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    
    public static string BuildUrlWithQueryStringUsingUriBuilder(string basePath, Dictionary<string, string> queryParams)
    {
        var uriBuilder = new UriBuilder(basePath)
        {
            Query = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"))
        };
        return uriBuilder.Uri.AbsoluteUri;
    }

}
