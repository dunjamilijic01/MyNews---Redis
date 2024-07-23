using Microsoft.AspNetCore.Mvc;
using Models;

[ApiController]
[Route("[controller]")]
public class KorisnikController : ControllerBase
{
    RedisRepo redis = new RedisRepo();

    [HttpPost]
    [Route("RegisterUser/{mail}/{password}")]
    public ActionResult RegisterUser(string mail,string password)
    {
        Korisnik k = new Korisnik();
        k.Id = mail;
        k.Password = password;
        k.Procitano=true;
        if(redis.RegisterUser(k))
        {
               return Ok(new{
                Uloga = "Korisnik",
                Mail = k.Id,
                Obavestenja=redis.getSubscriptions(mail)
               });
        }
        else
            return BadRequest("Korisnik vec postoji");
        
        
    }

    [HttpGet]
    [Route("Login/{username}/{password}")]
    public ActionResult Login(string username,string password)
    {   
        var jesteAdmin = redis.Login(username,password);
        if(jesteAdmin)
        {
            return Ok(new{
                Uloga = "Admin",
                Mail = "admin@gmail.com"
            });
        }
        else
        {
            //vracam korisnika
            Korisnik k = redis.GetKorisnik(username);
            if(k.Password==password)
            {
               return Ok(new{
                Uloga = "Korisnik",
                Mail = k.Id,
                Obavestenja=redis.getSubscriptions(username)
            });
            }
            else
            {
                return BadRequest("Nevalidan password");
            }
        }
        
    }
    [HttpPut]
    [Route("ChangeStatus/{user}")]
    public ActionResult ChangeStatus(string user)
    {
        
        redis.ChangeStatus(user);
        Korisnik k=redis.GetKorisnik(user);
        return Ok(k.Procitano);

    }
    [HttpGet]
    [Route ("GetKorisnik/{user}")]
    public ActionResult GetKorisnik(string user)
    {
        return Ok(redis.GetKorisnik(user));
    }
    [HttpGet]
    [Route ("GetStatusKorisnika/{userId}")]
    public ActionResult GetStatusKorisnika(string userId)
    {
        return Ok(redis.GetKorisnik(userId).Procitano);
    }
}