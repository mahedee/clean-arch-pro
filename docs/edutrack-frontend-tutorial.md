# EduTrack Frontend Tutorial - Step by Step Guide

## 📋 Overview
This tutorial provides a complete step-by-step guide to create the EduTrack Angular frontend application from scratch. Follow these instructions to recreate the `edutrack-ui` application with all its components and configurations.

---

## 🎯 Prerequisites

### **System Requirements:**
- **Node.js**: Version 18.18.2 or higher
- **npm**: Version 9.8.1 or higher
- **Angular CLI**: Version 17.x (compatible with Node.js 18)
- **PowerShell**: For Windows command execution

### **Verify Prerequisites:**
```powershell
# Check Node.js and npm versions
node --version
npm --version

# Should output:
# v18.18.2
# 9.8.1
```

---

## 📁 Step 1: Project Setup and Navigation

### **1.1 Navigate to Frontend Directory**
```powershell
# From project root directory
cd frontend
```

### **1.2 Install Angular CLI (Compatible Version)**
```powershell
# Install Angular CLI version 17 (compatible with Node.js 18)
npm install -g @angular/cli@17

# Verify installation
npx @angular/cli@17 version
```

**Expected Output:**
```
Angular CLI: 17.3.17
Node: 18.18.2
Package Manager: npm 9.8.1
```

---

## 🚀 Step 2: Create Angular Project

### **2.1 Generate New Angular Project**
```powershell
# Create new Angular project with routing and SCSS
npx @angular/cli@17 new edutrack-ui --routing --style=scss --package-manager=npm
```

**Interactive Prompts:**
- **Server-Side Rendering (SSR)**: Choose `Yes`
- **Static Site Generation**: Automatically enabled with SSR

### **2.2 Navigate to Project Directory**
```powershell
cd edutrack-ui
```

---

## 🎨 Step 3: Install Angular Material

### **3.1 Add Angular Material**
```powershell
npx ng add @angular/material
```

**Interactive Prompts:**
- **Share usage data**: Choose `No`
- **Prebuilt theme**: Choose `Indigo/Pink`
- **Global typography styles**: Choose `No`
- **Angular animations**: Choose `Include and enable animations`

### **3.2 Install Additional Dependencies**
```powershell
# Install compatible NgRx and Angular CDK
npm install @angular/cdk @ngrx/store@17 @ngrx/effects@17 @ngrx/store-devtools@17
```

---

## 🗂️ Step 4: Create Project Structure

### **4.1 Create Core Directories**
```powershell
# Create essential folder structure
New-Item -ItemType Directory -Path "src/app/core", "src/app/shared", "src/app/features", "src/app/layout" -Force
```

**Directory Structure Created:**
```
src/app/
├── core/          # Core services and utilities
├── shared/        # Shared components and modules
├── features/      # Feature modules
└── layout/        # Layout components
```

---

## ⚙️ Step 5: Environment Configuration

### **5.1 Create Development Environment**
```typescript
// src/environments/environment.ts
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7071/api',
  appName: 'EduTrack UI',
  version: '1.0.0'
};
```

### **5.2 Create Production Environment**
```typescript
// src/environments/environment.prod.ts
export const environment = {
  production: true,
  apiUrl: 'https://api.edutrack.com/api',
  appName: 'EduTrack UI',
  version: '1.0.0'
};
```

---

## 🧩 Step 6: Core Module Setup

### **6.1 Create Core Module**
```typescript
// src/app/core/core.module.ts
import { NgModule, Optional, SkipSelf } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [],
  imports: [
    CommonModule,
    HttpClientModule
  ],
  providers: [
    // Core services will be added here
  ]
})
export class CoreModule {
  constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
    if (parentModule) {
      throw new Error('CoreModule is already loaded. Import it in the AppModule only');
    }
  }
}
```

### **6.2 Create Material Module**
```typescript
// src/app/shared/material.module.ts
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatMenuModule } from '@angular/material/menu';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatSortModule } from '@angular/material/sort';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';

const MaterialComponents = [
  MatButtonModule,
  MatCardModule,
  MatIconModule,
  MatInputModule,
  MatFormFieldModule,
  MatToolbarModule,
  MatSidenavModule,
  MatListModule,
  MatMenuModule,
  MatTableModule,
  MatPaginatorModule,
  MatSortModule,
  MatProgressSpinnerModule,
  MatDialogModule,
  MatSnackBarModule,
  MatSelectModule,
  MatCheckboxModule,
  MatDatepickerModule,
  MatNativeDateModule
];

@NgModule({
  imports: MaterialComponents,
  exports: MaterialComponents
})
export class MaterialModule { }
```

### **6.3 Create Shared Module**
```typescript
// src/app/shared/shared.module.ts
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { MaterialModule } from './material.module';

@NgModule({
  declarations: [
    // Shared components will be added here
  ],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MaterialModule
  ],
  exports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    MaterialModule
    // Shared components will be exported here
  ]
})
export class SharedModule { }
```

---

## 🔐 Step 7: Authentication Service

