using System;
using System.IO;
using System.Text;

namespace Blogs.LocalisingJavascriptResourceFiles.UI.Helpers
{
    public class JavascriptResourceFileContentLoader
    {
        private const char BackSlash = '\\';
        private readonly string _baseScriptFolderPath;
        private readonly string _controllerName;
        private readonly string _resourceName;
        private string _resourceFilePath;
        public string Output { get; private set; }

        public JavascriptResourceFileContentLoader(string baseScriptFolderPath, string controllerName, string resourceName)
        {
            if (string.IsNullOrWhiteSpace(baseScriptFolderPath)) throw new ArgumentNullException("baseScriptFolderPath");
            if (string.IsNullOrWhiteSpace(controllerName)) throw new ArgumentNullException("controllerName");
            if (string.IsNullOrWhiteSpace(resourceName)) throw new ArgumentNullException("resourceName");

            _baseScriptFolderPath = baseScriptFolderPath;
            _controllerName = controllerName;
            _resourceName = resourceName;
        }

        public JavascriptResourceFileContentLoader Load()
        {
            BuildFilePath();
            EnsureFileExists();
            LoadFile();

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
            pathBuilder.Append(_resourceName.ToLower());
            pathBuilder.Append(".resource.js");

            _resourceFilePath = pathBuilder.ToString();
        }

        private void EnsureFileExists()
        {
            var fileExists = File.Exists(_resourceFilePath);
            if (!fileExists) throw new FileNotFoundException(_resourceFilePath + " does not exist. ");
            throw new NotImplementedException();
        }

        private void LoadFile()
        {
            throw new NotImplementedException();
        }
    }
}