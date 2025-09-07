export interface Address {
  street?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
}

export interface Student {
  id: string;
  fullName: string;
  email: string;
  age: number;
  phoneNumber: string;
  gpa?: string;
  status: StudentStatus;
  enrollmentDate: string;
  dateOfBirth?: string;
  address?: Address;
}

export interface CreateStudentDto {
  fullName: string;
  dateOfBirth: string;
  email: string;
  phoneNumber: string;
  address?: Address;
}

export interface UpdateStudentDto {
  fullName: string;
  email: string;
  phoneNumber: string;
  gpa?: number;
  address?: Address;
}

export interface PaginatedStudentList {
  students: Student[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface GetStudentListQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: StudentStatus;
  sortBy?: string;
  sortDirection?: string;
  minGPA?: number;
  maxGPA?: number;
  minAge?: number;
  maxAge?: number;
}

export enum StudentStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Graduated = 'Graduated',
  Suspended = 'Suspended'
}
