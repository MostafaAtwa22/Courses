import { Routes } from '@angular/router';

export const routes: Routes = [
    {
        path: '',
        loadComponent: () => import('./features/home/home').then(m => m.Home)
    },
    {
        path: 'auth',
        loadComponent: () => import('./features/auth/auth-layout/auth-layout.component').then(m => m.AuthLayoutComponent),
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
