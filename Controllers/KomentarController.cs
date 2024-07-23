using Microsoft.AspNetCore.Mvc;
using Models;

namespace MyNews.Controllers;

[ApiController]
[Route("[controller]")]
public class KomentarController : ControllerBase
{
    RedisRepo redis = new RedisRepo();

    [HttpPost]
    [Route("AddComment/{idVesti}/{tekst}/{korisnikID}")]
    public ActionResult AddComment(string idVesti,string tekst,string korisnikID)
    {
        Komentar k = new Komentar();
        k.Tekst = tekst;
        k.KorisnikId = korisnikID;
        redis.DodavanjeKomentaraVesti(k,idVesti);
        return Ok(k);
    }
}