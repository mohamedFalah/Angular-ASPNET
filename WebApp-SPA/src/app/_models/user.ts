import { Photo } from './Photo';

export interface User {
    id: number;
    username: string;
    knownAs?: string;
    created?: Date;
    age: number;
    gender: string;
    lastActive: Date;
    photoUrl: string;
    city: string;
    country: string;
    interests?: string;
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];
}
