using System.Text.Json;
using Models;
using ServiceStack.Redis;
using StackExchange.Redis;

public class RedisRepo
{
    RedisClient redis = new RedisClient("localhost:6379");
    static ConnectionMultiplexer redis2 = ConnectionMultiplexer.Connect("localhost:6379");
    IDatabase db = redis2.GetDatabase();
    ISubscriber sub = redis2.GetSubscriber();
    
    public RedisRepo(){}
    

    public void setKategorija(Kategorija kat)
    {
        var serializedkat = JsonSerializer.Serialize<Kategorija>(kat);   
        redis.Set(kat.Id,serializedkat);
        redis.AddItemToSet("kategorije",serializedkat);
    }
    public Kategorija GetKategorija(string id)
    {
        var kat = redis.Get<string>(id);
        Kategorija kateg = JsonSerializer.Deserialize<Kategorija>(kat);
        return kateg;
    }
    public List<Kategorija> GetKategorije()
    {
        List<Kategorija> kategorije = new List<Kategorija>();
        var kategorijeIzRedisa = redis.GetAllItemsFromSet("kategorije");
        foreach(var k in kategorijeIzRedisa)
        {
            kategorije.Add(JsonSerializer.Deserialize<Kategorija>(k));
        }
        return kategorije;
    }

    public void createVest(Vest vest)
    {
        var serializedVest= JsonSerializer.Serialize<Vest>(vest);
        redis.Set(vest.Id,serializedVest);
        //redis.AddItemToList("vesti",serializedVest);
        //redis.PushItemToList("vesti",serializedVest);
        redis.EnqueueItemOnList("vesti",serializedVest);
        //redis.AddItemToList(vest.KategorijaID+":vest",vest.Id);
        //redis.PushItemToList(vest.KategorijaID+":vest",vest.Id);
        redis.EnqueueItemOnList(vest.KategorijaID+":vest",vest.Id);
        redis.AddItemToSortedSet("popularnevesti",serializedVest,0); // set za sortiranje vesti
        redis.Set("counter:"+vest.Id,0);
    }

    public string getCounterValue(string id)
    {
        return redis.Get<string>("counter:"+id);
    }

    public void incrementCounterValue(string id)
    {
        redis.IncrementValue("counter:"+id);
        
    }

    public List<Vest> GetVesti()
    {
        var sveVesti=redis.GetAllItemsFromList("vesti");
        List<Vest> listaVesti= new List<Vest>();
        foreach (var vest in sveVesti)
        {
            listaVesti.Add(JsonSerializer.Deserialize<Vest>(vest));
        }

        return listaVesti;
    }

    public void DodavanjeKomentaraVesti(Komentar komentar,string idVesti)
    {
        var serializedComment = JsonSerializer.Serialize<Komentar>(komentar);
        redis.Set(komentar.Id,serializedComment); // dodat komentar u bazu

        var vestIzRedisa = redis.Get<string>(idVesti);
        Vest vest = JsonSerializer.Deserialize<Vest>(vestIzRedisa); // preuzeta vest za konkretan komentar

        redis.AddItemToList(vest.Id+":komentari",komentar.Id);
        
    }
    public List<Komentar> SviKomentariVesti(string idVest)
    {
        List<Komentar> komentari = new List<Komentar>(); 
        var vestIzRedisa = redis.Get<string>(idVest);
        Vest vest = JsonSerializer.Deserialize<Vest>(vestIzRedisa);

        var komentariIzRedisa = redis.GetAllItemsFromList(vest.Id+":komentari");
        foreach(var k in komentariIzRedisa)
        {
            komentari.Add(JsonSerializer.Deserialize<Komentar>(redis.Get<string>(k)));
        }
        return komentari;
    }

    public Vest VestSaKomentarima(string idVest)
    {
        var vestIzRedisa = redis.Get<string>(idVest);
        Vest vest = JsonSerializer.Deserialize<Vest>(vestIzRedisa);
        return vest;
    }

    public List<Vest> GetVestiOdredjeneKategorije(string kategorijaId)
    {
        var sveVestiOdredjeneKat=redis.GetAllItemsFromList(kategorijaId+":vest");
        List<Vest> listaVesti= new List<Vest>();
        foreach (var vest in sveVestiOdredjeneKat)
        {
            listaVesti.Add(JsonSerializer.Deserialize<Vest>(redis.Get<string>(vest)));
        }
        return listaVesti;
    }
    public Vest GetVest(string Id)
    {
        var vest = redis.Get<string>(Id);
        Vest v = JsonSerializer.Deserialize<Vest>(vest);
        return v;
    }
    public Korisnik RegisterAdmin()
    {
        var a=redis.Get<string>("admin@gmail.com");
        if(a==null)
        {
            Korisnik k= new Korisnik();
            k.Id="admin@gmail.com";
            k.Password="admin123";
            redis.Set(k.Id,JsonSerializer.Serialize<Korisnik>(k));
            return k;
        }
        return null;
    }

