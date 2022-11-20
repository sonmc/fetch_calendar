import { CalendarService } from './service/calendar.service';
import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import {
  SocialAuthService,
  GoogleLoginProvider,
  SocialUser,
} from 'angularx-social-login';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'calendar-demo';
  calendarConfig: any;
  calendarId: string = '';
  accounts: any = [];
  resources = [
    {
      id: 1,
      name: 'Default Calendar',
      eventColor: 'green',
    },
  ];

  socialUser!: SocialUser;
  isLoggedin?: boolean;

  events: any = [];
  event: any = {
    summary: '',
    startDate: new Date(),
    endDate: new Date(),
  };
  constructor(
    private socialAuthService: SocialAuthService,
    private calendarService: CalendarService,
    private cd: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.socialAuthService.authState.subscribe((user) => {
      this.socialUser = user;
      if (user.response.access_token == user.authToken) {
        console.log(user.response.access_token);
      }
      this.isLoggedin = user != null;

      if (user) { 
        this.fetch(user.email);
      }
      console.log(this.socialUser);
    });
    //this.getAccount();
  }

  fetch = (calendarId: any) => {
    this.calendarService
      .fetch(calendarId)
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
      .catch((e) => {});
  };

  create = () => {  
    this.calendarService
      .create(this.event)
      .then((res: any) => {
         this.events.push(res);
      })
      .catch((e: any) => {});
  };

  loginWithGoogle(): void {
    this.socialAuthService.signIn(GoogleLoginProvider.PROVIDER_ID);
  }
  logOut(): void {
    this.socialAuthService.signOut();
  }
}
