import { CalendarService } from './service/calendar.service';
import { Component, OnInit, ViewChild, ChangeDetectorRef } from '@angular/core';
import { BryntumCalendarComponent, BryntumProjectModelComponent } from '@bryntum/calendar-angular';
import { calendarConfig } from './app.config';

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
  constructor(private calendarService: CalendarService, private cd: ChangeDetectorRef) {
    this.calendarConfig = calendarConfig;
  }
  ngOnInit(): void {
    this.getAccount();
  }

  fetch = () => {
    this.calendarService
      .fetch(this.calendarId).then((res: any) => {
        this.events = res.filter((x: any) => x?.summary).map((event: any, i: number) => {
          return {
            id: ++i,
            name: event.summary,
            startDate: event.start.dateTime,
            endDate: event.end.dateTime
          };
        });
        this.cd.detectChanges();
      })
      .catch((e) => { });
  }

  getAccount = () => {
    this.calendarService
      .getAccount().then((res: any) => {
        this.accounts = res;
        this.calendarId = this.accounts[0].email;
        this.fetch();
      })
      .catch((e: any) => {

      });
  }

  changeAccount = () => {
    this.fetch();
  }
}
