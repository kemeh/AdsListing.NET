using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdsListing.Startup))]
namespace AdsListing
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
