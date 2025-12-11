export interface EventDto{
    id: string | undefined;
    capacity: number;
    description: string;
    last_reservation_utc: Date,
    venue_id: string;
    name: string;
    event_date: string;
    start_date: string;
    end_date: string;
    status: string;
    type: string;
    info: string;
    total_seats: number;
    reserved_seats: number;
    available_seats: number;
    popularity: number;
    popularity_change: number;
}