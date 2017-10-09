using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(jdean_bugtracker.Startup))]
namespace jdean_bugtracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
