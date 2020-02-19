using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RescuerLaApp.Models.Photo;

namespace RescuerLaApp.Models.ML
{
    public class MLModel : IMLModel
    {
        private readonly IMLModelConfig _config;
        private Docker.Docker _docker;
        private RestApiClient _client;
        private string _id;

        public MLModel(IMLModelConfig config)
        {
            _docker = new Docker.Docker();
            _id = "";
            _config = config;
        }
        
        public async Task Init()
        {
            try
            {
                _client = new RestApiClient(_config.Url);
            
                Console.WriteLine("INFO: Checking retina-net service...");
                var status = await _client.GetStatusAsync();
                if (status != null && status.Contains("server is running",StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("INFO: Retina-net is ready.");
                    return;
                }
            
                Console.WriteLine("INFO: Creating container...");
                _id = await _docker.CreateContainer(_config.Image);
            
                Console.WriteLine("INFO: Running container...");
                if (await _docker.Run(_id))
                {
                    Console.WriteLine("INFO: Container runs. Loading retina-net model...");
                    var startTime = DateTime.Now;
                    //wait no more then 10 min.
                    while ((DateTime.Now - startTime).TotalMinutes < 10)
                    {
                        // Provide a 100ms startup delay
                        await Task.Delay(TimeSpan.FromMilliseconds(100));
                        status = await _client.GetStatusAsync();
                        if (status != null && status.Contains("server is running",StringComparison.InvariantCultureIgnoreCase))
                        {
                            Console.WriteLine("INFO: Retina-net is ready.");
                            return;
                        }   
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Unable to init retina-net model", e);
            }
        }

        public async Task<List<Object>> Predict(IPhoto photo)
        {
            try
            {
                var objects = new List<Object>();
                var status = await _client.GetStatusAsync();
                if (status == null || !status.Contains("server is running"))
                    throw new Exception("server is not running");
            
                var request = new MLRequest
                {
                    Data = PhotoToByteArray(photo)
                };
            
                var jsonRequest = JsonConvert.SerializeObject(request);
                var response = JsonConvert.DeserializeObject<MLResponse>(
                    await _client.PostAsync(jsonRequest, "image"));
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
                throw new Exception("Con not predict photo", e);
            }
        }
        
        public void Dispose()
        {
            _docker.Dispose();
        }
        
        byte[] PhotoToByteArray(IPhoto photo)
        {
            if (photo == null || photo.ImageBrush == null)
                throw new NullReferenceException("Photo is null");

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, photo.ImageBrush.Source);
                return ms.ToArray();
            }
        }
    }
}