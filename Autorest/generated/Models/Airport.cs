// <auto-generated>
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace RouteService.Models
{
    using Newtonsoft.Json;
    using System.Linq;

    public partial class Airport
    {
        /// <summary>
        /// Initializes a new instance of the Airport class.
        /// </summary>
        public Airport()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the Airport class.
        /// </summary>
        /// <param name="name">Name of airport. May or may not contain the City
        /// name.</param>
        /// <param name="alias">Alias of the airline. For example, All Nippon
        /// Airways is commonly known as "ANA".</param>
        /// <param name="city">Main city served by airport. May be spelled
        /// differently from Name.</param>
        /// <param name="country">Country or territory where airport is
        /// located. See countries.dat to cross-reference to ISO 3166-1
        /// codes.</param>
        /// <param name="latitude">Decimal degrees, usually to six significant
        /// digits. Negative is South, positive is North.</param>
        /// <param name="longitude">Decimal degrees, usually to six significant
        /// digits. Negative is West, positive is East.</param>
        /// <param name="altitude">In feet.</param>
        public Airport(string name = default(string), string alias = default(string), string city = default(string), string country = default(string), double? latitude = default(double?), double? longitude = default(double?), int? altitude = default(int?))
        {
            Name = name;
            Alias = alias;
            City = city;
            Country = country;
            Latitude = latitude;
            Longitude = longitude;
            Altitude = altitude;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// Gets or sets name of airport. May or may not contain the City name.
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets alias of the airline. For example, All Nippon Airways
        /// is commonly known as "ANA".
        /// </summary>
        [JsonProperty(PropertyName = "alias")]
        public string Alias { get; set; }

        /// <summary>
        /// Gets or sets main city served by airport. May be spelled
        /// differently from Name.
        /// </summary>
        [JsonProperty(PropertyName = "city")]
        public string City { get; set; }

        /// <summary>
        /// Gets or sets country or territory where airport is located. See
        /// countries.dat to cross-reference to ISO 3166-1 codes.
        /// </summary>
        [JsonProperty(PropertyName = "country")]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets decimal degrees, usually to six significant digits.
        /// Negative is South, positive is North.
        /// </summary>
        [JsonProperty(PropertyName = "latitude")]
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets decimal degrees, usually to six significant digits.
        /// Negative is West, positive is East.
        /// </summary>
        [JsonProperty(PropertyName = "longitude")]
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets in feet.
        /// </summary>
        [JsonProperty(PropertyName = "altitude")]
        public int? Altitude { get; set; }

    }
}
