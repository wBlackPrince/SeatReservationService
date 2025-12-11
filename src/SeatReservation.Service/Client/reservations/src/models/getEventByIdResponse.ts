import type {EventDto} from "./EventDto.ts";
import type {ReservedSeatDto} from "./reservedSeatDto.ts";

export interface GetEventByIdResponseDto {
    event: EventDto;
    reservedSeats: ReservedSeatDto[];
}