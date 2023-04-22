import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { TableService } from '../services/table.service'

@Component({
  selector: 'app-select-table',
  templateUrl: './select-table.component.html',
  styleUrls: ['./select-table.component.css']
})
export class SelectTableComponent implements OnInit {

  constructor(private router: Router, private route: ActivatedRoute, private tableService: TableService) {

  }

  ngOnInit() {
    let partySize = this.route.snapshot.paramMap.get('partySize');
    this.tableService.get(partySize).subscribe(res => {
      this.table = res;
      console.log(this.table);
    })
  }

  table = {};
}
