namespace EduTrack.Domain.Enums
{
    /// <summary>
    /// Represents the academic level of a course
    /// </summary>
    public enum CourseLevel
    {
        /// <summary>
        /// Undergraduate level courses (Bachelor's degree programs)
        /// </summary>
        Undergraduate = 1,

        /// <summary>
        /// Graduate level courses (Master's degree programs)
        /// </summary>
        Graduate = 2,

        /// <summary>
        /// Postgraduate level courses (advanced Master's programs)
        /// </summary>
        Postgraduate = 3,

        /// <summary>
        /// Doctoral level courses (PhD and other doctoral programs)
        /// </summary>
        Doctoral = 4,

        /// <summary>
        /// Certificate courses and professional development
        /// </summary>
        Certificate = 5,

        /// <summary>
        /// Continuing education and lifelong learning courses
        /// </summary>
        Continuing = 6
    }
}