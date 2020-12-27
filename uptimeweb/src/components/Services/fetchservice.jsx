import authservice from '../Services/authservice'


export default class fetchservice  {
    getHeaders = () => {
        const token = new authservice().getToken();
        return {
            'Accept': '*/*',
            'Content-Type': 'application/json',
            'Authorization': 'Bearer '+ token
        }
    };
    /**
     * 
     * @param {string} URI 
     */
    async get(uri) {
        const headers = this.getHeaders();
        return await fetch(uri, {
            method: 'GET',
            headers: headers
        });
    }
}