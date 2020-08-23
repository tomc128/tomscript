using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using TDSStudios.TomScript.Core;

namespace TomScriptWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string InputCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public string GeneratedCode { get; set; }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string submit)
        {
            if (submit == "Compile")
            {
                if (string.IsNullOrWhiteSpace(InputCode)) return null;

                var compiler = new TomScriptCompiler(InputCode);
                GeneratedCode = compiler.Compile();
            }
            else if (submit == "Download")
            {
                if (string.IsNullOrWhiteSpace(GeneratedCode)) return null;

                return DownloadGeneratedCodeFile();
            }
            return null;
        }



        public IActionResult DownloadGeneratedCodeFile()
        {
            var data = Encoding.UTF8.GetBytes(GeneratedCode);
            var content = new MemoryStream(data);
            var contentType = "application/octet-stream";
            string filename = "output.py";
            return File(content, contentType, filename);
        }


    }
}
