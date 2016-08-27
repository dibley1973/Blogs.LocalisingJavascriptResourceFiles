if (typeof app === "undefined") {
    var app = {};
}
if (typeof app.home === "undefined") {
    app.home = {};
}
app.home.index = {
    message: "",
    message2: "",

    init: function (message, message2) {
        this.message = message;
        this.message2 = message2;
    },

    promptUser: function () {
        alert(this.message);
        alert(this.message2);
    },

    promptUser2: function () {
        alert(this.resources.message);
        alert(this.resources.message2);
    }
};