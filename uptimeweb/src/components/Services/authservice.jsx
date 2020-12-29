import { API_URI } from "../Settings/API.js";

export function logout() {
    this.setToken("");
}
/**
 * @returns {Object} a object containing "error" message and boolean "success"
 * @param {string} username 
 * @param {string} password 
 */
export async function createLogin(username, password) {
    const jsonbody = JSON.stringify({ Username: username, Password: password });
    const uri = API_URI + 'Auth/SignUp';

    var status = {
        error: "",
        success: false
    }
    await fetch(uri, {
        method: 'POST',
        body: jsonbody,
        headers: {
            'Accept': '*/*',
            'Content-Type': 'application/json'
        }
    }).then(res => {
        if (res.ok)
            status.success = true
        return res.text()
    }).then(message => {
        if (!status.success)
            status.error = message
    }).catch(err => {
        status.error = "something went wrong"
    });
    return status
}

/**
 * 
 * @param {string} username 
 * @param {string} password 
 * @returns {Object} a object containing "error" message and boolean "success"
 */
export async function login(username, password) {
    var status = {
        error: "",
        success: false
    }
    const uri = API_URI + "Auth/SignIn"
    const jsonbody = JSON.stringify({ Username: username, Password: password });
    await fetch(uri, {
        method: 'POST',
        body: jsonbody,
        headers: {
            'Accept': '*/*',
            'Content-Type': 'application/json'
        }
    }).then(res => {
        if (res.ok)
            status.success = true
        return res.text()
    }).then(message => {
        if (status.success)
            this.setToken(message);
        else
            status.error = message;
    }).catch(err => {
        status.error = "Something went wrong.";
    });
    return new Promise((resolve, reject) => {
        resolve(status);
    });
}
/**
 * returns the existing token string
 */
export function getToken() {
    return localStorage.getItem("x-auth-token")
}
export function setToken(token) {
    localStorage.setItem("x-auth-token", token);
}