### **7.1 Create Authentication Service**
```typescript
// src/app/core/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from '../../../environments/environment';

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  user: User;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject: BehaviorSubject<User | null>;
  public currentUser: Observable<User | null>;

  constructor(private http: HttpClient) {
    this.currentUserSubject = new BehaviorSubject<User | null>(
      JSON.parse(localStorage.getItem('currentUser') || 'null')
    );
    this.currentUser = this.currentUserSubject.asObservable();
  }

  public get currentUserValue(): User | null {
    return this.currentUserSubject.value;
  }

  login(loginRequest: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, loginRequest)
      .pipe(map(response => {
        // Store user details and jwt token in local storage
        localStorage.setItem('currentUser', JSON.stringify(response.user));
        localStorage.setItem('token', response.token);
        localStorage.setItem('refreshToken', response.refreshToken);
        this.currentUserSubject.next(response.user);
        return response;
      }));
  }

  logout(): void {
    // Remove user from local storage
    localStorage.removeItem('currentUser');
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    this.currentUserSubject.next(null);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  isAuthenticated(): boolean {
    return !!this.getToken();
  }
}
```

### **7.2 Create HTTP Interceptor**
```typescript
// src/app/core/interceptors/auth.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  constructor(private authService: AuthService) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Add authorization header with jwt token if available
    const token = this.authService.getToken();
    if (token) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`
        }
      });
    }

    return next.handle(request);
  }
}
```

---

## 🏗️ Step 8: Layout Components

### **8.1 Generate Main Layout Component**
```powershell
npx ng generate component layout/main-layout
```

### **8.2 Update Main Layout TypeScript**
```typescript
// src/app/layout/main-layout/main-layout.component.ts
import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MaterialModule } from '../../shared/material.module';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [RouterOutlet, MaterialModule],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.scss'
})
export class MainLayoutComponent {
}
```

### **8.3 Update Main Layout HTML**
```html
<!-- src/app/layout/main-layout/main-layout.component.html -->
<mat-toolbar color="primary">
  <button mat-icon-button (click)="sidenav.toggle()">
    <mat-icon>menu</mat-icon>
  </button>
  <span>EduTrack</span>
  <span class="spacer"></span>
  <button mat-icon-button>
    <mat-icon>notifications</mat-icon>
  </button>
  <button mat-icon-button>
    <mat-icon>account_circle</mat-icon>
  </button>
</mat-toolbar>

<mat-sidenav-container class="sidenav-container">
  <mat-sidenav #sidenav class="sidenav" mode="side" opened="true">
    <mat-nav-list>
      <a mat-list-item routerLink="/dashboard">
        <mat-icon>dashboard</mat-icon>
        Dashboard
      </a>
      <a mat-list-item routerLink="/students">
        <mat-icon>people</mat-icon>
        Students
      </a>
      <a mat-list-item routerLink="/courses">
        <mat-icon>book</mat-icon>
        Courses
      </a>
      <a mat-list-item routerLink="/attendance">
        <mat-icon>how_to_reg</mat-icon>
        Attendance
      </a>
      <a mat-list-item routerLink="/grades">
        <mat-icon>grade</mat-icon>
        Grades
      </a>
    </mat-nav-list>
  </mat-sidenav>
  
  <mat-sidenav-content>
    <div class="content">
      <router-outlet></router-outlet>
    </div>
  </mat-sidenav-content>
</mat-sidenav-container>
```

### **8.4 Update Main Layout SCSS**
```scss
// src/app/layout/main-layout/main-layout.component.scss
.spacer {
  flex: 1 1 auto;
}

.sidenav-container {
  height: calc(100vh - 64px);
}

.sidenav {
  width: 250px;
}

.content {
  padding: 20px;
}

mat-nav-list a {
  display: flex;
  align-items: center;
}

mat-nav-list a mat-icon {
  margin-right: 16px;
}
```

---

## 📊 Step 9: Dashboard Feature

### **9.1 Generate Dashboard Component**
```powershell
npx ng generate component features/dashboard
```

### **9.2 Update Dashboard TypeScript**
```typescript
// src/app/features/dashboard/dashboard.component.ts
import { Component } from '@angular/core';
import { MaterialModule } from '../../shared/material.module';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MaterialModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
}
```

### **9.3 Update Dashboard HTML**
```html
<!-- src/app/features/dashboard/dashboard.component.html -->
<div class="dashboard">
  <h1>Welcome to EduTrack</h1>
  
  <div class="dashboard-cards">
    <mat-card class="dashboard-card">
      <mat-card-header>
        <mat-card-title>Students</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <p>Total Students: 1,250</p>
      </mat-card-content>
    </mat-card>

    <mat-card class="dashboard-card">
      <mat-card-header>
        <mat-card-title>Courses</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <p>Active Courses: 45</p>
      </mat-card-content>
    </mat-card>

    <mat-card class="dashboard-card">
      <mat-card-header>
        <mat-card-title>Teachers</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <p>Faculty Members: 68</p>
      </mat-card-content>
    </mat-card>

    <mat-card class="dashboard-card">
      <mat-card-header>
        <mat-card-title>Attendance</mat-card-title>
      </mat-card-header>
      <mat-card-content>
        <p>Today's Attendance: 92%</p>
      </mat-card-content>
    </mat-card>
  </div>
