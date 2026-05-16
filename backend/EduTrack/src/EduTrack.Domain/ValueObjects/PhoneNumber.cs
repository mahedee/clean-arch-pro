using System.Text.RegularExpressions;

namespace EduTrack.Domain.ValueObjects
{
    /// <summary>
    /// PhoneNumber value object that encapsulates phone number validation and business logic
    /// </summary>
    public sealed class PhoneNumber : IEquatable<PhoneNumber>
    {
        private static readonly Regex PhonePattern = new(
            @"^\+?1?[-.\s]?\(?([0-9]{3})\)?[-.\s]?([0-9]{3})[-.\s]?([0-9]{4})$",
            RegexOptions.Compiled);

        /// <summary>
        /// The formatted phone number
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Gets the area code (first 3 digits)
        /// </summary>
        public string AreaCode { get; }

        /// <summary>
        /// Gets the exchange code (middle 3 digits)
        /// </summary>
        public string ExchangeCode { get; }

        /// <summary>
        /// Gets the subscriber number (last 4 digits)
        /// </summary>
        public string SubscriberNumber { get; }

        /// <summary>
        /// Gets the country code (default to +1 for US)
        /// </summary>
        public string CountryCode { get; }

        /// <summary>
        /// Private constructor to enforce factory method usage
        /// </summary>
        private PhoneNumber(string value, string areaCode, string exchangeCode, string subscriberNumber, string countryCode = "+1")
        {
            Value = value;
            AreaCode = areaCode;
            ExchangeCode = exchangeCode;
            SubscriberNumber = subscriberNumber;
            CountryCode = countryCode;
        }

        /// <summary>
        /// Factory method to create a new PhoneNumber instance with validation
        /// </summary>
        /// <param name="phoneNumber">The phone number string to validate</param>
        /// <returns>A valid PhoneNumber instance</returns>
        /// <exception cref="ArgumentException">Thrown when phone number format is invalid</exception>
        public static PhoneNumber Create(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number cannot be empty or whitespace", nameof(phoneNumber));

            // Remove common formatting characters for parsing
            var cleanNumber = Regex.Replace(phoneNumber, @"[^\d+]", "");

            // Handle different formats
            string areaCode, exchangeCode, subscriberNumber, countryCode = "+1";

            if (cleanNumber.StartsWith("+1"))
            {
                cleanNumber = cleanNumber[2..]; // Remove +1
            }
            else if (cleanNumber.StartsWith("1") && cleanNumber.Length == 11)
            {
                cleanNumber = cleanNumber[1..]; // Remove leading 1
            }

            if (cleanNumber.Length != 10)
                throw new ArgumentException("Phone number must be 10 digits (US format)", nameof(phoneNumber));

            areaCode = cleanNumber[..3];
            exchangeCode = cleanNumber[3..6];
            subscriberNumber = cleanNumber[6..10];

            // Validate area code (cannot start with 0 or 1)
            if (areaCode[0] == '0' || areaCode[0] == '1')
                throw new ArgumentException("Invalid area code. Area codes cannot start with 0 or 1", nameof(phoneNumber));

            // Validate exchange code (cannot start with 0 or 1)
            if (exchangeCode[0] == '0' || exchangeCode[0] == '1')
                throw new ArgumentException("Invalid exchange code. Exchange codes cannot start with 0 or 1", nameof(phoneNumber));

            // Format as (XXX) XXX-XXXX
            var formattedNumber = $"({areaCode}) {exchangeCode}-{subscriberNumber}";

            return new PhoneNumber(formattedNumber, areaCode, exchangeCode, subscriberNumber, countryCode);
        }

        /// <summary>
        /// Factory method to create PhoneNumber from separate parts
        /// </summary>
        public static PhoneNumber Create(string areaCode, string exchangeCode, string subscriberNumber)
        {
            var phoneNumber = $"{areaCode}{exchangeCode}{subscriberNumber}";
            return Create(phoneNumber);
        }

