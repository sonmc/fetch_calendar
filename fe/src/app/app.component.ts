import { CalendarService } from './service/calendar.service';
import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { BryntumCalendarComponent, BryntumProjectModelComponent } from '@bryntum/calendar-angular';
import { calendarConfig } from './app.config';
import {
  SocialAuthService,
  GoogleLoginProvider,
  SocialUser,
} from 'angularx-social-login';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'calendar-demo';
  calendarConfig: any;
  calendarId: string = "";
  accounts: any = [];
  @ViewChild('calendar') calendarComponent!: BryntumCalendarComponent;
  @ViewChild('project') projectComponent!: BryntumProjectModelComponent;
  resources = [
    {
      id: 1,
      name: 'Default Calendar',
      eventColor: 'green'
    }
  ];

  events: any = [
  ];

  constructor(
    private formBuilder: FormBuilder,
    private socialAuthService: SocialAuthService,
    private calendarService: CalendarService,
    private cd: ChangeDetectorRef
  ) {
    this.calendarConfig = calendarConfig;
  }
  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      email: ['', Validators.required],
      password: ['', Validators.required],
    });
    this.socialAuthService.authState.subscribe((user) => {
      this.socialUser = user;
      debugger
      if (user.response.access_token == user.authToken) {
        console.log(user.response.access_token);
      }
      this.isLoggedin = user != null;

      var auth = {
        accessToken: user.authToken,
        calendarId: user.email,
        refreshToken: user.idToken
      }
      this.fetch(auth);
      console.log(this.socialUser);
    });
    //this.getAccount();
  }

  fetch = (auth: any) => {
    this.calendarService
      .fetch(auth).then((res: any) => {
        this.events = res.filter((x: any) => x?.summary).map((event: any, i: number) => {

          return {
            id: ++i,
            name: event.summary,
            startDate: event.start.dateTime,
            endDate: event.end.dateTime
          };
        });
        console.log(this.events);
        this.cd.detectChanges();
      })
      .catch((e) => { });
  }

  loginForm!: FormGroup;
  socialUser!: SocialUser;
  isLoggedin?: boolean;

  loginWithGoogle(): void {
    this.socialAuthService.signIn(GoogleLoginProvider.PROVIDER_ID);

  }
  logOut(): void {
    this.socialAuthService.signOut();
  }
}
