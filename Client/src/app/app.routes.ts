import { Routes } from '@angular/router';
import { guestGuard } from './core/guards/guest.guard';
import { authGuard } from './core/guards/auth.guard';
import { instructorCompletionGuard } from './core/guards/instructor-completion.guard';
import { contentAccessGuard } from './core/guards/content-access.guard';

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
        loadComponent: () => import('./features/courses/courses-list').then(m => m.CoursesListComponent),
        canActivate: [instructorCompletionGuard]
    },
    {
        path: 'courses/:id',
        loadComponent: () => import('./features/courses/course-details/course-details').then(m => m.CourseDetailsComponent),
        canActivate: [instructorCompletionGuard],
        children: [
            {
                path: 'content/:contentId',
                loadComponent: () => import('./features/courses/course-details/components/content-player/content-player').then(m => m.ContentPlayerComponent),
                canActivate: [contentAccessGuard]
            }
        ]
    },
    {
        path: 'profile',
        loadComponent: () => import('./features/profiles/profile/profile').then(m => m.ProfileComponent),
        canActivate: [instructorCompletionGuard]
    },
    {
        path: 'instructor/create',
        loadComponent: () => import('./features/instructors/instructor-creation/instructor-creation.component').then(m => m.InstructorCreationComponent)
    },
    {
        path: 'instructors/:id',
        loadComponent: () => import('./features/instructors/public-profile/public-profile.component').then(m => m.InstructorPublicProfileComponent)
    },
    {
        path: 'admin/instructors',
        loadComponent: () => import('./features/admin/instructors-list/instructors-list.component').then(m => m.InstructorsListComponent),
        canActivate: [authGuard]
    },
    {
        path: 'admin/dashboard',
        loadComponent: () => import('./features/dashboards/admin-dashboard/admin-dashboard').then(m => m.AdminDashboardComponent)
    },
    {
        path: 'instructor/dashboard',
        loadComponent: () => import('./features/dashboards/instructor-dashboard/instructor-dashboard').then(m => m.InstructorDashboardComponent)
    },
    {
        path: 'student/dashboard',
        loadComponent: () => import('./features/dashboards/student-dashboard/student-dashboard').then(m => m.StudentDashboardComponent),
        canActivate: [authGuard]
    },
    {
        path: 'settings',
        loadComponent: () => import('./features/settings/settings-layout/settings-layout.component').then(m => m.SettingsLayoutComponent),
        canActivate: [authGuard],
        children: [
            {
                path: 'profile',
                loadComponent: () => import('./features/settings/profile-settings/profile-settings.component').then(m => m.ProfileSettingsComponent)
            },
            {
                path: 'security',
                loadComponent: () => import('./features/settings/security-settings/security-settings.component').then(m => m.SecuritySettingsComponent)
            },
            {
                path: 'password',
                loadComponent: () => import('./features/settings/password-settings/password-settings.component').then(m => m.PasswordSettingsComponent)
            },
            {
                path: 'delete-account',
                loadComponent: () => import('./features/settings/delete-account/delete-account.component').then(m => m.DeleteAccountComponent)
            },
            {
                path: '',
                redirectTo: 'profile',
                pathMatch: 'full'
            }
        ]
    },
    {
        path: '**',
        redirectTo: ''
    }
];
