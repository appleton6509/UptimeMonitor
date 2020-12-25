
/**
 * @param {String} uri
 */
export default class uribuilder {
    /**
     * 
     * @param {String} uri 
     */
    constructor(uri) {
        this.uri = uri;
        this.route = "";
        this.query = "";
    }

    /**
     * 
     * @param {String} route 
     */
    setRoute(route) {
        this.route = route;
        return this;
    }
    /**
     * 
     * @param {Object.<string, string>} query 
     */
    addQuery(query) {
        if (this.query === "")
            this.query = "?";

        for (var key in query) {
            if (this.query !== "?")
                this.query += "&";
            this.query += key + "=" + query[key];
        }

        return this;
    }

    build() {
        return this.uri + this.route + this.query;
    }

    getQuery() {
        return this.query;
    }
}