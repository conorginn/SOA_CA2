using Library.Infrastructure.Data;
using Library.Application.Interfaces;
using Library.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace Library.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IAuthorService, AuthorService>();
            builder.Services.AddScoped<IBookService, BookService>();

            builder.Services.AddDbContext<LibraryDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("LibraryDb")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
