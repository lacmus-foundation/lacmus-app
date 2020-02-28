using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RescuerLaApp.Models.Photo;
using RescuerLaApp.Extensions;
using RescuerLaApp.ViewModels;

namespace RescuerLaApp.Models.ML
{
    public class MLModel : IMLModel
    {
        private readonly MLModelConfig _config;
        private Docker.Docker _docker;
        private RestApiClient _client;
        private string _id;

        public MLModel(MLModelConfig config)
        {
            _docker = new Docker.Docker();
            _id = "";
            _config = config;
        }
        
        public MLModelConfig Config => _config;

        public async Task Init()
        {
            try
            {
                _client = new RestApiClient(_config.Url);
            
                Console.WriteLine("INFO: Checking ml model...");
                var status = await _client.GetStatusAsync();
                if (status != null && status.Contains("server is running",StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("INFO: ml model is ready.");
                    return;
                }
            
                Console.WriteLine("INFO: Creating container...");
                _id = await _docker.CreateContainer(_config.Image);
            
                Console.WriteLine($"INFO: Running container {_id}...");
                if (await _docker.Run(_id))
                {
                    Console.WriteLine("INFO: Container runs. Loading ml model...");
                    var startTime = DateTime.Now;
                    //wait no more then 10 min.
                    while ((DateTime.Now - startTime).TotalMinutes < 10)
                    {
                        // Provide a 100ms startup delay
                        await Task.Delay(TimeSpan.FromMilliseconds(100));
                        status = await _client.GetStatusAsync();
                        if (status != null && status.Contains("server is running",StringComparison.InvariantCultureIgnoreCase))
                        {
                            Console.WriteLine("INFO: ml model is ready.");
                            return;
                        }   
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unable to init ml model", e);
            }
        }

        public async Task<List<Object>> Predict(PhotoViewModel photoViewModel)
        {
            try
            {
                DateTime startTime = DateTime.Now;
                var objects = new List<Object>();
                var status = await _client.GetStatusAsync();
                if (status == null || !status.Contains("server is running"))
                    throw new Exception("ml model is not initialize: server is not running");
            
                var request = new MLRequest
                {
                    Data = await PhotoVmToByteArray(photoViewModel)
                };
            
                var jsonRequest = JsonConvert.SerializeObject(request);
                var response = JsonConvert.DeserializeObject<MLResponse>(
                    await _client.PostAsync(jsonRequest, "/image")); 
                
                DateTime endTime = DateTime.Now;
                Console.WriteLine($"INFO: file {photoViewModel.Path} processed.\n\tTime: {endTime-startTime}.\n\tObjects found: {response.Objects.Count}.");
                
                foreach (var responseObject in response.Objects)
                {
                    objects.Add(new Object
                    {
                        Box = new Box
                        {
                            Xmax = responseObject.Xmax,
                            Xmin = responseObject.Xmin,
                            Ymax = responseObject.Ymax,
                            Ymin = responseObject.Ymin
                        },
                        Name = "Pedestrian"
                    });
                }
                return objects;
            }
            catch (Exception e)
            {
                throw new Exception("Con not predict photo.", e);
            }
        }

        public async Task Stop()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(_id))
                {
                    Console.WriteLine("WARN: the container id is not set up, so the model was not stopped. Is ml model was running manually?");
                    return;
                }
                await _docker.Stop(_id);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to stop ml model.", e);
            }
        }

        public async Task Download()
        {
            try
            {
                await _docker.Initialize(_config.Image, _config.Accaunt);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to download ml model.", e);
            }
        }

        public async Task Remove()
        {
            try
            {
                await _docker.StopAll(_config.Image.Name);
                await _docker.Remove(_config.Image);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to remove ml model.", e);
            }
        }

        public static async Task<List<uint>> GetInstalledVersions(MLModelConfig config)
        {
            try
            {
                using (var docker = new Docker.Docker())
                {
                    var versions = new List<uint>();
                    var tags = await docker.GetInstalledTags(config.Image.Name);
                    foreach (var t in tags)
                    {
                        try
                        {
                            var rex = new Regex($"{config.ApiVersion}\\.[0-9]+-{config.GetTagSuffix()}");
                            if (!rex.IsMatch(t))
                            {
                                Console.WriteLine($"WARN: local model with tag {t} installed but not used.\n"+
                                                  $"You can remove it by using command `docker rmi -f {config.Image.Name}:{t}`");
                                continue;
                            }
                            versions.Add(uint.Parse(t.Split('.').Last().Split('-').First()));
                        }
                        catch
                        {
                            Console.WriteLine($"WARN: cannot parse tag {t}.");
                        }
                    }
                    return versions;
                }
            }
            catch (Exception e)
            {
                throw new Exception("unable to get installed ml model versions.", e);
            }
        }
        
        public static async Task<List<uint>> GetAvailableVersionsFromRegistry(MLModelConfig config)
        {
            try
            {
                using (var docker = new Docker.Docker())
                {
                    var versions = new List<uint>();
                    var tags = await docker.GetTagsFromDockerRegistry(config.Image.Name);
                    foreach (var t in tags)
                    {
                        try
                        {
                            var rex = new Regex($"{config.ApiVersion}\\.[0-9]+-{config.GetTagSuffix()}");
                            if (!rex.IsMatch(t))
                            {
                                continue;
                            }
                            versions.Add(uint.Parse(t.Split('.').Last().Split('-').First()));
                        }
                        catch
                        {
                            Console.WriteLine($"WARN: cannot parse tag {t}.");
                        }
                    }
                    return versions;
                }
            }
            catch (Exception e)
            {
                throw new Exception("unable to get available ml model versions from registry.", e);
            }
        }

        public async void Dispose()
        {
            try
            {
                await _docker.StopAll(_config.Image.Name);
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR: Unable to stop docker containers while disposing ml model.\n\tDetails: {e}");
            }
            _docker.Dispose();
        }
        
        private async Task<byte[]> PhotoVmToByteArray(PhotoViewModel photo)
        {
            return await File.ReadAllBytesAsync(photo.Path);
        }
    }
}