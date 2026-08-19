import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';

export interface ProdutoInterface {
  codigo: string;
  descricao: string;
  saldo: number;
}

export interface NotaFiscalInterface {
  numero: string;
  status: string;
  produtos: { codigo: string; quantidade: number }[];
  saldoTotal: number;
}

@Injectable({
  providedIn: 'root',
})
export class ProdutoService {
  private readonly apiUrl = 'http://localhost:5000/api';

  private produtoSubject = new BehaviorSubject<ProdutoInterface[]>([]);
  public produtos$ = this.produtoSubject.asObservable();

  private notaFiscalSubject = new BehaviorSubject<NotaFiscalInterface[]>([]);
  public notasFiscais$ = this.notaFiscalSubject.asObservable();

  private errorSubject = new BehaviorSubject<string | null>(null);
  public error$ = this.errorSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Produtos
  loadProdutos(): void {
    this.errorSubject.next(null);
    this.http.get<ProdutoInterface[]>(`${this.apiUrl}/produtos`).pipe(
      catchError((error) => {
        console.error('Erro ao carregar produtos:', error);
        this.errorSubject.next('Erro ao carregar produtos');
        return of([]);
      })
    ).subscribe((produtos) => {
      this.produtoSubject.next(produtos);
    });
  }

  cadastrarProduto(produto: ProdutoInterface): Observable<ProdutoInterface> {
    return this.http.post<ProdutoInterface>(`${this.apiUrl}/produtos`, produto).pipe(
      tap(() => this.loadProdutos()),
      catchError((error) => {
        const msg = error.error?.error || 'Erro ao cadastrar produto';
        this.errorSubject.next(msg);
        throw new Error(msg);
      })
    );
  }

  atualizarProduto(codigo: string, produto: Partial<ProdutoInterface>): Observable<any> {
    return this.http.put(`${this.apiUrl}/produtos/${codigo}`, produto).pipe(
      tap(() => this.loadProdutos()),
      catchError((error) => {
        const msg = error.error?.error || 'Erro ao atualizar produto';
        this.errorSubject.next(msg);
        throw new Error(msg);
      })
    );
  }

  removerProduto(codigo: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/produtos/${codigo}`).pipe(
      tap(() => this.loadProdutos()),
      catchError((error) => {
        const msg = error.error?.error || 'Erro ao remover produto';
        this.errorSubject.next(msg);
        throw new Error(msg);
      })
    );
  }

  // Notas Fiscais
  loadNotasFiscais(): void {
    this.errorSubject.next(null);
    this.http.get<NotaFiscalInterface[]>(`${this.apiUrl}/notasfiscais`).pipe(
      catchError((error) => {
        console.error('Erro ao carregar notas:', error);
        this.errorSubject.next('Erro ao carregar notas fiscais');
        return of([]);
      })
    ).subscribe((notas) => {
      this.notaFiscalSubject.next(notas);
    });
  }

  cadastrarNotaFiscal(produtos: { codigo: string; quantidade: number }[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/notasfiscais`, { produtos }).pipe(
      tap(() => this.loadNotasFiscais()),
      catchError((error) => {
        const msg = error.error?.error || 'Erro ao criar nota fiscal';
        this.errorSubject.next(msg);
        throw new Error(msg);
      })
    );
  }

  imprimirNotaFiscal(numero: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/notasfiscais/${numero}/imprimir`, {}).pipe(
      tap(() => {
        this.loadNotasFiscais();
        this.loadProdutos();
      }),
      catchError((error) => {
        const msg = error.error?.error || 'Erro ao imprimir nota fiscal';
        this.errorSubject.next(msg);
        throw new Error(msg);
      })
    );
  }

  clearError(): void {
    this.errorSubject.next(null);
  }
}
