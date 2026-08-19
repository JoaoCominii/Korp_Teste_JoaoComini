import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProdutoService, ProdutoInterface } from '../../services/produto';
import { Subscription } from 'rxjs';

@Component({
  selector: 'korp-estoque',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './estoque.html',
  styleUrls: ['./estoque.scss'],
})
export class Estoque implements OnInit, OnDestroy {
  produtos: ProdutoInterface[] = [];
  private subscription!: Subscription;
  loading: boolean = true;

  constructor(private produtoService: ProdutoService) {}

  ngOnInit(): void {
    this.subscription = this.produtoService.produtos$.subscribe({
      next: (produtos) => {
        this.produtos = produtos;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });

    this.produtoService.loadProdutos();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  get totalEstoque(): number {
    return this.produtos.reduce((sum, p) => sum + p.saldo, 0);
  }

  get produtosEstoqueBaixo(): ProdutoInterface[] {
    return this.produtos.filter(p => p.saldo <= 10);
  }
}
