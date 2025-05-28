using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.DependencyInjection;
using Nonuso.Application.IServices;
using Nonuso.Infrastructure.Secret;
using Nonuso.Infrastructure.Storage.Services;

namespace Nonuso.Infrastructure.Storage
{
    public static partial class IServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureS3Storage(this IServiceCollection services, ISecretManager secretManager)
        {
            services.AddSingleton<IAmazonS3>(provider =>
            {
                var awsOptions = secretManager.GetS3Settings();
                var credentials = new BasicAWSCredentials(awsOptions.AccessKey, awsOptions.SecretKey);

                var config = new AmazonS3Config
                {
                    ServiceURL = awsOptions.Endpoint,
                    ForcePathStyle = true,
                    UseHttp = false
                };

                return new AmazonS3Client(credentials, config);
            });

            services.AddTransient<IS3StorageService, S3StorageService>();

            return services;
        }
    }
}
