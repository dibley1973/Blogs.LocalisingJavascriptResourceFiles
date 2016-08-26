using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Blogs.LocalisingJavascriptResourceFiles.UI.Startup))]
namespace Blogs.LocalisingJavascriptResourceFiles.UI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
