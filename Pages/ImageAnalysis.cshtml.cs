using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class ImageAnalysisModel : PageModel
{
    private readonly IConfiguration _configuration;

    public ImageAnalysisModel(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [BindProperty]
    public IFormFile ImageFile { get; set; }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ImageFile == null || ImageFile.Length == 0)
        {
            ModelState.AddModelError(string.Empty, "Please upload a valid image file.");
            return Page();
        }

        // Retrieve API key and endpoint from appsettings.json
        string apiKey = _configuration["AzureCognitiveServices:ComputerVisionKey"];
        string endpoint = _configuration["AzureCognitiveServices:ComputerVisionEndpoint"];

        var client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(apiKey))
        {
            Endpoint = endpoint
        };

        using var stream = ImageFile.OpenReadStream();
        var analysis = await client.AnalyzeImageInStreamAsync(
            stream, new List<VisualFeatureTypes?> { VisualFeatureTypes.Description });

        ViewData["Description"] = analysis.Description.Captions.FirstOrDefault()?.Text ?? "No description found.";
        return Page();
    }
}
