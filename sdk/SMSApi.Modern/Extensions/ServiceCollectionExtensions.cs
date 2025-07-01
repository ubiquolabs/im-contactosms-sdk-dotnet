using System.Net.Http;
using InteractuaMovil.ContactoSms.Api.Configuration;
using InteractuaMovil.ContactoSms.Api.Interfaces;
using InteractuaMovil.ContactoSms.Api.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace InteractuaMovil.ContactoSms.Api.Extensions;

/// <summary>
/// Dependency injection extensions for SMS API
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add SMS API services to the dependency injection container
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration section containing SMS API settings</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddSmsApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure options
        services.Configure<SmsApiOptions>(options => configuration.GetSection(SmsApiOptions.SectionName).Bind(options));
        
        // Validate options
        services.AddSingleton<IValidateOptions<SmsApiOptions>, SmsApiOptionsValidator>();

        // Register HTTP client with custom configuration
        services.AddHttpClient<ApiClient>((serviceProvider, client) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<SmsApiOptions>>().Value;
            
            // Configure proxy if specified
            if (options.Proxy != null)
            {
                var handler = new HttpClientHandler();
                if (!string.IsNullOrEmpty(options.Proxy.Address))
                {
                    handler.Proxy = new System.Net.WebProxy(options.Proxy.Address);
                    
                    if (!string.IsNullOrEmpty(options.Proxy.Username) && !string.IsNullOrEmpty(options.Proxy.Password))
                    {
                        handler.Proxy.Credentials = new System.Net.NetworkCredential(
                            options.Proxy.Username, 
                            options.Proxy.Password);
                    }
                    
                    handler.UseProxy = true;
                }
            }
        });

        // Register services
        services.AddScoped<ApiClient>();
        services.AddScoped<IMessages, MessagesService>();
        services.AddScoped<IContacts, ContactsService>();
        services.AddScoped<IGroups, GroupsService>();
        services.AddScoped<IAccounts, AccountsService>();
        services.AddScoped<ISmsApi, SmsApi>();

        return services;
    }

    /// <summary>
    /// Add SMS API services with manual configuration
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configureOptions">Action to configure SMS API options</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddSmsApi(this IServiceCollection services, Action<SmsApiOptions> configureOptions)
    {
        services.Configure(configureOptions);
        
        // Validate options
        services.AddSingleton<IValidateOptions<SmsApiOptions>, SmsApiOptionsValidator>();

        // Register HTTP client
        services.AddHttpClient<ApiClient>();

        // Register services
        services.AddScoped<ApiClient>();
        services.AddScoped<IMessages, MessagesService>();
        services.AddScoped<IContacts, ContactsService>();
        services.AddScoped<IGroups, GroupsService>();
        services.AddScoped<IAccounts, AccountsService>();
        services.AddScoped<ISmsApi, SmsApi>();

        return services;
    }
}

/// <summary>
/// Validates SMS API options
/// </summary>
public class SmsApiOptionsValidator : IValidateOptions<SmsApiOptions>
{
    public ValidateOptionsResult Validate(string? name, SmsApiOptions options)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ApiKey))
            errors.Add("ApiKey is required");

        if (string.IsNullOrWhiteSpace(options.SecretKey))
            errors.Add("SecretKey is required");

        if (string.IsNullOrWhiteSpace(options.ApiUrl))
            errors.Add("ApiUrl is required");
        else if (!Uri.TryCreate(options.ApiUrl, UriKind.Absolute, out _))
            errors.Add("ApiUrl must be a valid absolute URL");

        if (options.TimeoutSeconds <= 0)
            errors.Add("TimeoutSeconds must be greater than 0");

        if (errors.Count > 0)
            return ValidateOptionsResult.Fail(errors);

        return ValidateOptionsResult.Success;
    }
} 