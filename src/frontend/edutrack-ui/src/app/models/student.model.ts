// This file provides backward compatibility by re-exporting from the new interface structure
// New code should import directly from shared/interfaces instead

export {
  IStudent as Student,
  IStudent as StudentDto,
  IStudentCreateRequest as CreateStudentDto,
  IStudentUpdateRequest as UpdateStudentDto,
  IStudentContactUpdate as UpdateStudentContactDto,
  IStudentGPAUpdate as UpdateGPADto,
  IStudentStatusChange as ChangeStatusDto,
  IStudentListQuery as GetStudentListQuery,
  IPaginationResponse as PaginatedStudentListDto,
  StudentStatus,
  IAddress as Address
} from '../shared/interfaces';