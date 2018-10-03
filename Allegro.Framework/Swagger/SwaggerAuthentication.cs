using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;

namespace Allegro.Framework.Swagger
{
    public class SwaggerAuthentication
    {
        public string Name { get; set; }
        public ApiKeyScheme ApiKeyScheme { get; set; }
        public Dictionary<string, IEnumerable<string>> Requirement { get; set; }
    }
}
