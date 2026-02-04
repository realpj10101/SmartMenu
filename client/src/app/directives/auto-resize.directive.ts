import { AfterViewInit, Directive, ElementRef, inject, Input } from '@angular/core';

@Directive({
  selector: '[appAutoResize]'
})
export class AutoResizeDirective implements AfterViewInit {
  private _el = inject(ElementRef<HTMLTextAreaElement>);

  @Input() minHeight = 48;
  @Input() maxHeight = 120;

   ngAfterViewInit(): void {
    this.resize();
  }

  private resize(): void {
    const ta = this._el.nativeElement;

    ta.style.height = 'auto';
    const next = Math.min(ta.scrollHeight, this.maxHeight);
    ta.style.height = `${Math.max(next, this.minHeight)}px`;

    ta.style.overflowY = ta.scrollHeight > this.maxHeight ? 'scroll' : 'hidden';
  }
}
