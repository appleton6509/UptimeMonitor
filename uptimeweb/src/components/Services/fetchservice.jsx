import {getToken} from '../Services/authservice'
import {API_URI} from '../Settings/API'

export function getHeaders() {
        const token = getToken();
        return {
            'Accept': '*/*',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer '+ token
        }
    };

export async function fetchapi(route, method, body = null) {
    const uri = API_URI + route;
    const headers = getHeaders();
    return await fetch(uri, {
        method: method,
        headers: headers,
        body: (body != null ? JSON.stringify(body) : null) 
    });
}
