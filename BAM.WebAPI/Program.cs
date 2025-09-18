using BAM.DataAccessLayer.Interfaces;
using BAM.DataAccessLayer.Mapping;
using BAM.DataAccessLayer.Repos;
using BAM.DataAccessLayer.UnitOfWork;
using BAM.Domain.Calculators;
using BAM.Infra.Database;
using BAM.Services;
using BAM.Services.Interfaces;
using BAM.WebAPI;
using BAM.WebAPI.Mapping;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext (InMemory for simplicity; replace with real provider/config as needed)
builder.Services.AddDbContext<BAMDbContext>(options =>
    options.UseInMemoryDatabase("BAMDb"));
builder.Services.AddAutoMapper(cfg => { }, typeof(AccountMappingProfile), typeof(AccountDtoMappingProfile));

builder.Services.AddScoped<IAccountRepo, AccountRepo>();
builder.Services.AddScoped<ILoanRepo, LoanRepo>();
builder.Services.AddScoped<ICustomerRepo, CustomerRepo>();

builder.Services.AddScoped<ILoanService, LoanService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IInterestRateCalc, InterestRateCalc>(rc => new InterestRateCalc(InterestRateRules.GetDefaultRules()));
builder.Services.AddScoped<IInterestRateRule, RangeInterestRateRule>(rir => 
    new RangeInterestRateRule(minInclusive: 20, maxExclusive: 50, new Dictionary<int, decimal>
    {
        [1] = 20m,
        [3] = 15m,
        [5] = 10m
    }));
builder.Services.AddScoped<IInterestRateRule, RangeInterestRateRule>(rir =>
    new RangeInterestRateRule(minInclusive: 50, maxExclusive: 101, new Dictionary<int, decimal>
    {
        [1] = 12m,
        [3] = 8m,
        [5] = 5m
    }));
builder.Services.AddScoped<IInterestService, InterestService>();
builder.Services.AddScoped<IUnitOfWork, ApplyForLoanUnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseErrorHandling();

app.UseAuthorization();

app.MapControllers();

app.Run();

