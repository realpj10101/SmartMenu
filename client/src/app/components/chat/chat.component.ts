import { Component, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';
import { FormBuilder, FormControl, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MenuService } from '../../services/menu.service';
import { MenuRecommedRes, MenuRecommnedReq } from '../../models/menu';
import { Bubble } from '../../models/helpers/type';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatIconModule } from '@angular/material/icon';
import { AutoResizeDirective } from '../../directives/auto-resize.directive';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-chat',
  imports: [
    FormsModule, ReactiveFormsModule, MatIconModule, AutoResizeDirective, MatProgressSpinnerModule, CommonModule
  ],
  templateUrl: './chat.component.html',
  styleUrl: './chat.component.scss'
})
export class ChatComponent {
  @ViewChild('q') qInput!: ElementRef<HTMLTextAreaElement>;
  @ViewChild('historyEl') historyEl!: ElementRef<HTMLDivElement>;

  private _menuService = inject(MenuService);
  private _fB = inject(FormBuilder);
  private _snack = inject(MatSnackBar);

  loading = signal<boolean>(false);
  history = signal<Bubble[]>([]);

  submitFg = this._fB.group({
    searchCtr: ['']
  })

  get SearchCtrl(): FormControl {
    return this.submitFg.get('searchCtr') as FormControl;
  }

  onSubmit(): void {
    const q = this.SearchCtrl.value;
    if (!q || this.loading()) return;
    this.pushUser(q);
    this.SearchCtrl.setValue('');
    this.recommendTalk(q);
  }

  onKeyDown(e: KeyboardEvent): void {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      this.onSubmit();
    }
  }

  private pushUser(text: string): void {
    this.history.update(h => [...h, { role: 'user', text }]);
    this.loading.set(true);
    this.scrollToBottom();
  }

  private recommendTalk(prompt: string): void {
    let req = new MenuRecommnedReq();
    req.query = prompt;

    this._menuService.recommendTalk(req).subscribe({
      next: (res: MenuRecommedRes) => {
        this.loading.set(false);
        if (res) {
          this.history.update(h => [...h, { role: 'agent', text: res.messageFa, meta: res }])
        }
      },
      error: (err) => {
        this.loading.set(false);
        this._snack.open('خطایی رخ داد دوباره تلاش کنید!', 'بستن', {
          direction: 'rtl',
          duration: 5000,
          verticalPosition: 'top',
          horizontalPosition: 'center'
        })
      }
    })
  }

  private scrollToBottom(): void {
    queueMicrotask(() => {
      if (!this.historyEl) return;
      const el = this.historyEl.nativeElement;
      el.scrollTop = el.scrollHeight;
    })
  }
}
