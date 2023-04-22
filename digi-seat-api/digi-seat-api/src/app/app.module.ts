import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';

import { AppComponent } from './app.component';
import { HomeComponent } from './home/home.component';
import { SelectTableComponent } from './select-table/select-table.component';
import { TableService } from './services/table.service';
import { ConfigService } from './services/config.service';


const appRoutes: Routes = [
  { path: 'front', component: HomeComponent },
  { path: 'select-table:partySize', component: SelectTableComponent },
  { path: 'select-table', component: SelectTableComponent },
  { path: '**', redirectTo: 'front' }
];

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    SelectTableComponent
  ],
  imports: [
    BrowserModule,
    RouterModule.forRoot(
      appRoutes,
      //{ enableTracing: true } // <-- debugging purposes only
    ),
    HttpClientModule
  ],
  providers: [TableService, ConfigService],
  bootstrap: [AppComponent]
})
export class AppModule { }
