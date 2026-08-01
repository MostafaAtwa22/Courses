import { Component, Input, Output, EventEmitter, ViewChild, ElementRef, AfterViewInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { environment } from '../../../../environments/environment';

@Component({
  selector: 'app-video-player',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './video-player.component.html',
  styleUrl: './video-player.component.scss'
})
export class VideoPlayerComponent implements AfterViewInit, OnDestroy {
  @Input() videoUrl?: string;
  @Input() autoplay = true;
  @Input() showDownload = true;
  @Input() downloadTitle?: string;
  @Input() compactMode = false;
  
  @Output() videoLoaded = new EventEmitter<number>();
  @Output() timeUpdate = new EventEmitter<number>();
  @Output() playStateChange = new EventEmitter<boolean>();
  
  @ViewChild('videoPlayer', { static: false }) videoPlayer!: ElementRef<HTMLVideoElement>;
  @ViewChild('videoContainer', { static: false }) videoContainer!: ElementRef<HTMLDivElement>;
  
  // Video player controls
  playbackRate = 1;
  playbackSpeeds = [0.5, 0.75, 1, 1.25, 1.5, 1.75, 2];
  showSpeedMenu = false;
  volume = 1;
  lastVolume = 1;
  isMuted = false;
  isFullscreen = false;
  isPlaying = false;
  currentTime = 0;
  duration = 0;

  private eventListeners: Array<() => void> = [];

  ngAfterViewInit(): void {
    this.setupVideoEventListeners();
  }

  ngOnDestroy(): void {
    this.removeEventListeners();
  }

  private setupVideoEventListeners(): void {
    if (this.videoPlayer?.nativeElement) {
      const video = this.videoPlayer.nativeElement;
      
      const timeUpdateHandler = () => this.onTimeUpdate();
      const loadedMetadataHandler = () => this.onVideoLoaded();
      const playHandler = () => {
        this.isPlaying = true;
        this.playStateChange.emit(true);
      };
      const pauseHandler = () => {
        this.isPlaying = false;
        this.playStateChange.emit(false);
      };
      const volumeChangeHandler = () => {
        this.volume = video.volume;
        this.isMuted = video.muted;
      };

      video.addEventListener('timeupdate', timeUpdateHandler);
      video.addEventListener('loadedmetadata', loadedMetadataHandler);
      video.addEventListener('play', playHandler);
      video.addEventListener('pause', pauseHandler);
      video.addEventListener('volumechange', volumeChangeHandler);

      this.eventListeners.push(
        () => video.removeEventListener('timeupdate', timeUpdateHandler),
        () => video.removeEventListener('loadedmetadata', loadedMetadataHandler),
        () => video.removeEventListener('play', playHandler),
        () => video.removeEventListener('pause', pauseHandler),
        () => video.removeEventListener('volumechange', volumeChangeHandler)
      );
    }
  }

  private removeEventListeners(): void {
    this.eventListeners.forEach(unsubscribe => unsubscribe());
    this.eventListeners = [];
  }

  getFullUrl(relativeUrl?: string): string {
    if (!relativeUrl) return '';
    if (relativeUrl.startsWith('http://') || relativeUrl.startsWith('https://')) {
      return relativeUrl;
    }
    return `${environment.apiUrl}/${relativeUrl.replace(/^\//, '')}`;
  }

  onTimeUpdate(): void {
    if (this.videoPlayer?.nativeElement) {
      this.currentTime = this.videoPlayer.nativeElement.currentTime;
      this.timeUpdate.emit(this.currentTime);
    }
  }

  onVideoLoaded(): void {
    if (this.videoPlayer?.nativeElement) {
      this.duration = this.videoPlayer.nativeElement.duration;
      this.videoLoaded.emit(this.duration);
    }
  }

  togglePlay(): void {
    if (this.videoPlayer?.nativeElement) {
      const video = this.videoPlayer.nativeElement;
      if (video.paused) {
        video.play();
      } else {
        video.pause();
      }
    }
  }

  seekTo(event: MouseEvent): void {
    if (this.videoPlayer?.nativeElement) {
      const progressBar = event.currentTarget as HTMLElement;
      const rect = progressBar.getBoundingClientRect();
      const clickPosition = (event.clientX - rect.left) / rect.width;
      this.videoPlayer.nativeElement.currentTime = clickPosition * this.duration;
    }
  }

  setPlaybackRate(rate: number): void {
    this.playbackRate = rate;
    if (this.videoPlayer?.nativeElement) {
      this.videoPlayer.nativeElement.playbackRate = rate;
    }
    this.showSpeedMenu = false;
  }

  toggleMute(): void {
    if (this.videoPlayer?.nativeElement) {
      const video = this.videoPlayer.nativeElement;
      if (this.isMuted) {
        this.isMuted = false;
        video.muted = false;
        this.volume = this.lastVolume > 0 ? this.lastVolume : 1;
        video.volume = this.volume;
      } else {
        this.lastVolume = this.volume;
        this.isMuted = true;
        this.volume = 0;
        video.muted = true;
        video.volume = 0;
      }
    }
  }

  setVolume(vol: number): void {
    this.volume = vol;
    if (this.videoPlayer?.nativeElement) {
      this.videoPlayer.nativeElement.volume = vol;
      this.videoPlayer.nativeElement.muted = vol === 0;
      this.isMuted = vol === 0;
    }
  }

  getVolumeIcon(): string {
    if (this.isMuted || this.volume === 0) return 'fa-volume-mute';
    if (this.volume < 0.5) return 'fa-volume-down';
    return 'fa-volume-up';
  }

  async goFullscreen(): Promise<void> {
    if (this.videoContainer?.nativeElement) {
      try {
        if (!document.fullscreenElement) {
          await this.videoContainer.nativeElement.requestFullscreen();
          this.isFullscreen = true;
        } else {
          await document.exitFullscreen();
          this.isFullscreen = false;
        }
      } catch (err) {
        console.error('Fullscreen error:', err);
      }
    }
  }

  async togglePictureInPicture(): Promise<void> {
    if (this.videoPlayer?.nativeElement) {
      const video = this.videoPlayer.nativeElement;
      
      try {
        if (document.pictureInPictureElement) {
          await document.exitPictureInPicture();
        } else {
          if (video.requestPictureInPicture) {
            await video.requestPictureInPicture();
          } else {
            console.warn('Picture-in-Picture is not supported in this browser');
          }
        }
      } catch (err) {
        console.error('Picture-in-Picture error:', err);
      }
    }
  }

  formatTime(seconds: number): string {
    if (isNaN(seconds)) return '0:00';
    const mins = Math.floor(seconds / 60);
    const secs = Math.floor(seconds % 60);
    return `${mins}:${secs.toString().padStart(2, '0')}`;
  }

  reloadVideo(): void {
    if (this.videoPlayer?.nativeElement) {
      this.videoPlayer.nativeElement.load();
      this.currentTime = 0;
      this.duration = 0;
      this.isPlaying = false;
    }
  }

  play(): void {
    if (this.videoPlayer?.nativeElement) {
      this.videoPlayer.nativeElement.play();
    }
  }

  pause(): void {
    if (this.videoPlayer?.nativeElement) {
      this.videoPlayer.nativeElement.pause();
    }
  }
}
