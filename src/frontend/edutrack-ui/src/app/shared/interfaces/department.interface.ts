/**
 * Department related interfaces
 */

export interface IDepartment {
  id: string;
  name: string;
  code: string;
  description: string;
  status: string;
  location?: string | null;
  contactEmail?: string | null;
  contactPhone?: string | null;
  departmentHeadId?: string | null;
  facultyCount: number;
  studentCount: number;
  budget?: number | null;
  establishedDate?: string | null;
  createdAt?: string;
  updatedAt?: string;
}

export interface IDepartmentListItem {
  id: string;
  name: string;
  code: string;
  status: string;
  location?: string | null;
  facultyCount: number;
  studentCount: number;
}

export interface IDepartmentCreateRequest {
  name: string;
  code: string;
  description?: string;
  location?: string;
  contactEmail?: string;
  contactPhone?: string;
}

export interface IDepartmentUpdateRequest {
  name?: string;
  description?: string;
  location?: string;
  contactEmail?: string;
  contactPhone?: string;
}

export interface IDepartmentListQuery {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  status?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface IPaginatedDepartmentList {
  departments: IDepartmentListItem[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export enum DepartmentStatus {
  Active = 'Active',
  Inactive = 'Inactive',
  Merging = 'Merging',
  Dissolved = 'Dissolved'
}
