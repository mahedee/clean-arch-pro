import { IAddress, IPaginationResponse } from './common.interface';

/**
 * Student related interfaces
 */

export interface IStudent {
  id: string;
  fullName: string;
  dateOfBirth: string;
  email: string;
  phoneNumber?: string | null;
  address?: IAddress | null;
  gpa?: number | null;
  status: StudentStatus;
  enrollmentDate: string;
  age?: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface IStudentCreateRequest {
  fullName: string;
  dateOfBirth: string;
  email: string;
  phoneNumber?: string;
  street?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  address?: IAddress;
}

export interface IStudentUpdateRequest {
  fullName: string;
  email: string;
  phoneNumber?: string;
  street?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  country?: string;
  address?: IAddress;
  gpa?: number;
}

export interface IStudentListQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  status?: StudentStatus;
}

export interface IStudentContactUpdate {
  email: string;
  phoneNumber?: string;
}

export interface IStudentGPAUpdate {
  gpaValue: number;
}

export interface IStudentStatusChange {
  newStatus: string;
}

export enum StudentStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Graduated = 'Graduated',
  Withdrawn = 'Withdrawn',
  OnProbation = 'OnProbation'
}

export type StudentDto = IStudent;
export type CreateStudentDto = IStudentCreateRequest;
export type UpdateStudentDto = IStudentUpdateRequest;
export type UpdateStudentContactDto = IStudentContactUpdate;
export type UpdateGPADto = IStudentGPAUpdate;
export type ChangeStatusDto = IStudentStatusChange;
export type GetStudentListQuery = IStudentListQuery;
export type PaginatedStudentListDto = IPaginationResponse<IStudent>;

// Re-export for backward compatibility
export { IStudent as Student };
