// This file provides backward compatibility by re-exporting from the new interface structure
// New code should import directly from shared/interfaces instead

export {
  ICourse as Course,
  ICourse as CourseDto,
  ICourseCreateRequest as CreateCourseDto,
  ICourseUpdateRequest as UpdateCourseDto,
  ICourseListQuery as GetCourseListQuery,
  CourseStatus,
  CourseLevel
} from '../shared/interfaces';