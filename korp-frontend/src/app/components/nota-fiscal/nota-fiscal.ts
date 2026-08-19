import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ProdutoService, NotaFiscalInterface, ProdutoInterface } from '../../services/produto';
import { Subscription } from 'rxjs';

@Component({
  selector: 'korp-nota-fiscal',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './nota-fiscal.html',
  styleUrls: ['./nota-fiscal.scss'],
})
export class NotaFiscal implements OnInit, OnDestroy {
  notasFiscais: NotaFiscalInterface[] = [];
  produtos: ProdutoInterface[] = [];
  private subscription!: Subscription;
  loading: boolean = true;
  error: string | null = null;

  mostrarFormulario: boolean = false;
  produtosNota: { codigo: string; quantidade: number }[] = [];
  produtoSelecionado: string = '';
  quantidade: number = 1;
  saving: boolean = false;

  constructor(private produtoService: ProdutoService) {}

  ngOnInit(): void {
    this.subscription = this.produtoService.notasFiscais$.subscribe({
      next: (notas) => {
        this.notasFiscais = notas;
        this.loading = false;
      },
    });

    this.produtoService.loadNotasFiscais();
    this.produtoService.loadProdutos();

    this.produtoService.produtos$.subscribe({
      next: (produtos) => this.produtos = produtos,
    });
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }

  abrirFormulario(): void {
    this.mostrarFormulario = true;
    this.produtosNota = [];
    this.produtoSelecionado = '';
    this.quantidade = 1;
    this.error = null;
    this.produtoService.loadProdutos();
  }

  cancelar(): void {
    this.mostrarFormulario = false;
    this.produtosNota = [];
    this.error = null;
    this.produtoService.clearError();
  }

  adicionarProduto(): void {
    if (!this.produtoSelecionado || this.quantidade <= 0) {
      this.error = 'Selecione um produto e informe a quantidade';
      return;
    }

    const existe = this.produtosNota.find(p => p.codigo === this.produtoSelecionado);
    if (existe) {
      existe.quantidade += this.quantidade;
    } else {
      this.produtosNota.push({
        codigo: this.produtoSelecionado,
        quantidade: this.quantidade,
      });
    }

    this.produtoSelecionado = '';
    this.quantidade = 1;
  }

  removerProdutoNota(codigo: string): void {
    this.produtosNota = this.produtosNota.filter(p => p.codigo !== codigo);
  }

  criarNotaFiscal(): void {
    if (this.produtosNota.length === 0) {
      this.error = 'Adicione pelo menos um produto à nota fiscal';
      return;
    }

    if (this.saving) return;
    this.saving = true;

    this.produtoService.cadastrarNotaFiscal(this.produtosNota).subscribe({
      next: () => { this.saving = false; this.cancelar(); },
      error: (err) => { this.saving = false; this.error = err.message; },
    });
  }

  imprimirNotaFiscal(numero: string): void {
    if (confirm('Deseja imprimir esta nota fiscal? O status será alterado para "Fechada".')) {
      this.produtoService.imprimirNotaFiscal(numero).subscribe({
        error: (err) => this.error = err.message,
      });
    }
  }

  fecharErro(): void {
    this.error = null;
    this.produtoService.clearError();
  }

  getProdutoDescricao(codigo: string): string {
    const produto = this.produtos.find(p => p.codigo === codigo);
    return produto ? produto.descricao : codigo;
  }
}
