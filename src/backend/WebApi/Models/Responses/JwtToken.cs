namespace WebApi.Models.Responses
{

    public class JwtToken
    {

        public string Token { get; }

        public JwtToken(string token)
        {
            Token = token;
        }

    }

}
