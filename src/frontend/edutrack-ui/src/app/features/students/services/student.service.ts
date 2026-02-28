import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  IStudent,
  IStudentCreateRequest,
  IStudentUpdateRequest,
  IStudentContactUpdate,
  IStudentGPAUpdate,
  IStudentStatusChange,
  IStudentListQuery,
  IPaginationResponse,
  StudentStatus
} from '../../../shared/interfaces';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private readonly apiUrl = `${environment.apiUrl}/Students`;

  constructor(private http: HttpClient) {}

  getStudents(query?: IStudentListQuery): Observable<IPaginationResponse<IStudent>> {
    let params = new HttpParams();
    
    if (query) {
      if (query.pageNumber) params = params.set('pageNumber', query.pageNumber.toString());
      if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
      if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
      if (query.sortBy) params = params.set('sortBy', query.sortBy);
      if (query.sortDirection) params = params.set('sortDirection', query.sortDirection);
      if (query.status) params = params.set('status', query.status);
    }

    return this.http.get<IPaginationResponse<IStudent>>(this.apiUrl, { params });
  }

  getStudent(id: string): Observable<IStudent> {
    return this.http.get<IStudent>(`${this.apiUrl}/${id}`);
  }

  createStudent(student: IStudentCreateRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, student);
  }

  updateStudent(id: string, student: IStudentUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, student);
  }

  deleteStudent(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  updateStudentContact(id: string, contactDto: IStudentContactUpdate): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/contact`, contactDto);
  }

  updateStudentGPA(id: string, gpaDto: IStudentGPAUpdate): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/gpa`, gpaDto);
  }

  changeStudentStatus(id: string, statusDto: IStudentStatusChange): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/status`, statusDto);
  }

  getStudentsByStatus(status: StudentStatus, query?: IStudentListQuery): Observable<IPaginationResponse<IStudent>> {
    let params = new HttpParams();
    
    if (query) {
      if (query.pageNumber) params = params.set('pageNumber', query.pageNumber.toString());
      if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
      if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
      if (query.sortBy) params = params.set('sortBy', query.sortBy);
      if (query.sortDirection) params = params.set('sortDirection', query.sortDirection);
    }

    return this.http.get<IPaginationResponse<IStudent>>(`${this.apiUrl}/status/${status}`, { params });
  }

  getStudentsOnProbation(query?: IStudentListQuery): Observable<IPaginationResponse<IStudent>> {
    let params = new HttpParams();
    
    if (query) {
      if (query.pageNumber) params = params.set('pageNumber', query.pageNumber.toString());
      if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
      if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
      if (query.sortBy) params = params.set('sortBy', query.sortBy);
      if (query.sortDirection) params = params.set('sortDirection', query.sortDirection);
    }

    return this.http.get<IPaginationResponse<IStudent>>(`${this.apiUrl}/probation`, { params });
  }
}