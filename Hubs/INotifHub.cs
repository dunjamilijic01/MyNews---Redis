using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Models;

namespace Hubs
{
    public interface INotifHub
    {
         Task SendMessageToAll(string idVesti,string naslov,string kt,string dt,string slika,DateTime datumObjavljivanja,string idKat);
          
         Task JoinGroup(string idKategorije);
    }
}