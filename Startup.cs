using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CartModuleApi.EntityFrameWork;
using Microsoft.EntityFrameworkCore;
using CartModuleApi.Services;
using System.Collections.Generic;
using CartModuleApi.Models;
using System;

namespace CartModuleApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddDbContext<EnityFramWorkDbContext>(opt => opt.UseInMemoryDatabase("CartDB"));
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddSwaggerDocument();
        }

        private static void AddTestData(EnityFramWorkDbContext context)
        {
            IList<Product> products = new List<Product>();
            products.Add(new Product { Id = 1, Name = "Water Glass", Price = 10.00M, Photo = "", Desciption = "Water Glass for drinking" });
            products.Add(new Product { Id = 2, Name = "Jug Water Glass", Price = 12.00M, Photo = "", Desciption = "Water Glass for drinking" } );
            products.Add(new Product { Id = 3, Name = "Green Water Glass", Price = 13.00M, Photo = "", Desciption = "Water Glass for drinking" });
            products.Add(new Product { Id = 4, Name = "Red Water Glass", Price = 14.00M, Photo = "", Desciption = "Water Glass for drinking" });
            products.Add(new Product { Id = 5, Name = "Yellow Water Glass", Price = 15.00M, Photo = "", Desciption = "Water Glass for drinking" });
            context.Products.AddRange(products);
            //context.SaveChanges();

            IList<CartItem> cartItems = new List<CartItem>();
            cartItems.Add(new CartItem { Id = 1, ProductId = 1, Quantity = 2, UserId = 1 });
            cartItems.Add(new CartItem { Id = 2, ProductId = 5, Quantity = 1, UserId = 1 });
            context.CartItems.AddRange(cartItems);

            context.SaveChanges();
        }
        
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                var context = serviceProvider.GetService<EnityFramWorkDbContext>();
                AddTestData(context);
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseOpenApi();
            app.UseSwaggerUi3();
           
        }
    }
}
