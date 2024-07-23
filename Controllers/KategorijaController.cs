using Microsoft.AspNetCore.Mvc;
using Models;

namespace MyNews.Controllers;

[ApiController]
[Route("[controller]")]
public class KategorijaController : ControllerBase
{
    RedisRepo redis = new RedisRepo();

    [HttpGet]
    [Route("getKategorija/{id}")]
    public ActionResult<Kategorija> getKategorija(string id)
    {
        //RedisRepo redis = new RedisRepo();
        var pom = redis.GetKategorija(id);
        return Ok(pom);
    }
    //Marko komentarisao
    //Marko komentarisao2
    //Marko komentar3
    //Marko komentar4
    [HttpPost]
    [Route("SetKategorija/{naziv}")]
    public ActionResult<Kategorija> SetKategorija(string naziv)
    {
        //RedisRepo f = new RedisRepo();
        Kategorija k = new Kategorija();
        k.Naziv = naziv;
        redis.setKategorija(k);
        return Ok(k.Id);
    }
    [HttpGet]
    [Route("getKategorije")]
    public ActionResult<Kategorija> getKategorije()
    {
        //RedisRepo redis = new RedisRepo();
        var pom = redis.GetKategorije();
        return Ok(pom);
    }
    [HttpGet]
    [Route("vratiSveKategorijeNaKojeJeKorisnikPretplacen/{idKorisnika}")]
    public ActionResult vratiSveKategorijeNaKojeJeKorisnikPretplacen(string idKorisnika)
    {
        var kategorije = redis.vratiSveKategorijeNaKojeJeKorisnikPretplacen(idKorisnika);
        return Ok(kategorije);
    }
}