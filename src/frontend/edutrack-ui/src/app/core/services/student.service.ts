import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { 
  Student, 
  StudentDto,
  CreateStudentDto, 
  UpdateStudentDto, 
  UpdateStudentContactDto,
  UpdateGPADto,
  ChangeStatusDto,
  PaginatedStudentListDto, 
  GetStudentListQuery,
  StudentStatus
} from '../../models';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StudentService {
  private apiUrl = `${environment.apiUrl}/Students`;

  constructor(private http: HttpClient) {}

  getStudents(query?: GetStudentListQuery): Observable<PaginatedStudentListDto> {
    let params = new HttpParams();
    
    if (query) {
      if (query.pageNumber) params = params.set('pageNumber', query.pageNumber.toString());
      if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
      if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
      if (query.sortBy) params = params.set('sortBy', query.sortBy);
      if (query.sortDirection) params = params.set('sortDirection', query.sortDirection);
    }

    return this.http.get<PaginatedStudentListDto>(this.apiUrl, { params });
  }

  getStudent(id: string): Observable<StudentDto> {
    return this.http.get<StudentDto>(`${this.apiUrl}/${id}`);
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

  updateStudentContact(id: string, contactDto: UpdateStudentContactDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/contact`, contactDto);
  }

  updateStudentGPA(id: string, gpaDto: UpdateGPADto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/gpa`, gpaDto);
  }

  changeStudentStatus(id: string, statusDto: ChangeStatusDto): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}/status`, statusDto);
  }

  getStudentsByStatus(status: StudentStatus, pageNumber: number = 1, pageSize: number = 10): Observable<PaginatedStudentListDto> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PaginatedStudentListDto>(`${this.apiUrl}/status/${status}`, { params });
  }

  getStudentsOnProbation(pageNumber: number = 1, pageSize: number = 10): Observable<PaginatedStudentListDto> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<PaginatedStudentListDto>(`${this.apiUrl}/probation`, { params });
  }
}
