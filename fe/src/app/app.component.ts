import { CalendarService } from './service/calendar.service';
import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
declare var google: any;

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  title = 'calendar-demo';
  calendarConfig: any;
  clientId: any;
  calendarId: string = '';
  accounts: any = [];
  resources = [
    {
      id: 1,
      name: 'Default Calendar',
      eventColor: 'green',
    },
  ];

  isLoggedin?: boolean;

  events: any = [];
  event: any = {
    summary: '',
    startDate: new Date(),
    endDate: new Date(),
  };
  objetounico: any;
  constructor(
    private calendarService: CalendarService,
    private cd: ChangeDetectorRef
  ) {

  }

  ngAfterViewInit(): void {
    google.accounts.id.initialize({
      client_id:
        '802443277667-rhrn3lddp17723bkass03a4d96rrujor.apps.googleusercontent.com',
      callback: this.handleCredentialResponse,
    });
    google.accounts.id.renderButton(
      document.getElementById('buttonDiv'),
      { theme: 'outline', size: 'large' } // customization attributes
    );
    google.accounts.id.prompt(); // also display the One Tap dialog
  }


  handleCredentialResponse(response: any) {
    var base64Url = response.credential.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload: any = decodeURIComponent(
      window
        .atob(base64)
        .split('')
        .map(function (c) {
          return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        })
        .join('')
    );
    sessionStorage.setItem('email', JSON.parse(jsonPayload).email);
    sessionStorage.setItem('clientId', response.clientId);
  }

  clickFetch() {
    let email = sessionStorage.getItem('email') as string;
    let clientId = sessionStorage.getItem('clientId') as string;
    this.fetch(email, clientId);
  }

  fetch = (calendarId: any, clientId: any) => {
    this.calendarService
      .fetch(calendarId, clientId)
      .then((res: any) => {
        this.events = res
          .filter((x: any) => x?.summary)
          .map((event: any, i: number) => {
            return {
              index: i++,
              summary: event.summary,
              startDate: event.start.dateTime,
              endDate: event.end.dateTime,
            };
          });
        this.cd.detectChanges();
      })
      .catch((e) => { });
  };
  create = () => {
    let email = sessionStorage.getItem('email') as string;
    let clientId = sessionStorage.getItem('clientId') as string;
    this.event = {
      ...this.event,
      calendarId: email,
      clientId
    }
    this.calendarService
      .create(this.event)
      .then((res: any) => {
        this.events.push(res);
      })
      .catch((e: any) => { });
  };
}
