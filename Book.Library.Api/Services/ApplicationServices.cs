using Book.Library.Business.Services;
using Book.Library.Data.Repositories;

namespace Book.Library.Api.Services
{
    public static class ApplicationServices
    {
        public static void AddServices(IServiceCollection services)
        {

            services.AddTransient<IGenericService, GenericService>();

            services.AddTransient<IGenericRepository, GenericRepository>();

        }
    }
}
