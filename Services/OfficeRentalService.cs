using Data;
using Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Services
{
    public class OfficeRentalService : IOfficeRentalService
    {
        public List<Location> Location { get; set; }
        public List<Office> Office { get; set; }
        public List<Booking> Bokking { get; set; }
        public List<Suggestion> Suggestion { get; set; }
    }
}
