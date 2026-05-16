import { Component } from '@angular/core';
import { MaterialModule } from '../../shared/material.module';
import { NgChartsModule } from 'ng2-charts';
import { ChartData, ChartOptions } from 'chart.js';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [MaterialModule, NgChartsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {

  // --- Doughnut: Enrollment by Department ---
  enrollmentChartData: ChartData<'doughnut'> = {
    labels: ['Computer Science', 'Business', 'Engineering', 'Arts', 'Medicine'],
    datasets: [{
      data: [320, 210, 180, 150, 390],
      backgroundColor: ['#3f51b5', '#e91e63', '#ff9800', '#4caf50', '#00bcd4'],
      hoverOffset: 8
    }]
  };
  enrollmentChartOptions: ChartOptions<'doughnut'> = {
    responsive: true,
    plugins: {
      legend: { position: 'bottom' },
      title: { display: false }
    }
  };

  // --- Bar: Grade Distribution ---
  gradeChartData: ChartData<'bar'> = {
    labels: ['A+', 'A', 'B+', 'B', 'C+', 'C', 'D', 'F'],
    datasets: [{
      label: 'Students',
      data: [95, 210, 185, 230, 145, 180, 90, 55],
      backgroundColor: '#3f51b5',
      borderRadius: 4
    }]
  };
  gradeChartOptions: ChartOptions<'bar'> = {
    responsive: true,
    plugins: { legend: { display: false } },
    scales: {
      y: { beginAtZero: true, title: { display: true, text: 'Number of Students' } },
      x: { title: { display: true, text: 'Grade' } }
    }
  };

  // --- Line: Monthly Enrollment Trend ---
  enrollmentTrendData: ChartData<'line'> = {
    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    datasets: [
      {
        label: 'New Enrollments',
        data: [65, 78, 52, 91, 120, 85, 60, 200, 175, 110, 95, 80],
        borderColor: '#3f51b5',
        backgroundColor: 'rgba(63,81,181,0.1)',
        fill: true,
        tension: 0.4,
        pointRadius: 4
      },
      {
        label: 'Graduations',
        data: [20, 15, 25, 18, 30, 22, 15, 50, 45, 35, 28, 120],
        borderColor: '#e91e63',
        backgroundColor: 'rgba(233,30,99,0.08)',
        fill: true,
        tension: 0.4,
        pointRadius: 4
      }
    ]
  };
  enrollmentTrendOptions: ChartOptions<'line'> = {
    responsive: true,
    plugins: { legend: { position: 'bottom' } },
    scales: {
      y: { beginAtZero: true, title: { display: true, text: 'Students' } }
    }
  };
}
