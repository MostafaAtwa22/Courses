import { BaseResponseModel } from "../../../shared/models/base-response.model";

export enum CourseStatus {
    InProgress = 0,
    Done = 1
}

export enum ContentType {
    Video = 0,
    Document = 1,
    Quiz = 2,
    Other = 3
}

export interface ContentResponse extends BaseResponseModel {
    title: string;
    type: ContentType;
    contentUrl: string;
    order: number;
    isPreview: boolean;
    sectionId: string;
}

export interface SectionResponse extends BaseResponseModel {
    title: string;
    order: number;
    courseId?: string;
    contentsCount: number;
    contents?: ContentResponse[];
}

export interface ReviewResponse extends BaseResponseModel {
    headline: string;
    comment: string;
    rating: number;
    studentName: string;
    studentProfilePicture: string;
}

export interface CourseSummary extends BaseResponseModel {
    title: string;
    pictureUrl: string;
    cost: number;
    totalReviews: number;
    averageRate: number;
    category: string;
    instructorName: string;
    language: string;
}

export interface CourseResponse extends BaseResponseModel {
    title: string;
    description: string;
    pictureUrl: string;
    status: CourseStatus;
    cost: number;
    studentCount: number;
    totalReviews: number;
    averageRate: number;
    category: string;
    instructorName: string;
    instructorProfilePicture: string;
    instructorTitle: string;
    instructorBio?: string;
    sections?: SectionResponse[];
    reviews?: ReviewResponse[];
    language: string;
    whatYouWillLearn: string[];
    requirements: string[];
    introVideoUrl: string;
}

export interface CourseRequest {
    title: string;
    description: string;
    pictureUrl?: File;
    status: CourseStatus;
    cost: number;
    categoryId: string;
}

export interface ContentCreateRequest {
    title: string;
    type: ContentType;
    file: File;
    order: number;
    isPreview: boolean;
    sectionId: string;
}

export interface ContentUpdateRequest {
    title: string;
    type: ContentType;
    file?: File;
    order: number;
    isPreview: boolean;
    sectionId: string;
}

export interface SectionCreateRequest {
    title: string;
    order: number;
    courseId: string;
}

export interface SectionUpdateRequest {
    title: string;
    order: number;
}
