import { IStudent, ICourse } from './index';

/**
 * Enrollment related interfaces
 */

export interface IEnrollment {
  id: string;
  studentId: string;
  courseId: string;
  student: IStudent;
  course: ICourse;
  enrollmentDate: string;
  status: EnrollmentStatus;
  grade?: string;
  finalGrade?: number;
  credits: number;
  semester: string;
  year: number;
  createdAt: string;
  updatedAt: string;
}

export interface IEnrollmentCreateRequest {
  studentId: string;
  courseId: string;
  semester: string;
  year: number;
}

export interface IEnrollmentUpdateRequest {
  status?: EnrollmentStatus;
  grade?: string;
  finalGrade?: number;
}

export interface IEnrollmentListQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  studentId?: string;
  courseId?: string;
  semester?: string;
  year?: number;
  status?: EnrollmentStatus;
}

export enum EnrollmentStatus {
  Enrolled = 'Enrolled',
  Completed = 'Completed',
  Dropped = 'Dropped',
  Failed = 'Failed',
  Withdrawn = 'Withdrawn'
}

export type EnrollmentDto = IEnrollment;
export type CreateEnrollmentDto = IEnrollmentCreateRequest;
export type UpdateEnrollmentDto = IEnrollmentUpdateRequest;
export type GetEnrollmentListQuery = IEnrollmentListQuery;
