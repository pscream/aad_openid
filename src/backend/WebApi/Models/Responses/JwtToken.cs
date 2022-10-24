namespace WebApi.Models.Responses
{

    public class JwtToken
    {

        public string Token { get; }

        public string OpenIdRawData { get; }

        public string OpenIdTokenHeader { get; }

        public string OpenIdTokenPayload { get; }

        public JwtToken(string token)
        {
            Token = token;
        }

        public JwtToken(string token, string openIdRawData, string openIdTokenHeader, string openIdTokenPayload)
            : this(token)
        {
            OpenIdRawData = openIdRawData;
            OpenIdTokenHeader = openIdTokenHeader;
            OpenIdTokenPayload = openIdTokenPayload;
        }

    }

}
