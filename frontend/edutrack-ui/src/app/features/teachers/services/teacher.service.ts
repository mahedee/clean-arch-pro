import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  ITeacher,
  ITeacherCreateRequest,
  ITeacherUpdateRequest,
  ITeacherListQuery,
  IPaginatedTeacherList
} from '../../../shared/interfaces';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TeacherService {
  private readonly apiUrl = `${environment.apiUrl}/Teachers`;

  constructor(private http: HttpClient) {}

  getTeachers(query?: ITeacherListQuery): Observable<IPaginatedTeacherList> {
    let params = new HttpParams();
    if (query) {
      if (query.pageNumber) params = params.set('pageNumber', query.pageNumber.toString());
      if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
      if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
      if (query.department) params = params.set('department', query.department);
      if (query.status) params = params.set('status', query.status);
      if (query.sortBy) params = params.set('sortBy', query.sortBy);
      if (query.sortDirection) params = params.set('sortDirection', query.sortDirection);
    }
    return this.http.get<IPaginatedTeacherList>(this.apiUrl, { params });
  }

  getTeacher(id: string): Observable<ITeacher> {
    return this.http.get<ITeacher>(`${this.apiUrl}/${id}`);
  }

  createTeacher(teacher: ITeacherCreateRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, teacher);
  }

  updateTeacher(id: string, teacher: ITeacherUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, teacher);
  }

  deleteTeacher(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
