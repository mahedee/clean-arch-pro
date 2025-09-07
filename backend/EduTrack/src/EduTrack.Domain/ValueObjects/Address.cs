using System.Text.RegularExpressions;

namespace EduTrack.Domain.ValueObjects
{
    /// <summary>
    /// Address value object that encapsulates address validation and business logic
    /// </summary>
    public sealed class Address : IEquatable<Address>
    {
        private static readonly Regex ZipCodePattern = new(
            @"^\d{5}(-\d{4})?$",
            RegexOptions.Compiled);

        private static readonly Regex StatePattern = new(
            @"^[A-Z]{2}$",
            RegexOptions.Compiled);

        /// <summary>
        /// Street address line 1
        /// </summary>
        public string Street { get; private set; }

        /// <summary>
        /// Street address line 2 (optional)
        /// </summary>
        public string? Street2 { get; private set; }

        /// <summary>
        /// City name
        /// </summary>
        public string City { get; private set; }

        /// <summary>
        /// State abbreviation (2 letters)
        /// </summary>
        public string State { get; private set; }

        /// <summary>
        /// ZIP code (5 or 9 digits)
        /// </summary>
        public string ZipCode { get; private set; }

        /// <summary>
        /// Country (default to US)
        /// </summary>
        public string Country { get; private set; }

        /// <summary>
        /// Parameterless constructor for Entity Framework
        /// </summary>
        private Address()
        {
            Street = string.Empty;
            City = string.Empty;
            State = string.Empty;
            ZipCode = string.Empty;
            Country = string.Empty;
        }

        /// <summary>
        /// Private constructor to enforce factory method usage
        /// </summary>
        private Address(string street, string city, string state, string zipCode, string country, string? street2 = null)
        {
            Street = street;
            Street2 = street2;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
        }

        /// <summary>
        /// Factory method to create a new Address instance with validation
        /// </summary>
        public static Address Create(string street, string city, string state, string zipCode, string country = "US", string? street2 = null)
        {
            // Validate street
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("Street address cannot be empty", nameof(street));
            
            if (street.Length > 100)
                throw new ArgumentException("Street address cannot exceed 100 characters", nameof(street));

            // Validate street2 if provided
            if (!string.IsNullOrWhiteSpace(street2) && street2.Length > 100)
                throw new ArgumentException("Street address line 2 cannot exceed 100 characters", nameof(street2));

            // Validate city
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty", nameof(city));
            
            if (city.Length > 50)
                throw new ArgumentException("City cannot exceed 50 characters", nameof(city));

            // Validate state
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("State cannot be empty", nameof(state));

            state = state.ToUpperInvariant().Trim();
            if (!StatePattern.IsMatch(state))
                throw new ArgumentException("State must be a valid 2-letter abbreviation", nameof(state));

            // Validate ZIP code
            if (string.IsNullOrWhiteSpace(zipCode))
                throw new ArgumentException("ZIP code cannot be empty", nameof(zipCode));

            zipCode = zipCode.Trim();
            if (!ZipCodePattern.IsMatch(zipCode))
                throw new ArgumentException("ZIP code must be in format 12345 or 12345-6789", nameof(zipCode));

            // Validate country
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country cannot be empty", nameof(country));

            // Normalize inputs
            street = NormalizeText(street);
            street2 = string.IsNullOrWhiteSpace(street2) ? null : NormalizeText(street2);
            city = NormalizeText(city);
            country = country.ToUpperInvariant().Trim();

            return new Address(street, city, state, zipCode, country, street2);
        }

        /// <summary>
        /// Normalizes text by trimming and proper casing
        /// </summary>
        private static string NormalizeText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

            // Trim and remove extra spaces
            text = Regex.Replace(text.Trim(), @"\s+", " ");

