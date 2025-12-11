import type {GetEventsResponse} from "../models/getEventsResponse.ts";
import {API_BASE_URL} from "../../config.ts";
import axios from "axios";
import type {AxiosResponse} from "axios";
import type {EventDto} from "../models/EventDto.ts";

const apiConnector = {
    getEvents: async (): Promise<EventDto[]> => {
        try {
            const response : AxiosResponse<GetEventsResponse> = await axios.get(`${API_BASE_URL}/events`);
            const events = response.data.events;
            return events;
        }
        catch (error){
            console.log("Error fetching events: ", error);
            throw error;
        }
    },
    
    createEvent: async (event: EventDto): Promise<void> => {
        try {
            await axios.post(`${API_BASE_URL}/events`, event);
        }
        catch (error){
            
        }
    }
}