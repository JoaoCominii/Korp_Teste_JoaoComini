import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, BehaviorSubject, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

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

  private loadingSubject = new BehaviorSubject<boolean>(false);
  public loading$ = this.loadingSubject.asObservable();

  private errorSubject = new BehaviorSubject<string | null>(null);
  public error$ = this.errorSubject.asObservable();

  constructor(private http: HttpClient) {}

  // Produtos
  loadProdutos(): void {
    this.loadingSubject.next(true);
    this.errorSubject.next(null);

    this.http.get<ProdutoInterface[]>(`${this.apiUrl}/produtos`).pipe(
      catchError(this.handleError.bind(this))
    ).subscribe({
      next: (produtos) => {
        this.produtoSubject.next(produtos);
        this.loadingSubject.next(false);
      },
      error: () => {
        this.loadingSubject.next(false);
      }
    });
  }

  cadastrarProduto(produto: ProdutoInterface): Observable<ProdutoInterface> {
    return this.http.post<ProdutoInterface>(`${this.apiUrl}/produtos`, produto).pipe(
      tap(() => this.loadProdutos()),
      catchError(this.handleError.bind(this))
    );
  }

  atualizarProduto(codigo: string, produto: Partial<ProdutoInterface>): Observable<any> {
    return this.http.put(`${this.apiUrl}/produtos/${codigo}`, produto).pipe(
      tap(() => this.loadProdutos()),
      catchError(this.handleError.bind(this))
    );
  }

  removerProduto(codigo: string): Observable<any> {
    return this.http.delete(`${this.apiUrl}/produtos/${codigo}`).pipe(
      tap(() => this.loadProdutos()),
      catchError(this.handleError.bind(this))
    );
  }

  // Notas Fiscais
  loadNotasFiscais(): void {
    this.loadingSubject.next(true);
    this.errorSubject.next(null);

    this.http.get<NotaFiscalInterface[]>(`${this.apiUrl}/notasfiscais`).pipe(
      catchError(this.handleError.bind(this))
    ).subscribe({
      next: (notas) => {
        this.notaFiscalSubject.next(notas);
        this.loadingSubject.next(false);
      },
      error: () => {
        this.loadingSubject.next(false);
      }
    });
  }

  cadastrarNotaFiscal(produtos: { codigo: string; quantidade: number }[]): Observable<any> {
    return this.http.post(`${this.apiUrl}/notasfiscais`, { produtos }).pipe(
      tap(() => this.loadNotasFiscais()),
      catchError(this.handleError.bind(this))
    );
  }

  imprimirNotaFiscal(numero: string): Observable<any> {
    return this.http.post(`${this.apiUrl}/notasfiscais/${numero}/imprimir`, {}).pipe(
      tap(() => {
        this.loadNotasFiscais();
        this.loadProdutos();
      }),
      catchError(this.handleError.bind(this))
    );
  }

  // Error handling
  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Ocorreu um erro desconhecido';

    if (error.error instanceof ErrorEvent) {
      errorMessage = error.error.message;
    } else if (error.error?.error) {
      errorMessage = error.error.error;
    } else if (error.message) {
      errorMessage = error.message;
    }

    this.errorSubject.next(errorMessage);
    console.error('Erro:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }

  clearError(): void {
    this.errorSubject.next(null);
  }
}
