export interface ReservedSeatDto{
    id: string | undefined;
    row_number: number;
    seat_number: number;
    venue_id: string;
    is_active: boolean;
}