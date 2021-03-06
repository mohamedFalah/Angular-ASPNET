import { User } from '../_models/user';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/Alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable()
// tslint:disable-next-line: class-name
export class ListResolver implements Resolve<User> {
    pageNumber = 1;
    pageSize = 5;
    addsParam = 'Adders';
    constructor(private userService: UserService, private router: Router,
                private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.addsParam).pipe(
            catchError(error => {
                this.alertify.error('reteriving data error');
                this.router.navigate(['/home']);
                return of(null);
            })
        );
    }
}