</div>
```

### **9.4 Update Dashboard SCSS**
```scss
// src/app/features/dashboard/dashboard.component.scss
.dashboard {
  padding: 20px;
}

.dashboard h1 {
  margin-bottom: 30px;
  color: #333;
}

.dashboard-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
  margin-top: 20px;
}

.dashboard-card {
  height: 150px;
  display: flex;
  flex-direction: column;
}

.dashboard-card mat-card-content {
  flex: 1;
  display: flex;
  align-items: center;
  font-size: 1.2em;
  font-weight: 500;
}
```

---

## 🛣️ Step 10: Routing Configuration

### **10.1 Update App Routes**
```typescript
// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { MainLayoutComponent } from './layout/main-layout/main-layout.component';
import { DashboardComponent } from './features/dashboard/dashboard.component';

export const routes: Routes = [
  {
    path: '',
    component: MainLayoutComponent,
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      { path: 'dashboard', component: DashboardComponent },
      // Add more routes here as features are developed
    ]
  }
];
```

### **10.2 Update App Component HTML**
```html
<!-- src/app/app.component.html -->
<router-outlet></router-outlet>
```

---

## 🚀 Step 11: Build and Run

### **11.1 Start Development Server**
```powershell
npm start
```

### **11.2 Verify Application**
- **URL**: http://localhost:4200/
- **Expected**: EduTrack dashboard with navigation sidebar
- **Features**: Material Design UI, responsive layout, routing

**Build Output Expected:**
```
Browser bundles        
Initial chunk files     | Names               |  Raw size
styles.css              | styles              |  93.20 kB | 
polyfills.js            | polyfills           |  88.09 kB | 
main.js                 | main                |  12.42 kB | 

                        | Initial total       | 193.71 kB

✓ Application running at http://localhost:4200/
```

---

## 📁 Final Project Structure

```
frontend/edutrack-ui/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── services/
│   │   │   │   └── auth.service.ts
│   │   │   ├── interceptors/
│   │   │   │   └── auth.interceptor.ts
│   │   │   └── core.module.ts
│   │   ├── shared/
│   │   │   ├── material.module.ts
│   │   │   └── shared.module.ts
│   │   ├── features/
│   │   │   └── dashboard/
│   │   │       ├── dashboard.component.ts
│   │   │       ├── dashboard.component.html
│   │   │       ├── dashboard.component.scss
│   │   │       └── dashboard.component.spec.ts
│   │   ├── layout/
│   │   │   └── main-layout/
│   │   │       ├── main-layout.component.ts
│   │   │       ├── main-layout.component.html
│   │   │       ├── main-layout.component.scss
│   │   │       └── main-layout.component.spec.ts
│   │   ├── app.component.html
│   │   ├── app.component.ts
│   │   ├── app.routes.ts
│   │   └── app.config.ts
│   ├── environments/
│   │   ├── environment.ts
│   │   └── environment.prod.ts
│   ├── index.html
│   ├── main.ts
│   └── styles.scss
├── angular.json
├── package.json
├── tsconfig.json
└── README.md
```

---

## ✅ Verification Checklist

### **Functionality Tests:**
- [ ] ✅ Application loads at http://localhost:4200/
- [ ] ✅ Navigation sidebar displays correctly
- [ ] ✅ Dashboard shows welcome message and statistics cards
- [ ] ✅ Material Design components render properly
- [ ] ✅ Routing works (default redirects to dashboard)
- [ ] ✅ Responsive design on mobile/tablet
- [ ] ✅ No console errors in browser dev tools

### **Code Quality:**
- [ ] ✅ TypeScript compilation successful
- [ ] ✅ Angular linting passes
- [ ] ✅ Clean Architecture principles followed
- [ ] ✅ Modular structure implemented
- [ ] ✅ Services properly injected

---

## 🔧 Troubleshooting

### **Common Issues:**

**1. Node.js Version Incompatibility**
```powershell
# If Angular CLI 20+ is installed with Node.js 18
npm uninstall -g @angular/cli
npm install -g @angular/cli@17
```

**2. Material Components Not Displaying**
- Verify `MaterialModule` is imported in component
- Check Angular Material theme is applied in `styles.scss`

**3. Build Errors**
```powershell
# Clear npm cache and reinstall
npm cache clean --force
rm -rf node_modules package-lock.json
npm install
```

**4. Port Already in Use**
```powershell
# Start on different port
ng serve --port 4201
```

---

## 🎯 Next Steps

### **Ready for Development:**
1. **Student Management Module** - CRUD operations
2. **Authentication Pages** - Login/register forms
3. **API Integration** - Connect to EduTrack backend
4. **State Management** - Implement NgRx store
5. **Testing Setup** - Unit and integration tests

### **Optional Enhancements:**
- Progressive Web App (PWA) features
- Internationalization (i18n)
- Dark theme support
- Advanced Material components
- Real-time notifications

---

**✅ Tutorial Complete!** 

You now have a fully functional EduTrack Angular frontend application with Material Design UI, authentication service, and a responsive dashboard layout. The application is ready for further feature development and backend integration.
