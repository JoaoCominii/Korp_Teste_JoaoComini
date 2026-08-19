import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'korp-root',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = 'korp-frontend';
  usuario: string = 'João Comini';
  versao: string = '1.0.0';
  menuAberto: boolean = true;

  toggleMenu(): void {
    this.menuAberto = !this.menuAberto;
  }
}
