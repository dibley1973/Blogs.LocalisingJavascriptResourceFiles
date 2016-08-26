# Localising Javascript Resource Files in ASP.Net

Localising website text in ASP.Net projects is trivial when carried out server-side using resource files, but what do you do when you need localaised text served from the client side? In previous projects I have served up the localised text as parameters which my javascript is initialised with by placing a call to my init() mehtod in my MVC view and populating the parameters with values from the ViewModel. This is fine if you dont have much text to localise but what if you do?

In this article I'd like to present one option which I have recently used. So I'll start with a very basic out of the box ASP.Net MVC application. Lets use javscript to pop up a message on the home page and we will have a look at options for localising that. First lets replicate our "Views" structure and create a "Home" folder within the "Scripts" folder. Then add a javascript file to it with the filename "app.home.index.js". This will be the script that holds client side functionailty for the "Index" view of the "Home" controller.

We will first check the "app" namespace is defined and create it if it is not, and then the same for the "app.home" namespace. Then we will create the object that will hold functionality for the "Home/Index" page.

```javascript
if (typeof app === "undefined") {
    var app = {};
}
if (typeof app.home === "undefined") {
    app.home = {};
}
app.home.index = {

};
```