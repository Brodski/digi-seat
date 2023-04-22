import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private router: Router) {

  }

  ngOnInit() {
  }

  numbers = [
    { val: "1" },
    { val: "2" },
    { val: "3" },
    { val: "4" },
    { val: "5" },
    { val: "6" },
    { val: "7" },
    { val: "8" },
    { val: "+" },
  ];

  selectTable(partySize:string) {
    this.router.navigate(['select-table', { partySize: partySize }])
  }

  largeParty() {

  }
}
