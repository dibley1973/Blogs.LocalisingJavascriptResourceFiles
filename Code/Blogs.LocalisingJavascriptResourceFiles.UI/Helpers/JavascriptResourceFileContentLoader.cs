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
}