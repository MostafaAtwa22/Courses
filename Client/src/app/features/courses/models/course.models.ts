import { BaseResponseModel } from "../../../shared/models/base-response.model";

export enum CourseStatus {
    InProgress = 0,
    Done = 1
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
}

export interface CourseRequest {
    title: string;
    description: string;
    pictureUrl?: File;
    status: CourseStatus;
    cost: number;
    categoryId: string;
}
