

/**
 * service for providing authorization to the API
 */
export default class authservice {
    /**
    * @returns {string) a token string
    */
    signOut() {
        this.setToken("");
    }
    /**
     * 
     * @param {string} username 
     * @param {string} password 
     * @return {object} object containing "tokenReceived" and "error" strings
     */
    async signIn(username,password)  { 
        var status = {
            httperror: "",
            tokenReceived: false
        }
        const uri = 'https://localhost:44373/api/Auth/SignIn';
        const jsonbody = JSON.stringify({Username: username,Password: password});
        await fetch(uri, {
            method: 'POST',
            body: jsonbody,
            headers: {
                'Accept': '*/*',
                'Content-Type': 'application/json'
            }
        }).then(res => {
            if (res.ok)
                status.tokenReceived = true
            return res.text()       
        }).then(message => {
            if (status.tokenReceived)
                this.setToken(message);
            else 
                status.httperror = message;
        }).catch(err => {
            status.httperror = "Something went wrong.";
        });     
        return new Promise((resolve,reject) => {
            resolve(status);
        })
    }
    /**
     * returns the existing token string
     */
    getToken() {
        return localStorage.getItem("x-auth-token")
    }
    setToken(token) {
        localStorage.setItem("x-auth-token",token);
    }
}