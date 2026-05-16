/**
 * Common interfaces used across the application
 */

export interface IAddress {
  street: string;
  street2?: string;
  city: string;
  state: string;
  zipCode: string;
  country: string;
}

export interface IPaginationRequest {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface IPaginationResponse<T> {
  students?: T[];  // For compatibility with existing API
  courses?: T[];   // For course API responses
  data?: T[];      // Generic data property
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface ISelectOption {
  value: string | number;
  label: string;
  disabled?: boolean;
}

export interface IFormValidationErrors {
  [key: string]: string[];
}
