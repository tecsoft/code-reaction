
export interface Commit {
    Message: string;
    Revision: number;
    Timestamp: Date;
    Author: string;
    ApprovedBy: string;
    NumberReviewers: number;
    NumberLikes: number;
    NumberComments: number;
    NumberReplies: number;
};



export interface Comment {
    Author: string;
    Timestamp: Date;
    Message: string;
}

export interface File {

    ModText: string;
    Name: string;
    Lines: Line[];
    Revision: number;
}

export enum ChangeState {
    None = 0,
    Added,
    Removed,
    Break,
}

export interface Line {
    Id: string;
    Text : string;
    ChangeState: ChangeState;
    Likes: string[];
    RemovedLineNumber: number;
    AddedLineNumber: number;
}