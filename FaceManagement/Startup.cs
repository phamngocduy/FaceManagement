using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FaceManagement.Startup))]
namespace FaceManagement
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
