using RunGroupWebApp.Data.Enum;
using RunGroupWebApp.Models;

namespace RunGroupWebApp.ViewModels
{
    public class EditRaceViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public IFormFile Image { get; set; }

        public int AddressId { get; set; }

        public Address address { get; set; }

        public String? Url { get; set; }

        public RaceCategory raceCategory { get; set; }
    }
}
