using BlazorAppEcommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppEcommerce.Services
{
    public class ClientService
    {
        private readonly ApplicationDbContext _context;

        public ClientService(ApplicationDbContext context)
        {
            _context = context;
        }

        public int AddClient(Client client)
        {
            _context.Clients.Add(client);
            _context.SaveChanges();
            return client.CId;
        }

        public Client GetClient(int id)
        {
            return _context.Clients.FirstOrDefault(c => c.CId == id);
        }

        public int UpdateClient(Client c)
        {
            _context.Clients.Update(c);
            _context.SaveChanges();
            return c.CId;
        }
    }
}