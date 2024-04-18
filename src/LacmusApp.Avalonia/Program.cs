﻿using System;
using System.IO;
using Avalonia;
using Avalonia.ReactiveUI;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;
using Serilog;

namespace LacmusApp.Avalonia
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine($"Lacmus desktop application. Version {GetVersion()}.");
            Console.WriteLine("Copyright (c) 2022 Lacmus Foundation <gosha20777@live.ru>.");
            Console.WriteLine("Github page: https://github.com/lacmus-foundation.");
            Console.WriteLine("Powered by ODS <https://ods.ai>.");
            Console.WriteLine("This program comes with ABSOLUTELY NO WARRANTY;");
            Console.WriteLine("This is free software, and you are welcome to redistribute it under GNU GPL license;\nClick `help` -> `about' for details.");
            Console.WriteLine("------------------------------------");
            //Resources.Console.Culture = new CultureInfo("ru");
            //Console.WriteLine(Resources.Console.Greeting);
            try
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Exited with fatal error.");
            }
        }

        public static string GetVersion()
        {
            if (typeof(Program).Assembly.GetName().Version.Revision != 0)
                 return $"{typeof(Program).Assembly.GetName().Version.Major}.{typeof(Program).Assembly.GetName().Version.Minor}.{typeof(Program).Assembly.GetName().Version.Build} preview-{typeof(Program).Assembly.GetName().Version.Revision}";
            return $"{typeof(Program).Assembly.GetName().Version.Major}.{typeof(Program).Assembly.GetName().Version.Minor}.{typeof(Program).Assembly.GetName().Version.Build}";
        }
        
        private static AppBuilder BuildAvaloniaApp()
        {
            //FOR TEST IN VirtualBox
            /*
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return AppBuilder.Configure<App>()
                    .UsePlatformDetect()
                    .With(new Win32PlatformOptions {EnableMultitouch = true, AllowEglInitialization = true})
                    .With(new AvaloniaNativePlatformOptions {UseGpu = false})
                    .UseReactiveUI()
                    .LogToDebug();
            */

            return AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .With(new Win32PlatformOptions {EnableMultitouch = true, AllowEglInitialization = true})
                .With(new SkiaOptions{ MaxGpuResourceSizeBytes = 1024 * 1024 * 80})
                .UseReactiveUI()
                .LogToTrace()
                .WithIcons(container => container
                    .Register<FontAwesomeIconProvider>());
        }
        
        /*
        private static AppBuilder BuildAvaloniaApp()
        {
            bool useGpuLinux = true;

            var result = AppBuilder.Configure<App>();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                result
                    .UseWin32()
                    .UseSkia()
                    .UsePlatformDetect();
            }
            else
            {
                result.UsePlatformDetect();
            }

            // TODO remove this overriding of RenderTimer when Avalonia 0.9 is released.
            // fixes "Thread Leak" issue in 0.8.1 Avalonia.
            var old = result.WindowingSubsystemInitializer;

            result.UseWindowingSubsystem(() =>
            {
                old();

                AvaloniaLocator.CurrentMutable.Bind<IRenderTimer>().ToConstant(new DefaultRenderTimer(60));
            });
            
            result.UseReactiveUI();

            AvaloniaLocator.CurrentMutable.Bind<INeuroModel>().ToConstant(new NeuroModel());
            

            return result
                .With(new Win32PlatformOptions { AllowEglInitialization = true, UseDeferredRendering = true })
                .With(new X11PlatformOptions { UseGpu = useGpuLinux, WmClass = "lacmus" })
                .With(new AvaloniaNativePlatformOptions { UseDeferredRendering = true, UseGpu = true })
                .With(new MacOSPlatformOptions { ShowInDock = true });
        }
        */
    }
}
