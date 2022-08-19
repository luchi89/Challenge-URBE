namespace NetExam
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NetExam.Abstractions;
    using NetExam.Dto;

    public class OfficeRental : IOfficeRental
    {
        public List<LocationSpecs> Location { get; set; }
        public List<OfficeSpecs> Office { get; set; }
        public List<BookingRequest> Booking { get; set; }

        public OfficeRental()
        {
            Location = new List<LocationSpecs>();
            Office = new List<OfficeSpecs>();
            Booking = new List<BookingRequest>();
        }

        public void AddLocation(LocationSpecs locationSpecs)
        {
            if (Location.Exists(x => x.Name == locationSpecs.Name)) throw new Exception($"El Local {locationSpecs.Name} ya existe");            
            Location.Add(locationSpecs);
        }
        public IEnumerable<ILocation> GetLocations()
        {
            return Location;
        }

        public void AddOffice(OfficeSpecs officeSpecs)
        {
            if (!Location.Exists(x => x.Name == officeSpecs.LocationName)) throw new Exception($"El Local {officeSpecs.LocationName} no existe");
            if (Office.Exists(x => x.Name == officeSpecs.Name && x.LocationName == officeSpecs.LocationName)) throw new Exception($"La Oficina {officeSpecs.Name} ya existe");
            Office.Add(officeSpecs);
        }
        public IEnumerable<IOffice> GetOffices(string locationName)
        {
            return Office.Where(x => x.LocationName == locationName);
        }

        public void BookOffice(BookingRequest bookingRequest)
        {
            if (!Location.Exists(x => x.Name == bookingRequest.LocationName)) throw new Exception($"El Local {bookingRequest.LocationName} no existe");
            if (!Office.Exists(x => x.Name == bookingRequest.OfficeName)) throw new Exception($"La Oficina {bookingRequest.OfficeName} no existe");
            
            var booking = Booking.Where(x => x.LocationName == bookingRequest.LocationName && x.OfficeName == bookingRequest.OfficeName).ToList();

            if (booking.Exists(x => x.DateTime.AddHours(x.Hours) > bookingRequest.DateTime
                                  && x.DateTime < bookingRequest.DateTime.AddHours(bookingRequest.Hours)))
                throw new Exception($"La Oficina {bookingRequest.OfficeName} de Local {bookingRequest.LocationName} tiene una reserva dentro de la fecha solicitada");
            Booking.Add(bookingRequest);
        }

        public IEnumerable<IBooking> GetBookings(string locationName, string officeName)
        {
            return Booking.Where(x => x.LocationName == locationName && x.OfficeName == officeName);
        }
        public IEnumerable<IOffice> GetOfficeSuggestion(SuggestionRequest suggestionRequest)
        {
            var offices = Office.Where(x => x.MaxCapacity >= suggestionRequest.CapacityNeeded);

            if (!string.IsNullOrWhiteSpace(suggestionRequest.PreferedNeigborHood))
                offices.Where(x => Location.Where(y => y.Neighborhood == suggestionRequest.PreferedNeigborHood).Select(y => y.Name).Contains(x.LocationName));
          
            return OfficeResourcesNeeded(suggestionRequest.ResourcesNeeded, offices).OrderBy(x => x.MaxCapacity).OrderBy(x => x.AvailableResources.Count()).ToList();
        }       
        private IEnumerable<OfficeSpecs> OfficeResourcesNeeded(IEnumerable<string> resourcesNeeded, IEnumerable<OfficeSpecs> offices)
        {
            var officesSuggestions = new List<OfficeSpecs>();
            if (resourcesNeeded.Any())
                offices.ToList().ForEach(office =>
                {
                    var resource = true;
                    resourcesNeeded.ToList().ForEach(resourNeeded =>
                    {
                        if (!Array.Exists(office.AvailableResources.ToArray(), x => x == resourNeeded))
                            resource = false;
                    });
                    if (resource)
                        officesSuggestions.Add(office);
                });               
            else officesSuggestions.AddRange(offices);

            return officesSuggestions;
        }     
    }
}