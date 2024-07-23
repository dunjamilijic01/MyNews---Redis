using Microsoft.AspNetCore.Mvc;
using Models;

namespace MyNews.Controllers;

[ApiController]
[Route("[controller]")]
public class AdminController : ControllerBase
{
    RedisRepo redis = new RedisRepo();

    [HttpPost]
    [Route("RegisterAdmin")]
    public ActionResult<Korisnik> RegisterAdmin()
    {
        //RedisRepo f = new RedisRepo();
        
        return Ok(redis.RegisterAdmin());
    }

}