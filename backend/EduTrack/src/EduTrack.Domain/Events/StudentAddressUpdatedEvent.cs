using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student's address is updated
    /// </summary>
    public class StudentAddressUpdatedEvent : DomainEvent
    {
        public Guid StudentId { get; }
        public string FullName { get; }
        public string Email { get; }
        public string? PreviousAddress { get; }
        public string NewAddress { get; }
        public string Street { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }
        public string Country { get; }

        public StudentAddressUpdatedEvent(
            Guid studentId,
            string fullName,
            string email,
            string? previousAddress,
            string street,
            string city,
            string state,
            string zipCode,
            string country)
        {
            StudentId = studentId;
            FullName = fullName;
            Email = email;
            PreviousAddress = previousAddress;
            Street = street;
            City = city;
            State = state;
            ZipCode = zipCode;
            Country = country;
            NewAddress = $"{street}, {city}, {state} {zipCode}, {country}";
        }
    }
}
