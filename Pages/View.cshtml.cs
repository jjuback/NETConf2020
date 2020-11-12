using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using ElectronNET.API;
using ElectronNET.API.Entities;

namespace Processes.Pages
{
    public class ViewModel : PageModel
    {
        public ViewModel()
        {
            PropertyList = new string[] {
                "Id",
                "ProcessName",
                "PriorityClass",
                "WorkingSet64"
            };
        }
        
        public Process Process { get; set; }
        
        public string[] PropertyList { get; set; }
        
        public void OnGet(int? id)
        {
            if (id.HasValue)
            {
                Process = Process.GetProcessById(id.Value);
            }
        
            if (Process == null)
            {
                NotFound();
            }
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id.HasValue)
            {
                Process = Process.GetProcessById(id.Value);
            }
        
            if (Process == null)
            {
                return NotFound();
            }
        
            const string msg = "Are you sure you want to kill this process?";
            MessageBoxOptions options = new MessageBoxOptions(msg);
            options.Type = MessageBoxType.question;
            options.Buttons = new string[] {"No", "Yes"};
            options.DefaultId = 1;
            options.CancelId = 0;
            MessageBoxResult result = await Electron.Dialog.ShowMessageBoxAsync(options);
        
            if (result.Response == 1)
            {
                Process.Kill();
                return RedirectToPage("Index");
            }
        
            return Page();
        }
    }
}
