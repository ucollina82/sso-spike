import {Injectable} from '@angular/core';
import {CookieService} from 'ngx-cookie-service';

@Injectable({
  providedIn: 'root'
})
export class CookieManagerService implements Storage {

  constructor(private cookieService: CookieService) {
  }

  [name: string]: any;

  length: number=0;

  key(index: number): string {
    throw new Error('Method not implemented.');
  }

  removeItem(key: string): void {
    this.cookieService.delete(`${key}`);
  }

  clear(): void {
    this.cookieService.deleteAll();
  }

  getItem(key: string) {
    //alert("get cookie: " + key);

    let item = this.cookieService.get(key);

    if (!!item) {
      return JSON.parse(item);
    } else {
      return null;
    }
  }

  setItem(key: string, value: any) {
    value = value || null;
    //Expiration time can be set in the third parameter of below function.
    //alert(key);
    this.cookieService.set(`${key}`, JSON.stringify(value), undefined, undefined, undefined, true, "Strict");
    //alert("set cookie: " + key+value);

    return true;
  }
}
