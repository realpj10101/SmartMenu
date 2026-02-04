import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { environment } from '../../environments/environment.development';
import { MenuRecommedRes, MenuRecommnedReq } from '../models/menu';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MenuService {
  private _http = inject(HttpClient);
  
  private readonly _baseApiUrl = environment.apiUrl + 'api/menu/';

  recommendTalk(req: MenuRecommnedReq): Observable<MenuRecommedRes> {
    return this._http.post<MenuRecommedRes>(this._baseApiUrl + 'menu-talk', req);
  }
}
