using Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR(e => {
                e.MaximumReceiveMessageSize = 102400000;
            });

builder.Services.AddCors(options => {
                options.AddPolicy("CORS", builder => {
                    builder.WithOrigins(new string []
                    {
                        "https://localhost:5001",
                        "http://localhost:5001",
                        "https://127.0.0.1:5001",
                        "http://127.0.0.1:5001",
                        "https://localhost:5500",
                        "http://localhost:5500",
                        "https://127.0.0.1:5500",
                        "http://127.0.0.1:5500",
                        "https://localhost:3000",
                        "http://localhost:3000",
                        "https://127.0.0.1:3000",
                        "http://127.0.0.1:3000",
                        "https://localhost:7220",
                        "http://localhost:7220",
                        "https://127.0.0.1:7220",
                        "http://127.0.0.1:7220"

 

                    })
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            });

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors("CORS");


app.MapControllers();


app.Run();
app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<Notif>("/hubs/notif");
            });
