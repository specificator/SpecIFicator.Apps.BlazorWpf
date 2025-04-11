using MDD4All.FileAccess.Contracts;
using MDD4All.FileAccess.WPF;
using MDD4All.SpecIF.DataProvider.Base;
using MDD4All.SpecIF.DataProvider.Base.DataStreams;
using MDD4All.SpecIF.DataProvider.Contracts;
using MDD4All.SpecIF.DataProvider.Contracts.DataStreams;
using MDD4All.UI.BlazorComponents.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SpecIFicator.Apps.BlazorWPF.PluginSupport;
using SpecIFicator.Framework.Configuration;
using SpecIFicator.Framework.PluginManagement;
using SpecIFicator.Framework.Services;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using VisNetwork.Blazor;

namespace SpecIFicator.Apps.BlazorWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            SetTitle();

            InitializeServices();

            Loaded += OnMainWindowLoaded;
        }

        private void InitializeServices()
        {
            var services = new ServiceCollection();
            services.AddWpfBlazorWebView();
//#if DEBUG
            services.AddBlazorWebViewDeveloperTools();
            //#endif

            services.AddScoped<BrowserDimensionService>();

            services.AddLocalization(options =>
            {

                options.ResourcesPath = "Resources";
            });

            List<CultureInfo> supportedCultures = new List<CultureInfo>
            {
               new CultureInfo("en"),
               new CultureInfo("de")
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture =
                   new Microsoft.AspNetCore.Localization.RequestCulture("de");
                options.SupportedUICultures = supportedCultures;
            });

            services.AddSingleton<ISpecIfDataProviderFactory>(factory =>
            {
                return new SpecIfDataProviderFactory();
            });

            services.AddSingleton<ISpecIfStreamDataPublisherProvider>(_ =>
            {
                return new SpecIfStreamDataPublisherProvider();
            });

            services.AddSingleton<ISpecIfStreamDataSubscriberProvider>(_ =>
            {
                return new SpecIfStreamDataSubscriberProvider();
            });

            services.AddScoped<ClipboardDataProvider>();

            services.AddSingleton<DragDropDataProvider>();

            services.AddSingleton<IFileSaver>(fileSaver =>
            {
                return new WpfFileSaver();
            });

            services.AddSingleton<IFileLoader>(fileLoader =>
            {
                return new WpfFileLoader();
            });

            services.AddVisNetworkServer();

            services.AddHttpClient();

            Resources.Add("services", services.BuildServiceProvider());

            // SpecIFicator framework initialization
            PluginCssReferenceManager.CreateCssReferences();

            DynamicConfigurationManager.LoadConfiguration();

            PluginManager.LoadPlugins();
        }
    
        private void SetTitle()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fileVersionInfo = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fileVersionInfo.FileVersion;

            Title = "SpecIFicator | " + version;
        }

        private async void OnMainWindowLoaded(object sender, RoutedEventArgs e)
        {
            await blazorWebView.WebView.EnsureCoreWebView2Async();
            blazorWebView.WebView.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
        }
    }
}
