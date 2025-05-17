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

// using Nest;

var builder = WebApplication.CreateBuilder(args);

// builder.Services.AddSingleton<IElasticClient>(_ =>
// {
//     var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
//         .DefaultIndex("articles");
//     return new ElasticClient(settings);
// });

// Use custom filter for parameter validation
builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
builder.Services.AddControllers(options => { options.Filters.Add<ParamValidationFilter>(); });

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(Swagger.ConfigureSwaggerGen);

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
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
builder.Services.AddSingleton<IAiRequestService, AiRequestService>();

// Inject repository service
builder.Services.AddScoped<IRepositoryService, RepositoryService>();
// Inject search service
// builder.Services.AddScoped<IArticleSearchService, ArticleSearchService>();
// Inject current study session service
// 单例 会话
builder.Services.AddSingleton<ICurrentStudySessionService, CurrentStudySessionService>();
// 定时清理Session
builder.Services.AddHostedService<ClearSessionTimedService>(); // 注册后台任务
// 单例 ai生成任务中用户
builder.Services.AddSingleton<ICurrentGenerateTaskService, CurrentGenerateTaskService>();

// Use snake_case for JSON serialization
builder.Services.AddControllers().AddJsonOptions(options => {
	options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
	options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.SnakeCaseLower;
});

// 阻止循环引用
builder.Services.AddControllers().AddJsonOptions(options => {
	options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});


builder.Services.AddCors(options => {
	options.AddPolicy("AllowAll",
	configurePolicy: policy => policy.AllowAnyOrigin() // 允许任何来源
		.AllowAnyMethod() // 允许任何请求方法
		.AllowAnyHeader()); // 允许任何请求头
});


// Configure PgSQL database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<SeiunDbContext>(options => { options.UseNpgsql(connectionString); });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	// Set up Swagger
	app.UseSwagger();
	app.UseSwaggerUI();

	// Configure migration automatically
	// using var scope = app.Services.CreateScope();
	// var dbContext = scope.ServiceProvider.GetRequiredService<SeiunDbContext>();
	// dbContext.Database.Migrate();

	// 配置 ElasticSearch 分词方式 
	// var elasticClient = scope.ServiceProvider.GetRequiredService<IElasticClient>();
	// 先检查索引是否存在
	// var indexExistsResponse = await elasticClient.Indices.ExistsAsync("articles");
	// if (!indexExistsResponse.Exists)
	// {
	//     var createIndexResponse = await elasticClient.Indices.CreateAsync("articles", c => c
	//         .Map<ArticleSearchEntity>(m => m
	//             .Properties(props => props
	//                 .Text(t => t
	//                         .Name(n => n.Article) // 文章内容进行分词
	//                         .Analyzer("standard") // 使用标准分析器
	//                 )
	//                 .Keyword(k => k
	//                     .Name(n => n.CreatorUserName)
	//                 )
	//                 .Text(k => k
	//                     .Name(n => n.CreatorNickName)
	//                     .Analyzer("standard")
	//                 )
	//                 .Keyword(l => l
	//                     .Name(n => n.ArticleId))
	//             )
	//         )
	//     );
	//     var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
	//     if (createIndexResponse.IsValid)
	//         logger.LogInformation("Elasticsearch 索引创建成功！");
	//     else
	//         logger.LogError("Elasticsearch 索引创建失败: {Reason}", createIndexResponse.OriginalException?.Message);
	// }
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
