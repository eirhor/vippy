define([
    "dojo/_base/declare",
    "dojo/store/JsonRest",
    "dijit/form/FilteringSelect",
    "epi/routes"
],
function (
    declare,
    JsonRest,
    FilteringSelect,
    routes) {

    return declare([FilteringSelect], {
        postMixInProperties: function () {
            this.storeurl = this.storeurl || routes.getRestPath({ moduleArea: "app", storeName: "vippyvideo" });
            var store = new JsonRest(dojo.mixin({ target: this.storeurl }));
            this.set("store", store);
            // call base implementation            
            this.inherited(arguments);
        }
    });
});