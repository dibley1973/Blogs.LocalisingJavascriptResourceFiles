if (typeof app === "undefined") {
    var app = {};
}
if (typeof app.home === "undefined") {
    app.home = {};
}
app.home.index = {
    promptUser: function () {
        alert(this.resources.message);
        alert(this.resources.message2);
    }
};