using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LacmusApp.Models.ML;
using Serilog;

namespace LacmusApp.Extensions
{
    public static class MLModelConfigExtension
    {
        public static string GetDockerTag(this MLModelConfig config)
        {
            return $"{config.ApiVersion}.{config.ModelVersion}-{GetTagSuffix(config)}";
        }
        public static string GetDockerTag(uint apiVersion, uint modelVersion, MLModelType type)
        {
            return $"{apiVersion}.{modelVersion}-{GetTagSuffix(type)}";
        }
        
        public static async Task Save(this MLModelConfig config, string path)
        {
            try
            {
                var str = JsonConvert.SerializeObject(config);
                var dir = Path.GetDirectoryName(path);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                await File.WriteAllTextAsync(path, str);
                Log.Debug($"Config saved to {path}.");
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to save config to file {path}.", e);
            }
        }
        
        public static async Task<MLModelConfig> Load(string path)
        {
            try
            {
                if (!File.Exists(path))
                    throw new Exception($"unable to load config file. Bo such file {path}.");
                var str = await File.ReadAllTextAsync(path);
                return JsonConvert.DeserializeObject<MLModelConfig>(str);
            }
            catch (Exception e)
            {
                throw new Exception($"unable to load config from {path}.", e);
            }
        }
        public static string GetTagSuffix(this MLModelConfig config)
        {
            switch (config.Type)
            {
                case MLModelType.Cpu:
                    return "cpu";
                case MLModelType.CpuNoAvx:
                    return "cpu-no-avx"; 
                case MLModelType.Gpu:
                    return "gpu";
                //case MLModelType.Tpu:
                //    return "tpu"; 
                //case MLModelType.TpuNoAvx:
                //    return "tpu-no-avx";
                //case MLModelType.TpuCpu:
                //    return "tpu-cpu";
                //case MLModelType.TpuCpuNoAvx:
                //    return "tpu-cpu-no-avx";
                //case MLModelType.TpuGpu:
                //    return "tpu-gpu";
                default:
                    throw new Exception($"Invalid model type: {config.Type.ToString()}.");
            }
        }
        public static string GetTagSuffix(MLModelType type)
        {
            switch (type)
            {
                case MLModelType.Cpu:
                    return "cpu";
                case MLModelType.CpuNoAvx:
                    return "cpu-no-avx"; 
                case MLModelType.Gpu:
                    return "gpu";
                //case MLModelType.Tpu:
                //    return "tpu"; 
                //case MLModelType.TpuNoAvx:
                //    return "tpu-no-avx";
                //case MLModelType.TpuCpu:
                //    return "tpu-cpu";
                //case MLModelType.TpuCpuNoAvx:
                //    return "tpu-cpu-no-avx";
                //case MLModelType.TpuGpu:
                //    return "tpu-gpu";
                default:
                    throw new Exception($"Invalid model type: {type.ToString()}.");
            }
        }
    }
}