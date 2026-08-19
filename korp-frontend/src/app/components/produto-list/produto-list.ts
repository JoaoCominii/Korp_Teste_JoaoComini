import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProdutoService, ProdutoInterface } from '../../services/produto';
import { Subscription } from 'rxjs';

@Component({
  selector: 'korp-produto-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './produto-list.html',
  styleUrls: ['./produto-list.scss'],
})
export class ProdutoList implements OnInit, OnDestroy {
  produtos: ProdutoInterface[] = [];
  private subscription!: Subscription;
  loading: boolean = true;
  error: string | null = null;

  // Form
  mostrarFormulario: boolean = false;
  editando: boolean = false;
  codigoAtual: string = '';
  novoProduto: ProdutoInterface = { codigo: '', descricao: '', saldo: 0 };

  constructor(private produtoService: ProdutoService) {}

  ngOnInit(): void {
    this.subscription = new Subscription();

    this.subscription.add(
      this.produtoService.produtos$.subscribe({
        next: (produtos) => {
          this.produtos = produtos;
          this.loading = false;
        },
        error: (err) => {
          console.error('Erro ao carregar produtos:', err);
          this.loading = false;
        },
      })
    );

    this.subscription.add(
      this.produtoService.loading$.subscribe({
        next: (loading) => this.loading = loading
      })
    );

    this.subscription.add(
      this.produtoService.error$.subscribe({
        next: (error) => this.error = error
      })
    );

    this.produtoService.loadProdutos();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  abrirFormulario(): void {
    this.mostrarFormulario = true;
    this.editando = false;
    this.novoProduto = { codigo: '', descricao: '', saldo: 0 };
  }

  editarProduto(produto: ProdutoInterface): void {
    this.mostrarFormulario = true;
    this.editando = true;
    this.codigoAtual = produto.codigo;
    this.novoProduto = { ...produto };
  }

  cancelar(): void {
    this.mostrarFormulario = false;
    this.editando = false;
    this.novoProduto = { codigo: '', descricao: '', saldo: 0 };
    this.produtoService.clearError();
  }

  salvar(): void {
    if (!this.novoProduto.codigo || !this.novoProduto.descricao) {
      this.error = 'Código e Descrição são obrigatórios';
      return;
    }

    if (this.editando) {
      this.produtoService.atualizarProduto(this.codigoAtual, this.novoProduto).subscribe({
        next: () => {
          this.cancelar();
        },
        error: (err) => {
          this.error = err.message;
        }
      });
    } else {
      this.produtoService.cadastrarProduto(this.novoProduto).subscribe({
        next: () => {
          this.cancelar();
        },
        error: (err) => {
          this.error = err.message;
        }
      });
    }
  }

  removerProduto(codigo: string): void {
    if (confirm('Tem certeza que deseja remover este produto?')) {
      this.produtoService.removerProduto(codigo).subscribe({
        error: (err) => {
          this.error = err.message;
        }
      });
    }
  }

  fecharErro(): void {
    this.error = null;
    this.produtoService.clearError();
  }
}
