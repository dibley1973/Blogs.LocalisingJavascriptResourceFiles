# Localising Javascript Resource Files in ASP.Net MVC

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

So next up we need a class that can covert our javscript files with named place holders into a script with the appropriate culture resource strings inserted. Lets add a folder called Helpers to the root of the "Blogs.LocalisingJavascriptResourceFiles.UI" project. However before we can "covert" the file we need to "load" the file contents and to carry this out we will create a class called `JavascriptResourceFileContentLoader`. The class takes the base javascript folder name, the name of the controller and the name of the view in the constructor. After some simple guard clause are passed the values are cached. The `Load` method builds the file path, checks a file exists at the path and opens the file without a file lock and then loads the content into the `Output` poperty. Notice the `Load` method returns the current instance. It could have been a void, but instead by return the same instance it allows a fluid API. Not everyone's cup of tea, I know. If the file path does not lead to a file then a `FileNotFoundException` is thrown from the `EnsureFileExists` method.

```java
    public class JavascriptResourceFileContentLoader
    {
        private const char BackSlash = '\\';
        private readonly string _baseScriptFolderPath;
        private readonly string _controllerName;
        private readonly string _viewName;
        private string _resourceFilePath;
        public string Output { get; private set; }

        public JavascriptResourceFileContentLoader(string baseScriptFolderPath, string controllerName, string viewName)
        {
            if (string.IsNullOrWhiteSpace(baseScriptFolderPath)) throw new ArgumentNullException("baseScriptFolderPath");
            if (string.IsNullOrWhiteSpace(controllerName)) throw new ArgumentNullException("controllerName");
            if (string.IsNullOrWhiteSpace(viewName)) throw new ArgumentNullException("viewName");

            _baseScriptFolderPath = baseScriptFolderPath;
            _controllerName = controllerName;
            _viewName = viewName;
        }

        public JavascriptResourceFileContentLoader Load()
        {
            BuildFilePath();
            EnsureFileExists();
            LoadFileWithNoLock();

            return this;
        }

        private void BuildFilePath()
        {
            var pathBuilder = new StringBuilder();
            pathBuilder.Append(_baseScriptFolderPath);
            var lastCharIsBackSlash = (pathBuilder[pathBuilder.Length - 1] == BackSlash);
            if (!lastCharIsBackSlash) pathBuilder.Append(BackSlash);
            pathBuilder.Append(_controllerName);
            pathBuilder.Append(BackSlash);
            pathBuilder.Append("app.");
            pathBuilder.Append(_controllerName.ToLower());
            pathBuilder.Append(".");
            pathBuilder.Append(_viewName.ToLower());
            pathBuilder.Append(".resource.js");

            _resourceFilePath = pathBuilder.ToString();
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _resourceFilePath = string.Concat(baseDirectory, _resourceFilePath);

        }

        private void EnsureFileExists()
        {
            var fileExists = File.Exists(_resourceFilePath);
            if (!fileExists) throw new FileNotFoundException(_resourceFilePath + " does not exist. ");
        }

        private void LoadFileWithNoLock()
        {
            string content;
            using (var fileReadStream = new FileStream(_resourceFilePath,
                FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
            )
            {
                using (StreamReader fileStreamReader = new StreamReader(fileReadStream))
                {
                    content = fileStreamReader.ReadToEnd();
                }
            }

            Output = content;
        }
    }
```

Now we have a class that can read the content of teh javascript resource file, we now need a class to manipulate that content and mutate the named placeholders in culture specific values, i.e. replacing the placeholders with values from the approppriate C# resource file. For this we will use the `JavascriptResourceFileContentCulturiser`. Nice name? Not really but I could not come up with anything more descriptive!

The first point of note is the class has some static members which hold the assembly and construct and hold the namespace. These are static as they will always be the same for all instances of the class, regardless of what parameters the class is constructed with. The class takes the unadulterated javascript file content, the name of the controller and the name of the view in the constructor. After some simple guard clause are passed the values are cached. The main method is `Culturise` which gets a reference to the ResourceManager for the we are intending to use, splits the content into lines and iterates through them looking for one of our names placeholders. If one is found on the line then it is compared with the named resource strings and if a matching resource string is found the placeholder name replaced with the resource string value. All of the content lines, mutated or otherwise are packed into a StringBuilder and forced into the `Output` property.

