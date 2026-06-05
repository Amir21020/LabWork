var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "TimeTracker API",
        Version = "v1",
        Description = "API для веб-приложения учета рабочего времени сотрудников компании",
        Contact = new Microsoft.OpenApi.OpenApiContact
        {
            Name = "Radmir Mergaliev",
            Url = new Uri("https://t.me/Mergalievr")
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty; 
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
