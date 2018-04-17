using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RojgarmitraSolution.Startup))]
namespace RojgarmitraSolution
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
