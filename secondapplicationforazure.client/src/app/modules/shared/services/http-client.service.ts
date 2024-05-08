import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
//import { saveAs } from 'file-saver';

@Injectable({
  providedIn: 'root'
})
export class HttpClientService {

  headers: HttpHeaders;

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    this.headers = new HttpHeaders();
  }

  get<T>(url: string): Observable<T> {
    console.log(this.baseUrl);
    return this.http.get<T>(this.baseUrl + url, { headers: this.headers });
  }

  post<T>(url: string, body: any): Observable<T> {
    return this.http.post<T>(this.baseUrl + url, body, { headers: this.headers });
  }

  post2<T>(url: string, body: any, headers: any): Observable<T> {
    return this.http.post<T>(this.baseUrl + url, body, { headers: headers });
  }

  del<T>(url: string): Observable<T> {
    return this.http.delete<T>(this.baseUrl + url, { headers: this.headers });
  }

  postHtmlResponse(url: string, body: any): Observable<string> {
    return this.http.post(this.baseUrl + url, body, { responseType: 'text' });
  }

  put<T>(url: string, body: any): Observable<T> {
    return this.http.put<T>(this.baseUrl + url, body, { headers: this.headers });
  }

  getFile<Blob>(url: string): Observable<Blob> {
    return this.http.get<Blob>(this.baseUrl + url, { headers: this.headers, responseType: 'blob' as 'json' });
  }

  postFile<Blob>(url: string, body: any): Observable<Blob> {
    return this.http.post<Blob>(this.baseUrl + url, body, { headers: this.headers, responseType: 'blob' as 'json' });
  }

  // saveFile(file: Blob, fileName?: string): void {
  //   let blob = new Blob([file], { type: 'application/txt' });

  //   if (fileName == null) {
  //     fileName = new Date().toISOString() + '.txt';
  //   }
  //   // zapis pliku
  //   saveAs(blob, fileName);

  //   return;
  // }

  // saveXlsmFile(file: Blob, fileName?: string): void {
  //   let blob = new Blob([file], { type: 'application/octet-stream' });

  //   if (fileName == null) {
  //     fileName = new Date().toISOString() + '.xls';
  //   }
  //   // zapis pliku
  //   saveAs(blob, fileName);

  //   return;
  // }
}
