using Amazon.SimpleSystemsManagement.Model;
using Amazon.SimpleSystemsManagement;

namespace JbHiFi.Services
{
    public class AwsParameterService
    {
        private readonly IAmazonSimpleSystemsManagement _ssm;

        public AwsParameterService(IAmazonSimpleSystemsManagement ssm)
        {
            _ssm = ssm;
        }

        public async Task<List<string>> GetArrayParameterAsync(string name)
        {
            var request = new GetParameterRequest
            {
                Name = name,
                WithDecryption = true
            };

            var response = await _ssm.GetParameterAsync(request);
            var raw = response.Parameter?.Value;

            return raw?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                       .Select(x => x.Trim())
                       .ToList() ?? new List<string>();
        }
    }
}
