/**
 * API related interfaces for HTTP responses and errors
 */

export interface IApiResponse<T = any> {
  data: T;
  success: boolean;
  message?: string;
  errors?: string[];
  timestamp: string;
}

export interface IApiError {
  type: string;
  title: string;
  status: number;
  detail: string;
  instance: string;
  traceId: string;
  correlationId: string;
  timestamp: string;
  errorCode: string;
  context?: {
    StackTrace?: string;
    ExceptionType?: string;
    [key: string]: any;
  };
  errors?: any;
}

export interface IApiValidationError {
  [field: string]: string[];
}

export interface IHttpErrorResponse {
  error: IApiError;
  headers: any;
  message: string;
  name: string;
  ok: boolean;
  status: number;
  statusText: string;
  url: string;
}

// HTTP Status Codes
export enum HttpStatusCode {
  OK = 200,
  Created = 201,
  NoContent = 204,
  BadRequest = 400,
  Unauthorized = 401,
  Forbidden = 403,
  NotFound = 404,
  MethodNotAllowed = 405,
  Conflict = 409,
  InternalServerError = 500
}

// Common API endpoints
export interface IApiEndpoints {
  students: string;
  courses: string;
  enrollments: string;
  health: string;
}