    public bool RegisterUser(Korisnik k)
    {
        var postojiKorisnik = redis.Get<string>(k.Id);
        if(postojiKorisnik==null)
        {
            var serializedKorisnik = JsonSerializer.Serialize<Korisnik>(k);
            //redis.Add<string>(k.Id,serializedKorisnik);
            redis.Set(k.Id,serializedKorisnik);
            redis.AddItemToSet("korisnici",serializedKorisnik);
            
            return true;
        }
        else
            return false;
    }
    public bool Login(string username,string password)
    {
        if(username=="admin@gmail.com" && password=="admin123")
        {
            return true;
        }
        else
            return false;
    }
    public Korisnik GetKorisnik(string mail)
    {
        var serializedKorisnik = redis.Get<string>(mail);
        Korisnik k = JsonSerializer.Deserialize<Korisnik>(serializedKorisnik);
        return k;
    }
    public List<Vest> getPopularneVesti()
    {
        List<Vest> popularneVesti = new List<Vest>();
        var serilizedVesti = redis.GetAllItemsFromSortedSetDesc("popularnevesti");
        foreach(var v in serilizedVesti)
        {
            popularneVesti.Add(JsonSerializer.Deserialize<Vest>(v));
        }
        return popularneVesti;

    }
    public double UpdateScore(string idVesti)
    {
        incrementCounterValue(idVesti);
        double score = 0;
        double counterVest = Double.Parse(getCounterValue(idVesti));
        Vest v = GetVest(idVesti);
        //double numDays = Double.Parse((DateTime.Today - v.DatumObjavljivanja.Date).ToString());
        double numDays = (DateTime.Today - v.DatumObjavljivanja.Date).TotalDays;
        score = counterVest*2.5/(numDays+1);
        redis.AddItemToSortedSet("popularnevesti",redis.Get<string>(idVesti),score);
        return score;
    }
    public void DeleteVest(string idVesti)
    {
        Vest v = GetVest(idVesti);
        redis.Remove(idVesti); //izbrisana vest iz baze
        var serializedVest = JsonSerializer.Serialize<Vest>(v);
        redis.RemoveItemFromList("vesti",serializedVest);
        redis.RemoveItemFromList(v.KategorijaID+":vest",v.Id);
        redis.RemoveItemFromSortedSet("popularnevesti",serializedVest);
        redis.Remove("counter:"+v.Id);
        Kategorija k=GetKategorija(v.KategorijaID);
        var korisnici= redis.GetAllItemsFromSet("sub:"+k.Id);
        foreach( var kor in korisnici)
        {
            Console.WriteLine(db.ListPosition("sub:"+kor,serializedVest));
           if(db.ListPosition("sub:"+kor,serializedVest)==0)
           {
                redis.RemoveItemFromList("sub:"+kor,serializedVest);
           }
        }

    }

    public void PublishMesg(string kanal,string msg)
    {
       // redis.PublishMessage("kanal",msg);
       //redis.PublishMessage("kanal",msg);
       sub.Publish(kanal,msg);
    }

    List<string> msgs=new List<string>();
    public void Subscribe(string kanal,string user){
        //IRedisSubscriptionAsync redisSub= (IRedisSubscriptionAsync)redis.CreateSubscription();
        //redisSub.SubscribeToChannels(new string[]{"kanal"});
        //redisSub.OnMessageAsync+=Func<out>{}
       
        //redisSub.SubscribeToChannelsAsync(new string[]{"kanal"});
        redis.AddItemToSet("subkategorije:"+user,kanal);
        sub.Subscribe(kanal, (channel, message) => {
            //Console.WriteLine(db.ListPosition("sub:"+user,message));
             if(db.ListPosition("sub:"+user,message)<0)
                {
                    redis.EnqueueItemOnList("sub:"+user,message);
                    Korisnik k= GetKorisnik(user);
                    k.Procitano=false;
                    //redis.SetValueIfExists(user,JsonSerializer.Serialize<Korisnik>(k));
                    redis.Set(user,JsonSerializer.Serialize<Korisnik>(k));
                    redis.AddItemToSet("sub:"+kanal,user);
                }

             //redis.EnqueueItemOnList("sub:"+user,message);
        });
    }
    public List<string> vratiSveKategorijeNaKojeJeKorisnikPretplacen(string id)
    {
        var kategorije = redis.GetAllItemsFromSet("subkategorije:"+id);
        List<string> res = new List<string>(kategorije.Count());
        foreach (var k in kategorije)
        {
            res.Add(k);
        }
        return res;
    }
    public List<string> getList()
    {
        return msgs;
    }

    public List<Vest> getSubscriptions(string user)
    {
        List<Vest> vesti= new List<Vest>();
        var redisVesti=redis.GetAllItemsFromList("sub:"+user);
        foreach(var v in redisVesti)
        {
            Vest vest= JsonSerializer.Deserialize<Vest>(v);
            vesti.Add(vest);
        }
        return vesti;
    }

    public void ChangeStatus(string user)
    {
        Korisnik k= GetKorisnik(user);
        k.Procitano=true;
        //redis.SetValueIfExists(user,JsonSerializer.Serialize<Korisnik>(k));
        redis.Set(user,JsonSerializer.Serialize<Korisnik>(k));

    }

    
}