using BlazorAppEcommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorAppEcommerce.Services
{
    public class SellerService
    {
        private readonly ApplicationDbContext _context;

        public SellerService(ApplicationDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<int> AddSeller(Seller seller)
        {
            _context.Sellers.Add(seller);
            await _context.SaveChangesAsync();
            return seller.SId;
        }

        // READ BY ID (includes user)
        public async Task<Seller?> GetSeller(int id)
        {
            return await _context.Sellers
                .Include(s => s.user)
                .FirstOrDefaultAsync(s => s.SId == id);
        }

        // READ ALL
        public async Task<List<Seller>> GetAllSellers()
        {
            return await _context.Sellers
                .Include(s => s.user)
                .ToListAsync();
        }

        // UPDATE (FIXED)
        public async Task<bool> UpdateSeller(Seller updatedSeller)
        {
            var seller = await _context.Sellers
                .FirstOrDefaultAsync(s => s.SId == updatedSeller.SId);

            if (seller == null)
                return false;

            // ONLY VALID FIELDS FROM YOUR MODEL
            seller.UserId = updatedSeller.UserId;
            seller.SellerRating = updatedSeller.SellerRating;

            await _context.SaveChangesAsync();
            return true;
        }

        // DELETE
        public async Task<bool> DeleteSeller(int id)
        {
            var seller = await _context.Sellers
                .FirstOrDefaultAsync(s => s.SId == id);

            if (seller == null)
                return false;

            _context.Sellers.Remove(seller);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}