using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    
    public class ExpediaController : Controller
    {
        

        public IActionResult Index(string[] json2)
        {
            lowestPrice lowlowprice = new lowestPrice(10000, "a", "b");
            List<Rootobject2> travelers = new List<Rootobject2>();
            foreach (string str in json2)
            {
                travelers.Add(JsonConvert.DeserializeObject<Rootobject2>(str));
            }
            foreach(Rootobject2 trav in travelers)
            {
                RunAsync(trav, lowlowprice).Wait();
            }
            ViewData["finished"] = lowlowprice.price +" "+ lowlowprice.destinationCode +" "+ lowlowprice.link;
            return View();
        }

        static async Task RunAsync(Rootobject2 trav, lowestPrice lowlowprice)
        {
            string[] destinations = new string[] { "ATL" };//, "ATL", "ORD" };//, "LAX", "DFW", "DEN", "JFK", "LAS", "SFO", "PHX", "IAH", "CLT", "MIA", "MCO", "EWR", "YYZ", "MSP", "SEA", "DTW" }//, "PHL", "BOS", "MEX", "LGA", "FLL", "IAD", "BWI", "SLC", "MDW", "DCA", "YVR", "SAN", "TPA", "PEK", "HND", "HKG", "CGK", "BKK", "SIN", "CAN", "PVG", "KUL", "SYD", "ICN", "DEL", "SHA", "BOM", "MNL", "CTU", "SZX", "MEL", "NRT", "TPE", "KMG", "BNE", "GMP", "HGH", "CJU", "CTS", "XMN", "FUK", "AKL", "LHR", "CDG", "FRA", "AMS", "MAD", "MUC", "FCO", "IST", "BCN", "LGW", "ORY", "DME", "AYT", "ZRH", "PMI", "CPH", "SVO", "VIE", "OSL", "DUS", "BOG", "MXP", "ARN", "MAN", "BRU", "DUB", "STN", "TXL", "HEL", "LIS", "ATH", "DXB", "JED", "DOH", "RUH", "AUH" };
            try {
                using (var client = new HttpClient())
                {
                    List<Rootobject> resultsList = new List<Rootobject>();

                    foreach (string str in destinations)
                    {


                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        var response = await client.GetAsync("http://terminal2.expedia.com/x/packages?departureDate=" + trav.leaveDate + "&originAirport=" + trav.origin + "&destinationAirport=" + str + "&returnDate=" + trav.returnDate + "regionid=6000479&apikey=1So6mgLTwsd2prAEhYI9e3QAMZZ4m5wF");
                        if (response.IsSuccessStatusCode)
                        {
                            var json = await response.Content.ReadAsStringAsync();
                            Rootobject rootObject = JsonConvert.DeserializeObject<Rootobject>(json);
                            for (int i = 0; i < rootObject.PackageSearchResultList.PackageSearchResult.Count(); i++)
                            {
                                if (rootObject.PackageSearchResultList.PackageSearchResult[i].PackagePrice.TotalPrice.Value < lowlowprice.price)
                                {
                                    lowlowprice.price = rootObject.PackageSearchResultList.PackageSearchResult[i].PackagePrice.TotalPrice.Value;
                                    lowlowprice.link = rootObject.PackageSearchResultList.PackageSearchResult[i].DetailsUrl;
                                    lowlowprice.destinationCode = str;
                                }

                            }
                        }

                    }
                }
            }
            catch
            {
                Console.WriteLine("Toast");
            }
        }

        public class lowestPrice
        {
            public int price;
            public string link;
            public string destinationCode;
            public lowestPrice(int price, string link, string destinationCode)
            {
                this.price = price;
                this.link = link;
                this.destinationCode = destinationCode;
            }
        }
        public class eachPerson
        {
            public List<string> departureAirport = new List<string>();
            public List<string> prices = new List<string>();
            public List<string> webDetails = new List<string>();
        }

        public class Rootobject2
        {
            public string origin;
            public string leaveDate;
            public string returnDate;
        }

        public class Rootobject
        {
            public PackageSearchResultList PackageSearchResultList { get; set; }
            public FlightList FlightList { get; set; }
            public HotelList HotelList { get; set; }
        }

        public class PackageSearchResultList
        {
            public PackageSearchResult[] PackageSearchResult { get; set; }
        }

        public class PackageSearchResult
        {
            public string FlightReferenceIndex { get; set; }
            public string HotelReferenceIndex { get; set; }
            public PackagePrice PackagePrice { get; set; }
            public string DetailsUrl { get; set; }
        }

        public class PackagePrice
        {
            public TotalPrice TotalPrice { get; set; }
            public TotalSavings TotalSavings { get; set; }
        }

        public class TotalPrice
        {
            public int Value { get; set; }
            public string Currency { get; set; }
        }

        public class TotalSavings
        {
            public string Value { get; set; }
            public string Currency { get; set; }
        }

        public class FlightList
        {
            public Flight Flight { get; set; }
        }

        public class Flight
        {
            public string FlightIndex { get; set; }
            public FlightItinerary FlightItinerary { get; set; }
            public FlightPrice FlightPrice { get; set; }
            public string TicketsRemaining { get; set; }
        }

        public class FlightItinerary
        {
            public FlightLeg[] FlightLeg { get; set; }
        }

        public class FlightLeg
        {
            public string FlightLegIndex { get; set; }
            public string FlightDuration { get; set; }
            public FlightSegment[] FlightSegment { get; set; }

        }

        public class FlightSegment
        {
            public string FlightSegmentIndex { get; set; }
            public string DepartureAirportCode { get; set; }
            public string ArrivalAirportCode { get; set; }
            public DateTime DepartureDateTime { get; set; }
            public DateTime ArrivalDateTime { get; set; }
            public string CarrierCode { get; set; }
            public string FlightNumber { get; set; }
            public string FlightDuration { get; set; }
            public string OperatingCarrierCode { get; set; }
        }

        public class FlightPrice
        {
            public BaseRate BaseRate { get; set; }
            public TaxesAndFees TaxesAndFees { get; set; }
            public TotalRate TotalRate { get; set; }
        }

        public class BaseRate
        {
            public string Value { get; set; }
            public string Currency { get; set; }
        }

        public class TaxesAndFees
        {
            public string Value { get; set; }
            public string Currency { get; set; }
        }

        public class TotalRate
        {
            public string Value { get; set; }
            public string Currency { get; set; }
        }

        public class HotelList
        {
            public string CheckInDate { get; set; }
            public string CheckOutDate { get; set; }
            public Hotel[] Hotel { get; set; }
        }

        public class Hotel
        {
            public string HotelIndex { get; set; }
            public string HotelID { get; set; }
            public string Name { get; set; }
            public Location Location { get; set; }
            public string Description { get; set; }
            public string StarRating { get; set; }
            public string TravelerReviewRating { get; set; }
            public string TravelerReviewCount { get; set; }
            public string OverviewThumbnail { get; set; }
            public string RoomRatePlanDescription { get; set; }
            public HotelPrice HotelPrice { get; set; }
            public string RoomsRemaining { get; set; }
            public RoomAmenityList RoomAmenityList { get; set; }
            public Promotion Promotion { get; set; }
        }

        public class Location
        {
            public string StreetAddress { get; set; }
            public string City { get; set; }
            public string Province { get; set; }
            public string Country { get; set; }
            public GeoLocation GeoLocation { get; set; }
        }

        public class GeoLocation
        {
            public string Latitude { get; set; }
            public string Longitude { get; set; }
        }

        public class HotelPrice
        {
            public BaseRate1 BaseRate { get; set; }
            public TaxRcAndFees TaxRcAndFees { get; set; }
            public TotalRate1 TotalRate { get; set; }
        }

        public class BaseRate1
        {
            public string Value { get; set; }
            public string Currency { get; set; }
        }

        public class TaxRcAndFees
        {
            public string Value { get; set; }
            public string Currency { get; set; }
        }

        public class TotalRate1
        {
            public string Value { get; set; }
            public string Currency { get; set; }
        }

        public class RoomAmenityList
        {
            public object RoomAmenity { get; set; }
        }

        public class Promotion
        {
            public Amount Amount { get; set; }
            public string Description { get; set; }
        }

        public class Amount
        {
            public string Value { get; set; }
            public string Currency { get; set; }
        }
    }
}
