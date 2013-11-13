define([

    "dojo/_base/connect",
    "dojo/_base/declare",

    "dijit/_CssStateMixin",
    "dijit/_Widget",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",
    "dijit/form/FilteringSelect",

    "epi/dependency",
    "epi/epi",

    "epi/shell/widget/_ValueRequiredMixin",
    //We are calling the require module class to ensure that the App module has been set up
    "geta/requiremodule!App"
],

function (

    connect,
    declare,

    _CssStateMixin,
    _Widget,
    _TemplatedMixin,
    _WidgetsInTemplateMixin,
    FilteringSelect,

    dependency,
    epi,
    _ValueRequiredMixin,

    appModule
) {

    return declare("geta.editors.VippyVideoSelection", [_Widget, _TemplatedMixin, _WidgetsInTemplateMixin, _CssStateMixin, _ValueRequiredMixin], {

        templateString: "<div class=\"dijitInline\">\
                            <div data-dojo-attach-point=\"stateNode, tooltipNode\">\
                                <div data-dojo-attach-point=\"inputWidget\" data-dojo-type=\"dijit.form.FilteringSelect\" style=\"width: 300px\"></div>\
                            </div>\
                        </div>",

        intermediateChanges: false,
        value: null,

        store: null,

        onChange: function (value) {
            // Event that tells EPiServer when the widget's value has changed.
        },

        postCreate: function () {
            // call base implementation
            this.inherited(arguments);

            // Init textarea and bind event
            this.inputWidget.set("intermediateChanges", this.intermediateChanges);

            var registry = dependency.resolve("epi.storeregistry");
            this.store = this.store || registry.get("geta.vippyvideo");

            this.inputWidget.set("store", this.store);
            this.connect(this.inputWidget, "onChange", this._onInputWidgetChanged);
        },

        isValid: function () {
            // summary:
            //    Check if widget's value is valid.
            // tags:
            //    protected, override

            return this.inputWidget.isValid();
        },

        // Setter for value property
        _setValueAttr: function (value) {
            this.inputWidget.set("value", value);
            this._set("value", value);
        },

        _setReadOnlyAttr: function (value) {
            this._set("readOnly", value);
            this.inputWidget.set("readOnly", value);
        },

        // Event handler for the changed event of the input widget

        _onInputWidgetChanged: function (value) {
            this._updateValue(value);
        },

        _updateValue: function (value) {
            if (this._started && epi.areEqual(this.value, value)) {
                return;
            }

            this._set("value", value);
            this.onChange(value);
        }
    });
});