```java
    public class JavascriptResourceFileContentCulturiser
    {
        #region Static Members

        private static string _nameSpace;
        private static Assembly _assembly;
        private static readonly Regex PlaceHolderRegex = new Regex("{.+?}");
        private static readonly char[] LineSeparators = { '\n', '\r' };
        private const string ResourcesFolderName = "Resources";
        private const char PlaceHolderStartChracter = '{';
        private const char PlaceHolderEndChracter = '}';
        private const char NamespaceDelimiter = '.';

        static JavascriptResourceFileContentCulturiser()
        {
            SetAssembly();
            BuildNameSpace();
        }

        private static void SetAssembly()
        {
            _assembly = Assembly.GetExecutingAssembly();
        }

        private static void BuildNameSpace()
        {
            _nameSpace = _assembly.GetName().Name;
        }

        #endregion

        #region Instance members

        private readonly string _content;
        private readonly string _controllerName;
        private readonly string _viewName;
        public string Output { get; private set; }

        public JavascriptResourceFileContentCulturiser(string content, string controllerName, string viewName)
        {
            if (string.IsNullOrWhiteSpace(content)) throw new ArgumentNullException("content");
            if (string.IsNullOrWhiteSpace(controllerName)) throw new ArgumentNullException("controllerName");
            if (string.IsNullOrWhiteSpace(viewName)) throw new ArgumentNullException("viewName");

            _content = content;
            _controllerName = controllerName;
            _viewName = viewName;
        }

        public JavascriptResourceFileContentCulturiser Culturise()
        {
            var type = GetTypeForFullyQualifiedName();
            if (type == null) throw new InvalidOperationException(string.Concat(_viewName, " resource not found "));

            var resourceManager = new ResourceManager(type);
            var scriptBuilder = new StringBuilder();

            string[] originalContentAsLines = _content.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries);
            foreach (var originalContentLine in originalContentAsLines)
            {
                if (string.IsNullOrWhiteSpace(originalContentLine)) continue; // Should not actuall happen due to string split options.

                var matches = PlaceHolderRegex.Matches(originalContentLine);
                var haveMatches = matches.Count > 0;
                string finalScriptLine = originalContentLine;

                if (haveMatches)
                {
                    string replacementValue;
                    var firstMatch = matches[0].ToString();
                    var haveReplacableValue = TryGetReplacementValue(firstMatch, resourceManager, out replacementValue);
                    if (haveReplacableValue)
                    {
                        finalScriptLine = originalContentLine.Replace(firstMatch, replacementValue);
                    }
                }

                scriptBuilder.AppendLine(finalScriptLine);
            }

            Output = scriptBuilder.ToString();
            return this;
        }

        private Type GetTypeForFullyQualifiedName()
        {
            var fullyQualifiedName = string.Concat(_nameSpace, NamespaceDelimiter, ResourcesFolderName,
                NamespaceDelimiter, _controllerName, NamespaceDelimiter, _viewName);
            var type = _assembly.GetType(fullyQualifiedName);

            return type;
        }

        private static bool TryGetReplacementValue(string initialValue,
            ResourceManager resourceManager, out string replacementValue)
        {
            var placeHolderValue = initialValue.TrimStart(PlaceHolderStartChracter).TrimEnd(PlaceHolderEndChracter);
            replacementValue = resourceManager.GetString(placeHolderValue, CultureInfo.CurrentCulture);

            var haveReplacableValue = !string.IsNullOrEmpty(replacementValue);
            return haveReplacableValue;
        }

        #endregion
    }
```

