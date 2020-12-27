import uribuilder from '../Utilities/uribuilder'
import authservice from '../Services/authservice'
export default class fetchservice  {
    constructor(uri,method) {
        this.uri = uri;
        this.method = method;
    }

    /**
     * 
     * @param {Object} body 
     */
    async post(uri,body) {
        authservice.signIn()
        const response = await fetch(uri,{
            method: 'POST',
            body: JSON.stringify(body)
        }).then(res => res.json())
        .catch(err=> console.log(err));
        return response;
    }
    
    get(token) {

    }

}