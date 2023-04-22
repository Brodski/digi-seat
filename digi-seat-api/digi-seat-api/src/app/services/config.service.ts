import { Injectable } from '@angular/core';

@Injectable()
export class ConfigService {

  constructor() { }

  apiUrl() {
    return 'http://localhost:50323/api/'
  }

}
