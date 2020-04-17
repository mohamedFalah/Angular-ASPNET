import { User } from '../_models/user';
import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/Alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../_services/auth.service';

@Injectable()
// tslint:disable-next-line: class-name
export class memberEditResolver implements Resolve<User> {
    constructor(private userService: UserService, private router: Router,
                private alertify: AlertifyService, private authService: AuthService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<User> {
        return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
            catchError(error => {
                this.alertify.error('reteriving your data error');
                this.router.navigate(['/members']);
                return of(null);
            })
        );
    }
}