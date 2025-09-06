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
