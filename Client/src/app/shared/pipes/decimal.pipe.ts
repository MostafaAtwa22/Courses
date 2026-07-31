import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'decimal',
  standalone: true
})
export class DecimalPipe implements PipeTransform {
  transform(value: number | undefined | null, decimals: number = 1): string {
    if (value === undefined || value === null) {
      return '0.0';
    }
    return value.toFixed(decimals);
  }
}
