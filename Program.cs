using ProgClaims.Services;

namespace ProgClaims
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Register TableService for handling lecturer claims in Azure Table Storage
            builder.Services.AddSingleton(new TableService(
                builder.Configuration.GetConnectionString("AzureStorage"),
                "LecturerClaims" // The table name is "LecturerClaims"
            ));

            // Register FileService for handling file uploads to Azure File Storage
            builder.Services.AddSingleton(new FileService(
                builder.Configuration.GetConnectionString("AzureStorage"),
                "lecturerfile" // The file share name is "lecturerfile"
            ));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Set default routing pattern for controllers and actions
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
