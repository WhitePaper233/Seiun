using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Minio;
using Seiun.Entities;
using Seiun.Filters;
using Seiun.Services;
using Seiun.Utils;
using Nest;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IElasticClient>(provider =>
{
    var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
        .DefaultIndex("articles");
    return new ElasticClient(settings);
});

// Use custom filter for parameter validation
builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
builder.Services.AddControllers(options => { options.Filters.Add<ParamValidationFilter>(); });

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(Swagger.ConfigureSwaggerGen);

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
    };
});
builder.Services.AddAuthorization();
builder.Services.AddSingleton<IJwtService, JwtService>();

// Inject MinIO client
var minioConfig = builder.Configuration.GetSection("MinIO");
var minioClient = new MinioClient().WithEndpoint(minioConfig["Endpoint"])
    .WithCredentials(minioConfig["AccessKey"], minioConfig["SecretKey"]).Build();
builder.Services.AddSingleton(minioClient);

// Inject AI request service
builder.Services.AddSingleton<IAIRequestService, AIRequestService>();

// Inject repository service
builder.Services.AddScoped<IRepositoryService, RepositoryService>();
// Inject search service
builder.Services.AddScoped<IArticleSearchService, ArticleSearchService>();
// Inject current study session service
// 单例
builder.Services.AddSingleton<ICurrentStudySessionService, CurrentStudySessionService>();

// Use snake_case for JSON serialization
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
    options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:5173") // 你的前端地址
              .AllowAnyHeader()  // 允许所有请求头，包括 Authorization 头
              .AllowAnyMethod()  // 允许 GET、POST、PUT、DELETE 等
              .AllowCredentials()); // 允许前端携带 Cookie 或 Authorization 头
});

// Configure PostgreSQL database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SeiunDbContext>(options => { options.UseNpgsql(connectionString); });

var app = builder.Build();

// 子线程定时20小时自动清理session
Thread ClearSession = new(() =>
{
    var time = new System.Timers.Timer
    {
        Interval = 3600000 * 20
    };
    time.Elapsed += async (sender, args) =>
    {
        var currentStudySession = app.Services.GetService<CurrentStudySessionService>();
        if (currentStudySession != null)
        {
            var sessionRepository = app.Services.GetService<RepositoryService>()?.SessionRepository;
            var loggger = app.Services.GetService<ILogger>();
            if(sessionRepository != null && loggger!= null)
            {
                await currentStudySession.ClearSessionAsync(sessionRepository, loggger);
            }
        }
    };
    time.Start();
})
{
    IsBackground = true
};
ClearSession.Start();

if (app.Environment.IsDevelopment())
{
    // Set up Swagger
    app.UseSwagger();
    app.UseSwaggerUI();

    // Configure migration automatically
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<SeiunDbContext>();
    dbContext.Database.Migrate();

    // 示例代码（应用启动时运行一次）
    var elasticClient = scope.ServiceProvider.GetRequiredService<IElasticClient>();
    var createIndexResponse = await elasticClient.Indices.CreateAsync("articles", c => c
        .Map<ArticleSearchEntity>(m => m
            .Properties(props => props
                .Text(t => t
                    .Name(n => n.Article)  // 文章内容进行分词
                    .Analyzer("standard")   // 使用标准分析器
                )
                .Keyword(k => k
                    .Name(n => n.CreatorUserName) 
                )
                .Text(k => k
                    .Name(n => n.CreatorNickName)
                    .Analyzer("standard")
                )
                .Keyword(l => l
                    .Name(n => n.ArticleId))
            )
        )
    );
}

app.UseCors("AllowFrontend"); // 在 UseAuthorization 之前调用

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();