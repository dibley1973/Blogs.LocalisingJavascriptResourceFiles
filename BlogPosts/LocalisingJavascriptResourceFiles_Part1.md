# Localising Javascript Resource Files in ASP.Net

Localising website text in ASP.Net projects is trivial when carried out server-side using resource files, but what do you do when you need localaised text served from the client side? In previous projects I have served up the localised text as parameters which my javascript is initialised with by placing a call to my init() mehtod in my MVC view and populating the parameters with values from the ViewModel. This is fine if you dont have much text to localise but what if you do?

In this article I'd like to present one option which I have recently used. So I'll start with a very basic out of the box ASP.Net MVC application. Lets use javscript to pop up a message on the home page and we will have a look at options for localising that. First lets replicate our "Views" structure and create a "Home" folder within the "Scripts" folder. Then add a javascript file to it with the filename "app.home.index.js". This will be the script that holds client side functionailty for the "Index" view of the "Home" controller.

We will first check the "app" namespace is defined and create it if it is not, and then the same for the "app.home" namespace. Then we will create the object that will hold functionality for the "Home\Index" page. Before we go "fully resourced filed" lets take a look at how previosuly I may have approached this, and then we will make improvements as we go along.

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
 Within the "index" object we will create a field "message" to hold a message we are going to display and an init function which will take a message as a parameter and populate the message field from it. We will then add a function which will display the message to the user in a javascript popup. 

```javascript
app.home.index = {
    message: "",

    init: function(message) {
        this.message = message;
    },

    promptUser: function() {
        alert(this.message);
    }
};
```
Lets create a "Resources" folder in the root of the web application project, and within this a "Home" folder. Within the "Home" folder we will create resource file called "Index" for the content of the `Home/Index` view. In this file we will add a string resource called "Message" and set the value to "Hello World". We must change the "Access Modifier" of the file to "Public". While we are here lets move the "ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS and JavaScript." test into the resource file with the name "SubHeader" so we can reference this text in the view.

|   | Name      | Value                                                                                                             |  Comment |
|---|-----------|-------------------------------------------------------------------------------------------------------------------|----------|
| > | Message   | Hello World                                                                                                       |          |
|   | SubHeader | ASP.NET is a free web framework for building great Web sites and Web applications using HTML, CSS and JavaScript. |          |

Lets open the default `Home/Index` MVC view and remove all but the "jumbotron" message. Add a using at  the top of the view `@using Blogs.LocalisingJavascriptResourceFiles.UI.Resources.Home` to import the home page resources namespace. Lets change the sub header to use the resource file. Then we can add a scripts section to reference the javascript file for this view and initialise it using the resource file content, and then call the `promptUser()` to show the message.

```html
@using Blogs.LocalisingJavascriptResourceFiles.UI.Resources.Home
@{
    ViewBag.Title = "Home Page";
}

<div class="jumbotron">
  <h1>ASP.NET</h1>
  <p class="lead">@Index.SubHeader</p>
</div>

@section Scripts{
  <script src="@Url.Content("~/Scripts/Home/app.home.index.js")"></script>
  <script>
    app.home.index.init("@Html.Raw(Index.Message)");
    app.home.index.promptUser();
  </script>
}
```

Run the application and all going well you should have a javascript alert pop up with "hello World".

So thats all great we have a message in english declared in a C# resource file and use this to populate the fileed in the javasript. So now lets add another culture resource file. Copy the "Index.resx" file and paste a copy in the same "Resources/Home" folder location. Change the filename to "index.de-DE.resx" so we have a file for German speaking countries. Open the file and translate the two text resources to say "Hallo Welt" for the `Message`and "ASP.NET ist eine kostenlose Web-Framework für den Aufbau von großen Web -Sites und Web -Anwendungen mit HTML, CSS und JavaScript." for the `SubHeader`.

We now need to force the culture to "de-DE" in the webconfig file. Add a `globalization` node inside `system.web`.

```xml
  <system.web>
    <globalization uiCulture="de-DE" culture="de-DE" />
  </system.web>
```

Lest run the app again, both the sub heading *but* more importantly the javascript alert should display in Deutch (German). So this is great but what happens if we have a few messages that are displayed from javascript and need to be localised. Well first step you may decide to add another parameter to the javascript `app.home.index.init()` method...

Lest create a new string resource in the two C# resource files with the name `Message2` and the values "How are you?" and "Wie geht es dir?" respectively for the default and "de-DE" versions. Lets add a second message field to the javascript `app.home.index` object and call it inspiringly... `message2`. We need to initialise this from the `init()` function along with the original message.

```javascript
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
    }
};
```

And in the "Index "view..
```html
   app.home.index.init("@Html.Raw(Index.Message)", "@Html.Raw(Index.Message2)");
```

Ok so this works but already it's getting kind of unwieldy and the more text that is needed the worse it will get. Now you can get around this slightly using the JQuery's `extend` method like `options = $.extend(defaults, options);` to overwrite default paceholder strings with those passed in through an "options" parameter of the `app.home.index.init()` method. But this is still unwieldy as in your view you will need to instatiate the options and set the properties from the C# resource file, and this could become quite a high quantity in time.

What might be nice would be to have a javascript equivelent of the C# resources file that has the correctly localised strings embedded. So lets create a javascript resource file and see how things pan out. In the "Scripts\Home" folder add a javascript file called "app.home.index.resource.js". For the time being just populate the two resource message fields with the english values.

```javascript
if (typeof app === "undefined") {
    var app = {};
}
if (typeof app.home === "undefined") {
    app.home = {};
}
if (typeof app.home.index === "undefined") {
    app.home.index = {};
}
app.home.index.resources = {
    message: "Hello World",
    message2: "How are you?"
};
```

Add a new method to the `app.home.index` object called `promptUser2` which will reference the strings in the  resource file directly.

```javascript
    promptUser2: function () {
        alert(this.resources.message);
        alert(this.resources.message2);
    }
```

Switching  back to the "Index" view, comment out the call to `app.home.index.promptUser();` and add a call to `app.home.index.promptUser2();` instead. Add a script reference to teh javascript indes resource file after the main javascript index file.

```html
@section Scripts{
  <script src="@Url.Content("~/Scripts/Home/app.home.index.js")"></script>
  <script src="@Url.Content("~/Scripts/Home/app.home.index.resource.js")"></script>
  <script>
    app.home.index.init("@Html.Raw(Index.Message)", "@Html.Raw(Index.Message2)");
    //app.home.index.promptUser();
    app.home.index.promptUser2();
  </script>
}
```

Remember to comment out the "globalization" node in the "web.Config" file to return to your current culture, and then you can test the app. Obviously it will just return the hard coded non-localised strings in the js resources file at this present time. Now lets look at getting these values localised. So to do this we are going to turn the "magic strings" in the resource file into named placeholders that directly correspond to the names in the C# resource file. We will wrap the names in curly braces to help identify them when we read them later in the server code.

```javascript
if (typeof app === "undefined") {
    var app = {};
}
if (typeof app.home === "undefined") {
    app.home = {};
}
if (typeof app.home.index === "undefined") {
    app.home.index = {};
}
app.home.index.resources = {
    message: "{Message}",
    message2: "{Message2}"
};
```

TBC...



