using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Notas.Startup))]
namespace Notas
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
