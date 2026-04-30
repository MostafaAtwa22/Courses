import { BaseResponseModel } from "../../../shared/models/base-response.model";

export interface CategoryResponse extends BaseResponseModel {
    name: string;
    slug: string;
    numberOfCourses: number;
}

export interface CategoryRequest {
    name: string;
    slug: string;
}