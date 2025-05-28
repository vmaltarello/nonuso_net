namespace Nonuso.Infrastructure.Secret
{
    public interface ISecretManager
    {
        string GetConnectionString(string name);
        string GetJwtSecret();
        (string AccessKey, string SecretKey, string BucketName, string Endpoint) GetS3Settings();
        (string RestApiKey, string AppId) GetOneSignalSettings();
    }

    internal class SecretManager() : ISecretManager
    {
        public string GetConnectionString(string name)
        {
            string secretPath = name.ToLower() switch
            {
                "dbconnection" => "/run/secrets/db_connection",
                "redis" => "/run/secrets/redis_connection",
                _ => string.Empty
            };

            return File.ReadAllText(secretPath).Trim();
        }

        public string GetJwtSecret()
        {
            const string secretPath = "/run/secrets/jwt_secret";
            return File.ReadAllText(secretPath).Trim();

        }

        public (string AccessKey, string SecretKey, string BucketName, string Endpoint) GetS3Settings()
        {
            string accessKey, secretKey, bucketName, endpoint;

            accessKey = File.ReadAllText("/run/secrets/s3_access_key").Trim();

            secretKey = File.ReadAllText("/run/secrets/s3_secret_key").Trim();
            bucketName = File.ReadAllText("/run/secrets/s3_bucket_name").Trim();
            endpoint = File.ReadAllText("/run/secrets/s3_endpoint").Trim();

            return (
                AccessKey: accessKey,
                SecretKey: secretKey,
                BucketName: bucketName,
                Endpoint: endpoint
            );
        }

        public (string RestApiKey, string AppId) GetOneSignalSettings()
        {
            string restApiKey, appId;

            restApiKey = File.ReadAllText("/run/secrets/onesignal_api_key").Trim();

            appId = File.ReadAllText("/run/secrets/onesignal_app_id").Trim();

            return (RestApiKey: restApiKey, AppId: appId);
        }
    }
}