So now we can load the file and manipulate the contents, but we do not have anything to orchestrate these two processes, and how do we serve the modified file back to the client? Well we need a new controller; the `JavascriptFileController` with an "Action" called `CulturisedResourceFile()` which taking a "controller name" and a "view name" for teh view we need localised strings for as parameters. The "Action" first checks the parameters for null or emptyness and will return a BadRequest Http Status code for any errors. We then load the javascript file content, and pas this the class responsible for localising the placeholder text. If the javascript resource file was not found then a `HttpNotFoundResult` is returned, and if there is any error then a `HttpStatusCodeResult` carrying an "InternalServerError" `HttpStatusCode` is returned. 

```java
    public class JavascriptFileController : Controller
    {
        // GET: JavascriptResourceFile
        [HttpGet]
        //[OutputCache(Duration = Duration.InSeconds.OneHour, Location=OutputCacheLocation.Client, NoStore=true)]    // PROD ONLY
        //[OutputCache(Duration = Duration.InSeconds.TenSeconds, Location=OutputCacheLocation.Client, NoStore=true)] // TEST  ONLY
        [OutputCache(Duration = Duration.InSeconds.OneSecond, Location=OutputCacheLocation.Client, NoStore=true)]  // DEV  ONLY
        public ActionResult CulturisedResourceFile(string controllerName, string viewName)
        {
            if (string.IsNullOrWhiteSpace(controllerName)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "controllerName not supplied");
            if (string.IsNullOrWhiteSpace(viewName)) return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "viewName not supplied");

            const string baseScriptFolderPath = @"\Scripts";
            try
            {
                var originalFileContent = new JavascriptResourceFileContentLoader(
                    baseScriptFolderPath: baseScriptFolderPath,
                    controllerName: controllerName,
                    viewName: viewName)
                    .Load()
                    .Output;

                var culturisedFileContent = new JavascriptResourceFileContentCulturiser(
                    content: originalFileContent,
                    controllerName: controllerName,
                    viewName: viewName)
                    .Culturise()
                    .Output;

                return JavaScript(culturisedFileContent);
            }
            catch (FileNotFoundException)
            {
                return new HttpNotFoundResult("The requested resource was not found on the server. ");
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, "The requested resource causes an error on the server. ");
            }
        }
    }

```

We also set an `OutputCache` attribute on the action to save this code being run excesively as the file is VERY unlikley to change except during code enhancements. Rather than having an almost meaningless magic number indicating how many seconds the cache should work for, consider using well named constants in a nested static classes. For eaxample, add a "Constants" folder to the root of the web application, and within it add the following nested static constants classes. You may want to comment out the OutputCache attribute altogether while debugging, and set it with a low number during testing and ramp it up in production.

```C#
    public static class Duration
    {
        public static class InSeconds
        {
            public const int OneHour = 36000;
            public const int TenSeconds = 10;
            public const int OneSecond = 1;
        }
    }
```

If you need alternative durations elsewhere in your application, you can add to these classes and define well names classes and constants for them. For example you may want to override the default session timeout of 20 minutes using `Duration.InMinutes.HalfAnHour`, with a value of `30`. Or you may prefer the convention switched slightly to be `Duration.HalfAnHour.InSeconds`?


Now in the "Home/Index" view in the sripts section we can remove the original referece to the "~/Scripts/Home/app.home.index.resource.js" file and instead add a script tag and set the source to call to the `CulturisedResourceFile` action of the `JavascriptFileController` controller.

```html
<script src="@Url.Action("CulturisedResourceFile", "JavascriptFileController", new {Controller = "Home", View = "Index"})"></script>
```

Test the application both with the `globalization` node in teh web config setting "de-DE" or your default culture. You should see the message changes as you do. Remeber to turn the output cash off or set it very low! Once tested you can simplify the "app.home.index.js" file greatly to just use the js resource file.

```javascript
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
```

And this now requires the simplifcation of the view too.

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
  <script src="@Url.Action("CulturisedResourceFile", "JavascriptFile", new {controllerName = "Home", viewName = "Index"})"></script>
  <script>
    app.home.index.promptUser();
  </script>
}
```

## Summary
And there you have it; one way to localise your javascript messages in ASP.Net MVC. There are probably other (better?) solutions to get to the same end result, but I thought I'd share with you all my way.

## Source code
The source code is available on my GitHub.