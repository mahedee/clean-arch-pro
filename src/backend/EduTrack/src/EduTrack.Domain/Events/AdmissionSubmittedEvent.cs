using EduTrack.Domain.Common;

namespace EduTrack.Domain.Events
{
    /// <summary>
    /// Domain event raised when an admission application is submitted
    /// </summary>
    public class AdmissionSubmittedEvent : DomainEvent
    {
        public Guid AdmissionId { get; }
        public string ApplicantName { get; }
        public string ApplicantEmail { get; }
        public string ProgramAppliedFor { get; }

        public AdmissionSubmittedEvent(Guid admissionId, string applicantName, string applicantEmail, string programAppliedFor)
        {
            AdmissionId = admissionId;
            ApplicantName = applicantName;
            ApplicantEmail = applicantEmail;
            ProgramAppliedFor = programAppliedFor;
        }
    }
}
