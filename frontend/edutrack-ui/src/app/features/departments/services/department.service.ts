import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  IDepartment,
  IDepartmentCreateRequest,
  IDepartmentUpdateRequest,
  IDepartmentListQuery,
  IPaginatedDepartmentList
} from '../../../shared/interfaces';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  private readonly apiUrl = `${environment.apiUrl}/Departments`;

  constructor(private http: HttpClient) {}

  getDepartments(query?: IDepartmentListQuery): Observable<IPaginatedDepartmentList> {
    let params = new HttpParams();
    if (query) {
      if (query.pageNumber) params = params.set('pageNumber', query.pageNumber.toString());
      if (query.pageSize) params = params.set('pageSize', query.pageSize.toString());
      if (query.searchTerm) params = params.set('searchTerm', query.searchTerm);
      if (query.status) params = params.set('status', query.status);
      if (query.sortBy) params = params.set('sortBy', query.sortBy);
      if (query.sortDirection) params = params.set('sortDirection', query.sortDirection);
    }
    return this.http.get<IPaginatedDepartmentList>(this.apiUrl, { params });
  }

  getDepartment(id: string): Observable<IDepartment> {
    return this.http.get<IDepartment>(`${this.apiUrl}/${id}`);
  }

  createDepartment(department: IDepartmentCreateRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, department);
  }

  updateDepartment(id: string, department: IDepartmentUpdateRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, department);
  }

  deleteDepartment(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
