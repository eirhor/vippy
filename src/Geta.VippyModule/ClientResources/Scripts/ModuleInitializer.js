﻿define([

// Dojo
    "dojo",
    "dojo/_base/declare",

//CMS
    "epi/_Module",
    "epi/dependency",
    "epi/routes"

], function (

// Dojo
    dojo,
    declare,

//CMS
    _Module,
    dependency,
    routes
) {
    return declare("geta/ModuleInitializer", [_Module], {

        // summary: Module initializer for the default module.
        initialize: function () {

            this.inherited(arguments);
            var registry = this.resolveDependency("epi.storeregistry");

            //Register the store
            registry.create("geta.vippyvideo", this._getRestPath("vippyvideo"));
        },

        _getRestPath: function (name) {
            return routes.getRestPath({ moduleArea: "app", storeName: name });
        }
    });
});