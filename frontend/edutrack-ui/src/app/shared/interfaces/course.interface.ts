/**
 * Course related interfaces
 */

export interface ICourse {
  id: string;
  code: string;  // API returns "code"
  title: string;
  description?: string;
  creditHours: number;  // API returns "creditHours"
  department: string;
  level: string;
  status: string;
  enrollment?: string;
  academicPeriod?: string;
  maxCapacity?: number;
  createdAt?: string;
  updatedAt?: string;
}

export interface ICourseCreateRequest {
  title: string;
  description: string;
  courseCode: string;
  credits: number;
  maxCapacity: number;
  department: string;
  level: string;
  academicPeriod: string;
  prerequisites?: string[];
}

export interface ICourseUpdateRequest {
  title: string;
  description: string;
  courseCode: string;
  credits: number;
  maxCapacity: number;
  department: string;
  level: string; // Backend expects string, not enum
  prerequisiteCreditHours: number;
}

export interface ICourseListQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  department?: string;
  level?: CourseLevel;
  status?: CourseStatus;
}

export enum CourseLevel {
  Undergraduate = 'Undergraduate',
  Graduate = 'Graduate',
  Doctoral = 'Doctoral'
}

export enum CourseStatus {
  Draft = 'Draft',
  Scheduled = 'Scheduled',
  Active = 'Active',
  Completed = 'Completed',
  Cancelled = 'Cancelled'
}

export type CourseDto = ICourse;
export type CreateCourseDto = ICourseCreateRequest;
export type UpdateCourseDto = ICourseUpdateRequest;
export type GetCourseListQuery = ICourseListQuery;
