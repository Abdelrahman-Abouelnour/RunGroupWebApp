using Microsoft.EntityFrameworkCore;
using RunGroupWebApp.Data;
using RunGroupWebApp.Interfaces;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.Repository
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardRepository(ApplicationDbContext context,IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }
        public  async Task<List<Club>> GetAllUserClubs()
        {
            var currUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            var UserClubs = _context.Clubs.Where(r => r.AppUser.Id == currUser);
            return UserClubs.ToList();
        }

        public async Task<List<Race>> GetAllUserRaces()
        {
            var currUser = _httpContextAccessor.HttpContext?.User.GetUserId();
            var UserRaces = _context.Races.Where(r => r.AppUser.Id == currUser);
            return UserRaces.ToList();
        }
        public async Task<AppUser> GetUserById(string id) { 
            return await _context.Users.FindAsync(id);
        }
        public async Task<AppUser> GetUserByIdNoTracking(string id)
        {
            return await _context.Users.Where(u => u.Id == id).AsNoTracking().FirstOrDefaultAsync();
        }
        public bool Update(AppUser user) {
            _context.Users.Update(user);
            return Save();      
        }
        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0? true: false;
        }
    }
}
