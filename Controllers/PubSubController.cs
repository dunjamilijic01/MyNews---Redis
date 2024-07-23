using Microsoft.AspNetCore.Mvc;
using Models;

namespace MyNews.Controllers;

[ApiController]
[Route("[controller]")]
public class PubSubController : ControllerBase
{
    RedisRepo redis = new RedisRepo();

    /*[HttpPost]
    [Route("Publish/{kanal}/{msg}")]
    public string Publish(string kanal,string msg)
    {
        redis.PublishMesg(kanal,msg);
        return "Ok";
    }*/

    [HttpGet]
    [Route("Subcribe/{kategorija}/{user}")]
    public ActionResult Subscribe(string kategorija,string user)
    {
        redis.Subscribe(kategorija,user);
        return Ok("ok");
    }
    [HttpGet]
    [Route("GetSubscriptions/{user}")]
    public ActionResult GetSubscriptions(string user)
    {
        List<Vest> vesti=redis.getSubscriptions(user);
        Korisnik k= redis.GetKorisnik(user);

        return Ok(new{
            status=k.Procitano,
            vesti=vesti.Select(p=>
            new{
            Id=p.Id,
            Naslov=p.Naslov,
            KratakTekst=p.KratakTekst,
            DuziTekst=p.DuziTekst,
            Datum=p.DatumObjavljivanja,
            Slika = p.Slika,
            Kategorija=redis.GetKategorija(p.KategorijaID)
            })

        });
    }

}