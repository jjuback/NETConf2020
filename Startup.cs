using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ElectronNET.API;
using ElectronNET.API.Entities;
using System.Runtime.InteropServices;

namespace Processes
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            if (HybridSupport.IsElectronActive)
            {
                CreateWindow();
            }
        }

        private void CreateMenu()
        {
            bool isMac = RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
            MenuItem[] menu = null;
        
            MenuItem[] appMenu = new MenuItem[]
            {
                new MenuItem { Role = MenuRole.about },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.services },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.hide },
                new MenuItem { Role = MenuRole.hideothers },
                new MenuItem { Role = MenuRole.unhide },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.quit }
            };
        
            MenuItem[] fileMenu = new MenuItem[]
            {
                new MenuItem { Label = "Save As...", Type = MenuType.normal, Click = async () => {
                    var mainWindow = Electron.WindowManager.BrowserWindows.First();
                    var options = new SaveDialogOptions() {
                        Filters = new FileFilter[] { new FileFilter{ Name ="CSV Files", Extensions = new string[] { "csv" } }
                    }};
                    string result = await Electron.Dialog.ShowSaveDialogAsync(mainWindow, options);
                    if (!string.IsNullOrEmpty(result))
                    {
                        string url = $"http://localhost:{BridgeSettings.WebPort}/SaveAs?path={result}";
                        mainWindow.LoadURL(url);
                    }
                }},
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = isMac ? MenuRole.close : MenuRole.quit }
            };
        
            MenuItem[] viewMenu = new MenuItem[]
            {
                new MenuItem { Role = MenuRole.reload },
                new MenuItem { Role = MenuRole.forcereload },
                new MenuItem { Role = MenuRole.toggledevtools },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.resetzoom },
                new MenuItem { Role = MenuRole.zoomin },
                new MenuItem { Role = MenuRole.zoomout },
                new MenuItem { Type = MenuType.separator },
                new MenuItem { Role = MenuRole.togglefullscreen }
            };
        
            if (isMac)
            {
                menu = new MenuItem[]
                {
                    new MenuItem { Label = "Electron", Type = MenuType.submenu, Submenu = appMenu },
                    new MenuItem { Label = "File", Type = MenuType.submenu, Submenu = fileMenu },
                    new MenuItem { Label = "View", Type = MenuType.submenu, Submenu = viewMenu }
                };
            }
            else
            {
                menu = new MenuItem[]
                {
                    new MenuItem { Label = "File", Type = MenuType.submenu, Submenu = fileMenu },
                    new MenuItem { Label = "View", Type = MenuType.submenu, Submenu = viewMenu }
                };
            }
        
            Electron.Menu.SetApplicationMenu(menu);
        }

        private async void CreateWindow()
        {
            CreateMenu();
            var window = await Electron.WindowManager.CreateWindowAsync();
            window.OnClosed += () => {
                Electron.App.Quit();
            };
        }
    }
}
