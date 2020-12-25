import uribuilder from '../Utilities/uribuilder'
export default class FetchService  {
    constructor(uri,method) {
        this.uri = uri;
        this.method = method;
    }

    /**
     * 
     * @param {Object} body 
     */
    async post(uri,body) {
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