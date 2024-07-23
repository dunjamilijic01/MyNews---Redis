using Microsoft.AspNetCore.Mvc;
using Models;

namespace MyNews.Controllers;

[ApiController]
[Route("[controller]")]
public class CounterController : ControllerBase
{
    RedisRepo redis = new RedisRepo();

    [HttpGet]
    [Route("GetCounter/{vestId}")]
    public ActionResult GetCounter(string vestId)
    {
        //RedisRepo f = new RedisRepo();
        
        return Ok(redis.getCounterValue(vestId));
    }

    [HttpGet]
    [Route("IncrementCounter/{vestId}")]
    public ActionResult IncrementCounter(string vestId)
    {
        //RedisRepo f = new RedisRepo();
        redis.incrementCounterValue(vestId);
        return Ok("inkrementirano!");
    }
}