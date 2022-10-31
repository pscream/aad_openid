using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using WebApi.Models.Requests;

namespace WebApi.Controllers
{

    [ApiController]
    [Area("api")]
    public class ChatController : ControllerBase
    {

        private string _azureClientId;
        private string _azureTenantId;
        private string _azureClientSecret;

        public GraphServiceClient _graphClient;

        public ChatController(IConfiguration configuration)
        {
            _azureTenantId = configuration["ApplicationSettings:AAD:TenantId"];
            _azureClientId = configuration["ApplicationSettings:AAD:ClientId"];
            _azureClientSecret = configuration["ApplicationSettings:AAD:ClientSecret"];

            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                    .Create(_azureClientId)
                    .WithTenantId(_azureTenantId)
                    .WithClientSecret(_azureClientSecret)
                    .Build();

            ClientCredentialProvider authenticationProvider = new ClientCredentialProvider(confidentialClientApplication);

            _graphClient = new GraphServiceClient(authenticationProvider);
        }

        [HttpPost("[area]/[controller]")]
        public async Task<ActionResult> CreateChat([FromQuery] string ownerId, [FromQuery] string memberId)
        {

            var chat = new Chat
            {
                ChatType = ChatType.OneOnOne,
                Members = new ChatMembersCollectionPage()
                    {
                        new AadUserConversationMember
                        {
                            Roles = new List<String>()
                            {
                                "owner"
                            },
                            AdditionalData = new Dictionary<string, object>()
                            {
                                {"user@odata.bind", $"https://graph.microsoft.com/v1.0/users('{ownerId}')"}
                            }
                        },
                        new AadUserConversationMember
                        {
                            Roles = new List<String>()
                            {
                                "member"
                            },
                            AdditionalData = new Dictionary<string, object>()
                            {
                                {"user@odata.bind", $"https://graph.microsoft.com/v1.0/users('{memberId}')"}
                            }
                        }
                    }
            };

            var results = await _graphClient.Chats.Request().AddAsync(chat);

            return Ok(results.Id);

        }

        /// <summary>
        /// Throws an exception 'Requested API is not supported'. The chat actually cannot be deleted.
        /// </summary>
        [HttpDelete("[area]/[controller]/{chatId}")]
        public async Task<ActionResult> DeleteChat([FromRoute] string chatId)
        {

            await _graphClient.Chats[chatId].Request().DeleteAsync();

            return Ok();

        }

        /// <summary>
        /// Throws an exception 'Requested API is not supported in application-only context'
        /// </summary>
        [HttpPost("[area]/[controller]/{chatId}/message")]
        public async Task<ActionResult> SendToChat([FromRoute] string chatId, [FromQuery] string senderId, [FromBody] TeamsMessage message)
        {

            var chatMessage = new ChatMessage
            {
                Body = new ItemBody
                {
                    Content = message.Message
                },
                From = new IdentitySet
                {
                    User = new Identity
                    {
                        Id = senderId,

                    }

                }
            };

            var results = await _graphClient.Users[senderId].Chats[chatId].Messages
                .Request()
                .AddAsync(chatMessage);

            // Also doesn't work as for the application context
            //var results = await _graphClient.Chats[chatId].Messages
            //            .Request()
            //            .AddAsync(chatMessage);

            return Ok(results.Id);

        }

        [HttpPost("[area]/[controller]/{chatId}/messageOnBehalf")]
        public async Task<ActionResult> SendOnBehalfToChat([FromRoute] string chatId, [FromBody] TeamsMessage message)
        {

            var chatMessage = new ChatMessage
            {
                Body = new ItemBody
                {
                    Content = message.Message
                },
            };

            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                    .Create(_azureClientId)
                    .WithTenantId(_azureTenantId)
                    .WithClientSecret(_azureClientSecret)
                    .Build();

            var userAssertion = new UserAssertion(message.Token, "urn:ietf:params:oauth:grant-type:jwt-bearer");
            var result = await confidentialClientApplication.AcquireTokenOnBehalfOf(new List<string>() { "Chat.ReadWrite" }, userAssertion).ExecuteAsync();

            var authProvider = new DelegateAuthenticationProvider(async (request) => {

                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
                await Task.CompletedTask;
            });

            var delegateGraphClient = new GraphServiceClient(authProvider);

            var results = await delegateGraphClient.Chats[chatId].Messages
                .Request()
                .AddAsync(chatMessage);

            return Ok(results.Id);

        }

        [HttpGet("[area]/[controller]")]
        public async Task<ActionResult> ListChats([FromQuery] string memberId)
        {

            var results = await _graphClient.Users[memberId].Chats
                    .Request().GetAsync();

            if (results.AdditionalData["statusCode"].ToString() == "OK")
            {
                string chatString = "Chats:";
                foreach(var chat in results.CurrentPage)
                {
                    chatString += Environment.NewLine + chat.Id; 
                }
                return Ok(chatString);
            }

            return Ok("No Chat were found.");

        }

        /// <summary>
        /// Throws an exception 'Invoked API requires Protected API access in application-only context when not using Resource Specific Consent'. 
        /// Visit https://learn.microsoft.com/en-us/graph/teams-protected-apis for more datails.
        /// </summary>
        [HttpGet("[area]/[controller]/{chatId}/message")]
        public async Task<ActionResult> ListMessages([FromRoute] string chatId)
        {

            var results = await _graphClient.Chats[chatId].Messages
                    .Request().GetAsync();

            if (results.AdditionalData["statusCode"].ToString() == "OK")
            {
                string chatString = "Messages:";
                foreach (var message in results.CurrentPage)
                {
                    chatString += Environment.NewLine + $"{message.Id}: {message.Body}";
                }
                return Ok(chatString);
            }

            return Ok("No Messages were found.");

        }

        // We use 'POST' here because we need to send a token through the body of the request
        // (the token is too long to be passed in a route), which is not usual practice for 'GET' requests
        [HttpPost("[area]/[controller]/{chatId}/getMessagesOnBehalf")]
        public async Task<ActionResult> ListMessagesOnBehalf([FromRoute] string chatId, [FromBody] TeamsChat chat)
        {

            IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                    .Create(_azureClientId)
                    .WithTenantId(_azureTenantId)
                    .WithClientSecret(_azureClientSecret)
            .Build();

            var userAssertion = new UserAssertion(chat.Token, "urn:ietf:params:oauth:grant-type:jwt-bearer");
            var result = await confidentialClientApplication.AcquireTokenOnBehalfOf(new List<string>() { "Chat.ReadWrite" }, userAssertion).ExecuteAsync();

            var authProvider = new DelegateAuthenticationProvider(async (request) => {

                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.AccessToken);
                await Task.CompletedTask;
            });

            var delegateGraphClient = new GraphServiceClient(authProvider);

            var results = await delegateGraphClient.Chats[chatId].Messages
                    .Request().GetAsync();

            if (results.AdditionalData["statusCode"].ToString() == "OK")
            {
                string chatString = "Messages:";
                foreach (var message in results.CurrentPage)
                {
                    chatString += Environment.NewLine + $"{message.Id}: {message.Body.Content}";
                }
                return Ok(chatString);
            }

            return Ok("No Messages were found.");

        }

    }

}
