import type {EventDto} from "./EventDto.ts";

export interface GetEventsResponse {
    events: EventDto[];
    total_count: number;
}