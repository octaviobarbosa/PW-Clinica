using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PW_Clinica.Startup))]
namespace PW_Clinica
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
