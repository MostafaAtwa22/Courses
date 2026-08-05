import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { ContentService } from '../../features/courses/services/content.service';
import { map, catchError, of } from 'rxjs';

export const contentAccessGuard: CanActivateFn = (route, state) => {
  const contentService = inject(ContentService);
  const router = inject(Router);

  const contentId = route.paramMap.get('contentId');
  const courseId = route.parent?.paramMap.get('id');

  if (!contentId || !courseId) {
    router.navigate(['/courses']);
    return of(false);
  }

  return contentService.getById(contentId, courseId).pipe(
    map((content) => {
      if (!content.contentUrl && !content.isPreview) {
        // User doesn't have access to this content
        router.navigate(['/courses', courseId]);
        return false;
      }
      return true;
    }),
    catchError(() => {
      router.navigate(['/courses', courseId]);
      return of(false);
    })
  );
};
