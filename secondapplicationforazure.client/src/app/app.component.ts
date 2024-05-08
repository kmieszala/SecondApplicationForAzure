import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { HttpClientService } from './modules/shared/services/http-client.service';

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  public forecasts: WeatherForecast[] = [];

  constructor(private _http: HttpClientService) {}

  ngOnInit() {
    this.getForecasts();
  }

  getForecasts() {
    this._http.get<WeatherForecast[]>('weatherforecast').subscribe(
      (result) => {
        this.forecasts = result;
      },
      (error) => {
        console.error(error);
      }
    );
  }

  title = 'secondapplicationforazure.client';
}
