/**
 * Teacher related interfaces
 */

export interface ITeacher {
  id: string;
  fullName: string;
  email: string;
  phoneNumber?: string | null;
  employeeId: string;
  department: string;
  title: string;
  status: string;
  hireDate: string;
  dateOfBirth: string;
  specializations: string[];
  qualifications: string[];
  maxCoursesPerSemester: number;
  currentCourseLoad: number;
  officeLocation?: string | null;
  officeHours?: string | null;
  createdAt?: string;
  updatedAt?: string;
}

export interface ITeacherListItem {
  id: string;
  fullName: string;
  email: string;
  employeeId: string;
  department: string;
  title: string;
  status: string;
  currentCourseLoad: number;
}

export interface ITeacherCreateRequest {
  fullName: string;
  email: string;
  phoneNumber?: string;
  employeeId: string;
  department: string;
  title: string;
  dateOfBirth: string;
}

export interface ITeacherUpdateRequest {
  fullName?: string;
  email?: string;
  phoneNumber?: string;
  department?: string;
  title?: string;
  officeLocation?: string;
  officeHours?: string;
}

export interface ITeacherListQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  department?: string;
  status?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface IPaginatedTeacherList {
  teachers: ITeacherListItem[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export enum TeacherStatus {
  Active = 'Active',
  OnLeave = 'OnLeave',
  Inactive = 'Inactive',
  Terminated = 'Terminated'
}

export enum AcademicTitle {
  Lecturer = 'Lecturer',
  AssistantProfessor = 'AssistantProfessor',
  AssociateProfessor = 'AssociateProfessor',
  Professor = 'Professor',
  AdjunctProfessor = 'AdjunctProfessor',
  VisitingProfessor = 'VisitingProfessor'
}
