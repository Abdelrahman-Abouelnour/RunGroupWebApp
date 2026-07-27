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
            var currUser = _httpContextAccessor.HttpContext?.User;
            var UserClubs = _context.Clubs.Where(r => r.AppUser.Id == currUser.ToString());
            return UserClubs.ToList();
        }

        public async Task<List<Race>> GetAllUserRaces()
        {
            var currUser = _httpContextAccessor.HttpContext?.User;
            var UserRaces = _context.Races.Where(r => r.AppUser.Id == currUser.ToString());
            return UserRaces.ToList();
        }
    }
}
