import { Routes } from '@angular/router';
import { ProdutoList } from './components/produto-list/produto-list';
import { NotaFiscal } from './components/nota-fiscal/nota-fiscal';
import { Estoque } from './components/estoque/estoque';

export const routes: Routes = [
  { path: '', redirectTo: '/produtos', pathMatch: 'full' },
  { path: 'produtos', component: ProdutoList },
  { path: 'notas-fiscais', component: NotaFiscal },
  { path: 'estoque', component: Estoque },
  { path: '**', redirectTo: '/produtos' }
];
