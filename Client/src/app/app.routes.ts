import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest.guard';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./features/home/home').then(m => m.Home)
    },
    {
        path: 'auth/github-callback',
        loadComponent: () => import('./features/auth/github-callback/github-callback.component').then(m => m.GithubCallbackComponent)
    },
    {
        path: 'auth',
        loadComponent: () => import('./features/auth/auth-layout/auth-layout.component').then(m => m.AuthLayoutComponent),
        canActivate: [guestGuard],
        children: [
            {
                path: 'login',
                loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
            },
            {
                path: 'register',
                loadComponent: () => import('./features/auth/register/register.component').then(m => m.RegisterComponent)
            },
            {
                path: 'role-select',
                loadComponent: () => import('./features/auth/role-select/role-select.component').then(m => m.RoleSelectComponent)
            },
            {
                path: 'two-factor',
                loadComponent: () => import('./features/auth/two-factor/two-factor.component').then(m => m.TwoFactorComponent)
            },
            {
                path: 'forget-password',
                loadComponent: () => import('./features/auth/forget-password/forget-password.component').then(m => m.ForgetPasswordComponent)
            },
            {
                path: 'reset-password',
                loadComponent: () => import('./features/auth/reset-password/reset-password.component').then(m => m.ResetPasswordComponent)
            },
            {
                path: 'confirm-email',
                loadComponent: () => import('./features/auth/confirm-email/confirm-email.component').then(m => m.ConfirmEmailComponent)
            },
            {
                path: '',
                redirectTo: 'login',
                pathMatch: 'full'
            }
        ]
    },
    {
        path: 'courses',
        loadComponent: () => import('./features/courses/courses-list').then(m => m.CoursesListComponent)
    },
    {
        path: 'courses/:id',
        loadComponent: () => import('./features/courses/course-details/course-details').then(m => m.CourseDetailsComponent),
        children: [
            {
                path: 'content/:contentId',
                loadComponent: () => import('./features/courses/course-details/components/content-player/content-player').then(m => m.ContentPlayerComponent)
            }
        ]
    },
    {
        path: '**',
        redirectTo: ''
    }
];
