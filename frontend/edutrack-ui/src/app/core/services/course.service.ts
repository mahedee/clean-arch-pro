import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ICourse,
  ICourseCreateRequest,
  ICourseUpdateRequest,
  ICourseListQuery,
  CourseStatus,
  CourseLevel
} from '../../shared/interfaces';
import { IPaginationResponse } from '../../shared/interfaces';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CourseService {
  private apiUrl = `${environment.apiUrl}/Courses`;

  constructor(private http: HttpClient) {}

  getCourses(query?: ICourseListQuery): Observable<IPaginationResponse<ICourse>> {
    let params = new HttpParams();
    
    // Set default values to ensure required parameters are always sent
    const finalQuery = {
      pageNumber: 1,
      pageSize: 10,
      sortBy: 'Title',
      sortDirection: 'asc',
      ...query
    };
    
    // Add all parameters
    params = params.set('pageNumber', finalQuery.pageNumber.toString());
    params = params.set('pageSize', finalQuery.pageSize.toString());
    params = params.set('sortBy', finalQuery.sortBy);
    params = params.set('sortDirection', finalQuery.sortDirection);
    
    // Add optional parameters only if they have values
    if (finalQuery.searchTerm) {
      params = params.set('searchTerm', finalQuery.searchTerm);
    }
    if (finalQuery.department) {
      params = params.set('department', finalQuery.department);
    }
    if (finalQuery.level) {
      params = params.set('level', finalQuery.level);
    }
    if (finalQuery.status) {
      params = params.set('status', finalQuery.status);
    }

    console.log('Course API call:', `${this.apiUrl}?${params.toString()}`);
    return this.http.get<IPaginationResponse<ICourse>>(this.apiUrl, { params });
  }

  getCourse(id: string): Observable<ICourse> {
    return this.http.get<ICourse>(`${this.apiUrl}/${id}`);
  }

  createCourse(command: ICourseCreateRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, command);
  }

  updateCourse(id: string, command: ICourseUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, command);
  }

  deleteCourse(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  scheduleCourse(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/schedule`, {});
  }

  activateCourse(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/activate`, {});
  }

  completeCourse(id: string): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/complete`, {});
  }

  getCoursesByDepartment(department: string, level?: CourseLevel, status?: CourseStatus): Observable<ICourse[]> {
    let params = new HttpParams();
    
    if (level) params = params.set('level', level);
    if (status) params = params.set('status', status);

    return this.http.get<ICourse[]>(`${this.apiUrl}/department/${department}`, { params });
  }

  // Utility methods for managing course data
  getCoursesByStatus(status: CourseStatus, pageNumber: number = 1, pageSize: number = 10): Observable<IPaginationResponse<ICourse>> {
    const query: ICourseListQuery = {
      status: status,
      pageNumber,
      pageSize
    };
    return this.getCourses(query);
  }

  searchCourses(searchTerm: string, pageNumber: number = 1, pageSize: number = 10): Observable<IPaginationResponse<ICourse>> {
    const query: ICourseListQuery = {
      searchTerm,
      pageNumber,
      pageSize
    };
    return this.getCourses(query);
  }
}