        /// <summary>
        /// Gets the phone number in E.164 international format
        /// </summary>
        public string ToE164Format() => $"{CountryCode}{AreaCode}{ExchangeCode}{SubscriberNumber}";

        /// <summary>
        /// Gets the phone number in national format without formatting
        /// </summary>
        public string ToNationalFormat() => $"{AreaCode}{ExchangeCode}{SubscriberNumber}";

        /// <summary>
        /// Gets the phone number in dots format (XXX.XXX.XXXX)
        /// </summary>
        public string ToDotFormat() => $"{AreaCode}.{ExchangeCode}.{SubscriberNumber}";

        /// <summary>
        /// Gets the phone number in dashes format (XXX-XXX-XXXX)
        /// </summary>
        public string ToDashFormat() => $"{AreaCode}-{ExchangeCode}-{SubscriberNumber}";

        /// <summary>
        /// Gets the phone number for SMS/calling applications
        /// </summary>
        public string ToCallableFormat() => ToE164Format();

        /// <summary>
        /// Checks if this is a toll-free number
        /// </summary>
        public bool IsTollFree()
        {
            var tollFreeAreaCodes = new[] { "800", "833", "844", "855", "866", "877", "888" };
            return tollFreeAreaCodes.Contains(AreaCode);
        }

        /// <summary>
        /// Checks if this appears to be a mobile number (rough heuristic)
        /// </summary>
        public bool IsMobile()
        {
            // This is a very basic heuristic - in reality, mobile vs landline 
            // determination requires carrier database lookup
            var mobileAreaCodes = new[] { "201", "202", "203", "205", "206", "207", "208", "209", "210" };
            return mobileAreaCodes.Contains(AreaCode);
        }

        /// <summary>
        /// Gets the geographic region for the area code (basic mapping)
        /// </summary>
        public string GetRegion()
        {
            return AreaCode switch
            {
                var code when code.StartsWith("2") => "Eastern US",
                var code when code.StartsWith("3") => "Eastern US",
                var code when code.StartsWith("4") => "Southeast US",
                var code when code.StartsWith("5") => "Central US",
                var code when code.StartsWith("6") => "Central US",
                var code when code.StartsWith("7") => "Western US",
                var code when code.StartsWith("8") => "Toll-Free",
                var code when code.StartsWith("9") => "Western US",
                _ => "Unknown"
            };
        }

        /// <summary>
        /// Creates a masked version for display (e.g., (XXX) XXX-1234)
        /// </summary>
        public string ToMaskedFormat(int visibleDigits = 4)
        {
            if (visibleDigits < 0 || visibleDigits > 10)
                throw new ArgumentException("Visible digits must be between 0 and 10", nameof(visibleDigits));

            var fullNumber = ToNationalFormat();
            var maskedPart = new string('X', 10 - visibleDigits);
            var visiblePart = fullNumber[^visibleDigits..];
            var masked = maskedPart + visiblePart;

            return $"({masked[..3]}) {masked[3..6]}-{masked[6..10]}";
        }

        /// <summary>
        /// Value equality comparison
        /// </summary>
        public bool Equals(PhoneNumber? other)
        {
            return other is not null && ToNationalFormat() == other.ToNationalFormat();
        }

        /// <summary>
        /// Object equality comparison
        /// </summary>
        public override bool Equals(object? obj) => Equals(obj as PhoneNumber);

        /// <summary>
        /// Gets hash code for the phone number value
        /// </summary>
        public override int GetHashCode() => ToNationalFormat().GetHashCode();

        /// <summary>
        /// String representation of the phone number
        /// </summary>
        public override string ToString() => Value;

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(PhoneNumber? left, PhoneNumber? right) => Equals(left, right);

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(PhoneNumber? left, PhoneNumber? right) => !Equals(left, right);

        /// <summary>
        /// Implicit conversion to string for convenience
        /// </summary>
        public static implicit operator string(PhoneNumber phoneNumber) => phoneNumber.Value;
    }
}
