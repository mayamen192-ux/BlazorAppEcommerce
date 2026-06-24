using BlazorAppEcommerce.DTOs;
using BlazorAppEcommerce.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BlazorAppEcommerce.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // =========================
        // ADD USER (REGISTER)
        // =========================
        public async Task<(bool Success, string Message, int? UserId)> AddUser(User user)
        {
            if (user == null)
                return (false, "User is null", null);

            // Check if email already exists
            var exists = await _context.Users
                .AnyAsync(u => u.email == user.email);

            if (exists)
            {
                _logger.LogWarning("Registration failed - email exists: {Email}", user.email);
                return (false, "Email already exists", null);
            }

            // Hash password
            user.password = HashPassword(user.password);

            user.IsActive = true;
            user.createdAt = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User created successfully: {Email}", user.email);

            return (true, "User created successfully", user.Id);
        }

        // =========================
        // LOGIN
        // =========================
        public User? Login(string email, string password)
        {
            var hashedPassword = HashPassword(password);

            var user = _context.Users
                .FirstOrDefault(u => u.email == email && u.password == hashedPassword);

            if (user == null)
            {
                _logger.LogWarning("Login failed for email: {Email}", email);
                return null;
            }

            _logger.LogInformation("Login successful: {Email}", email);
            return user;
        }

        // =========================
        // GET ALL USERS
        // =========================
        public async Task<List<UserOutput>> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.orders)
                    .ThenInclude(o => o.OrderProducts)
                        .ThenInclude(op => op.Product)
                .ToListAsync();

            return users.Select(user => new UserOutput
            {
                name = user.name,
                email = user.email,
                phone = user.phone,
                orders = user.orders?.Select(o => new OrderOutput
                {
                    order_Id = o.order_Id,
                    orderDate = o.orderDate,
                    totalAmount = o.TotalAmount
                }).ToList()
            }).ToList();
        }

        // =========================
        // GET USER BY ID (SECURITY CHECK)
        // =========================
        public async Task<(bool Success, string Message, UserOutput? Data)> GetUserById(
            int id,
            ClaimsPrincipal currentUser)
        {
            var userIdClaim = currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return (false, "Invalid token", null);

            int userId = int.Parse(userIdClaim);

            if (userId != id && !currentUser.IsInRole("Admin"))
                return (false, "Access denied", null);

            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return (false, "User not found", null);

            var output = new UserOutput
            {
                name = user.name,
                email = user.email,
                phone = user.phone
            };

            return (true, "Success", output);
        }

        // =========================
        // PASSWORD HASHING
        // =========================
        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(bytes);
        }
    }
}