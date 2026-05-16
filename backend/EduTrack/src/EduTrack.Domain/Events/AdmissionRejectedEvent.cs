using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when an admission application is rejected
    /// </summary>
    public class AdmissionRejectedEvent : DomainEvent
    {
        public Guid AdmissionId { get; }
        public string ApplicantName { get; }
        public string ApplicantEmail { get; }
        public string? RejectionReason { get; }

        public AdmissionRejectedEvent(Guid admissionId, string applicantName, string applicantEmail, string? rejectionReason)
        {
            AdmissionId = admissionId;
            ApplicantName = applicantName;
            ApplicantEmail = applicantEmail;
            RejectionReason = rejectionReason;
        }
    }
}
