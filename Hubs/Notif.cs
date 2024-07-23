
using System.Threading.Tasks;

using Microsoft.AspNetCore.SignalR;
using Models;

namespace Hubs
{
    public class Notif : Hub<INotifHub>
    {
        public async Task SendMessageToAll( string idKategorije,string Id,string Naslov,string KratakTekst,string DuziTekst,string Slika,DateTime DatumObjavljivanja,string KategorijaID) 
        {
            await Clients.Group(idKategorije).SendMessageToAll(Id,Naslov,KratakTekst,DuziTekst,Slika,DatumObjavljivanja,KategorijaID); 
        }
        
        public async Task JoinGroup(string idKategorije)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId,idKategorije);
        }
       
    }
}