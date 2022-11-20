import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
@Injectable({
  providedIn: 'root',
})
export class CalendarService {
  private API_URL = "https://localhost:7200/api/";
  constructor(private httpClient: HttpClient) {

  }

  fetch = (calendarId: any): Promise<Object> => {
    return new Promise((resolve, reject) => {
      let url = `${this.API_URL}calendars/fetch?calendarId=${calendarId}`;
      this.httpClient.get(url).subscribe(
        (res) => {
          resolve(res);
        },
        (err) => {
          reject(err);
        }
      );
    });
  };

  create= (event: any): Promise<Object> => {
    return new Promise((resolve, reject) => {
      let url = `${this.API_URL}calendars/create`;
      this.httpClient.post(url, event).subscribe(
        (res) => {
          resolve(res);
        },
        (err) => {
          reject(err);
        }
      );
    });
  };
  
  getAccount = (): Promise<Object> => {
    return new Promise((resolve, reject) => {
      let url = `${this.API_URL}calendars/getAccounts`;
      this.httpClient.get(url).subscribe(
        (res) => {
          resolve(res);
        },
        (err) => {
          reject(err);
        }
      );
    });
  };

}
