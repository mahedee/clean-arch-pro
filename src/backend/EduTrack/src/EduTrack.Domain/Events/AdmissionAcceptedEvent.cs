using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when an admission application is accepted
    /// </summary>
    public class AdmissionAcceptedEvent : DomainEvent
    {
        public Guid AdmissionId { get; }
        public string ApplicantName { get; }
        public string ApplicantEmail { get; }
        public string ProgramAppliedFor { get; }
        public string ReviewedBy { get; }

        public AdmissionAcceptedEvent(Guid admissionId, string applicantName, string applicantEmail, string programAppliedFor, string reviewedBy)
        {
            AdmissionId = admissionId;
            ApplicantName = applicantName;
            ApplicantEmail = applicantEmail;
            ProgramAppliedFor = programAppliedFor;
            ReviewedBy = reviewedBy;
        }
    }
}
