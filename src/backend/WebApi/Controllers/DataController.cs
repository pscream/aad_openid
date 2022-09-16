using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{

    [ApiController]
    [Area("api")]
    public class DataController : ControllerBase
    {

        [HttpGet("[area]/data")]
        [Authorize]
        public ActionResult GetData()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok("Authenticated");
            }

            return Ok("Not Authenticated");
        }


    }

}
