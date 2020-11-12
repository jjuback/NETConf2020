using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Processes.Pages
{
    public class SaveAsModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync(string path)
        {
            System.IO.StringWriter writer = new System.IO.StringWriter();
            writer.WriteLine("Id,Process Name,Physical Memory");
        
            var items = Process.GetProcesses().Where(p => !String.IsNullOrEmpty(p.ProcessName)).ToList();
            items.ForEach(p => {
                writer.Write(p.Id);
                writer.Write(",");
                writer.Write(p.MainModule != null ? p.MainModule.ModuleName : p.ProcessName);
                writer.Write(",");
                writer.WriteLine(p.WorkingSet64);
            });
        
            await System.IO.File.WriteAllTextAsync(path, writer.ToString());
            return RedirectToPage("Index");
        }
    }
}
