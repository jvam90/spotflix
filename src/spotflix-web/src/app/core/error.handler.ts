import { ErrorHandler, Injectable, Injector } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { ToastService } from './notify/toast.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private injector: Injector) {}

  handleError(error: Error | HttpErrorResponse): void {
    const toast = this.injector.get(ToastService);

    if (error instanceof HttpErrorResponse) {
      console.error('[GlobalErrorHandler] HTTP Error', error.status, error.message);
      const msg = error.error?.message || error.message || 'Erro na requisição';
      if (error.status >= 500) {
        toast.error('Erro do servidor. Por favor, tente novamente.');
      } else if (error.status === 0) {
        toast.error('Erro de conexão. Verifique sua internet.');
      }
    } else {
      console.error('[GlobalErrorHandler] Uncaught Error', error.message);
      toast.error('Algo deu errado. Por favor, tente novamente.');
    }
  }
}
