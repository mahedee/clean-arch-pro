import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  Student, 
  CreateStudentDto, 
  UpdateStudentDto, 
  PaginatedStudentList, 
  GetStudentListQuery 
} from '../../shared/models/student.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private apiUrl = `http://localhost:5152/api/Students`;

  constructor(private http: HttpClient) {}

  getStudents(query?: GetStudentListQuery): Observable<PaginatedStudentList> {
    let params = new HttpParams();
    
    if (query) {
      if (query.pageNumber) params = params.set('pageNumber', query.pageNumber.toString());
      if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
      if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
      if (query.status) params = params.set('status', query.status);
      if (query.sortBy) params = params.set('sortBy', query.sortBy);
      if (query.sortDirection) params = params.set('sortDirection', query.sortDirection);
      if (query.minGPA) params = params.set('minGPA', query.minGPA.toString());
      if (query.maxGPA) params = params.set('maxGPA', query.maxGPA.toString());
      if (query.minAge) params = params.set('minAge', query.minAge.toString());
      if (query.maxAge) params = params.set('maxAge', query.maxAge.toString());
    }

    return this.http.get<PaginatedStudentList>(this.apiUrl, { params });
  }

  getStudent(id: string): Observable<Student> {
    return this.http.get<Student>(`${this.apiUrl}/${id}`);
  }

  createStudent(student: CreateStudentDto): Observable<string> {
    return this.http.post<string>(this.apiUrl, student);
  }

  updateStudent(id: string, student: UpdateStudentDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, student);
  }

  deleteStudent(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  updateStudentContact(id: string, email: string, phoneNumber?: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/contact`, { email, phoneNumber });
  }

  updateStudentGPA(id: string, gpaValue: number): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/gpa`, { gpaValue });
  }

  changeStudentStatus(id: string, newStatus: string): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/status`, { newStatus });
  }
}