            // Convert to title case
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLowerInvariant());
        }

        /// <summary>
        /// Gets the full address as a single formatted string
        /// </summary>
        public string GetFullAddress()
        {
            var address = Street;
            if (!string.IsNullOrWhiteSpace(Street2))
                address += $", {Street2}";
            
            address += $", {City}, {State} {ZipCode}";
            
            if (Country != "US")
                address += $", {Country}";

            return address;
        }

        /// <summary>
        /// Gets the address formatted for mailing labels
        /// </summary>
        public string[] GetMailingFormat()
        {
            var lines = new List<string> { Street };
            
            if (!string.IsNullOrWhiteSpace(Street2))
                lines.Add(Street2);
            
            lines.Add($"{City}, {State} {ZipCode}");
            
            if (Country != "US")
                lines.Add(Country);

            return lines.ToArray();
        }

        /// <summary>
        /// Gets just the first 5 digits of the ZIP code
        /// </summary>
        public string GetZip5() => ZipCode.Length > 5 ? ZipCode[..5] : ZipCode;

        /// <summary>
        /// Gets the ZIP+4 extension if available
        /// </summary>
        public string? GetZipExtension() => ZipCode.Length > 5 ? ZipCode[6..] : null;

        /// <summary>
        /// Checks if this is a PO Box address
        /// </summary>
        public bool IsPOBox() => Street.StartsWith("PO Box", StringComparison.OrdinalIgnoreCase) ||
                                 Street.StartsWith("P.O. Box", StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets the state name from abbreviation
        /// </summary>
        public string GetStateName()
        {
            var stateNames = new Dictionary<string, string>
            {
                ["AL"] = "Alabama", ["AK"] = "Alaska", ["AZ"] = "Arizona", ["AR"] = "Arkansas",
                ["CA"] = "California", ["CO"] = "Colorado", ["CT"] = "Connecticut", ["DE"] = "Delaware",
                ["FL"] = "Florida", ["GA"] = "Georgia", ["HI"] = "Hawaii", ["ID"] = "Idaho",
                ["IL"] = "Illinois", ["IN"] = "Indiana", ["IA"] = "Iowa", ["KS"] = "Kansas",
                ["KY"] = "Kentucky", ["LA"] = "Louisiana", ["ME"] = "Maine", ["MD"] = "Maryland",
                ["MA"] = "Massachusetts", ["MI"] = "Michigan", ["MN"] = "Minnesota", ["MS"] = "Mississippi",
                ["MO"] = "Missouri", ["MT"] = "Montana", ["NE"] = "Nebraska", ["NV"] = "Nevada",
                ["NH"] = "New Hampshire", ["NJ"] = "New Jersey", ["NM"] = "New Mexico", ["NY"] = "New York",
                ["NC"] = "North Carolina", ["ND"] = "North Dakota", ["OH"] = "Ohio", ["OK"] = "Oklahoma",
                ["OR"] = "Oregon", ["PA"] = "Pennsylvania", ["RI"] = "Rhode Island", ["SC"] = "South Carolina",
                ["SD"] = "South Dakota", ["TN"] = "Tennessee", ["TX"] = "Texas", ["UT"] = "Utah",
                ["VT"] = "Vermont", ["VA"] = "Virginia", ["WA"] = "Washington", ["WV"] = "West Virginia",
                ["WI"] = "Wisconsin", ["WY"] = "Wyoming", ["DC"] = "District of Columbia"
            };

            return stateNames.TryGetValue(State, out var stateName) ? stateName : State;
        }

        /// <summary>
        /// Creates a new address with updated street information
        /// </summary>
        public Address WithStreet(string newStreet, string? newStreet2 = null)
        {
            return Create(newStreet, City, State, ZipCode, Country, newStreet2);
        }

        /// <summary>
        /// Creates a new address with updated city
        /// </summary>
        public Address WithCity(string newCity)
        {
            return Create(Street, newCity, State, ZipCode, Country, Street2);
        }

        /// <summary>
        /// Creates a new address with updated state
        /// </summary>
        public Address WithState(string newState)
        {
            return Create(Street, City, newState, ZipCode, Country, Street2);
        }

        /// <summary>
        /// Creates a new address with updated ZIP code
        /// </summary>
        public Address WithZipCode(string newZipCode)
        {
            return Create(Street, City, State, newZipCode, Country, Street2);
        }

        /// <summary>
        /// Value equality comparison
        /// </summary>
        public bool Equals(Address? other)
        {
            return other is not null &&
                   Street.Equals(other.Street, StringComparison.OrdinalIgnoreCase) &&
                   (Street2?.Equals(other.Street2, StringComparison.OrdinalIgnoreCase) ?? other.Street2 is null) &&
                   City.Equals(other.City, StringComparison.OrdinalIgnoreCase) &&
                   State.Equals(other.State, StringComparison.OrdinalIgnoreCase) &&
                   ZipCode == other.ZipCode &&
                   Country.Equals(other.Country, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Object equality comparison
        /// </summary>
        public override bool Equals(object? obj) => Equals(obj as Address);

        /// <summary>
        /// Gets hash code for the address
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(
                Street.ToLowerInvariant(),
                Street2?.ToLowerInvariant(),
                City.ToLowerInvariant(),
                State,
                ZipCode,
                Country.ToLowerInvariant());
        }

        /// <summary>
        /// String representation of the address
        /// </summary>
        public override string ToString() => GetFullAddress();

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(Address? left, Address? right) => Equals(left, right);

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(Address? left, Address? right) => !Equals(left, right);
    }
}
