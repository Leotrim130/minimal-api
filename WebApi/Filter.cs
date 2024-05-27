using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class Filter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var excludedEndpoints = new List<string>
        {
            "/",
            "/check"
        };

        foreach (var excludedEndpoint in excludedEndpoints)
        {
            if (swaggerDoc.Paths.ContainsKey(excludedEndpoint))
            {
                swaggerDoc.Paths.Remove(excludedEndpoint);
            }
        }

    }
}
