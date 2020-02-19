using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RescuerLaApp.Models.Photo;

namespace RescuerLaApp.Models.ML
{
    public class MLModel : IMLModel
    {
        private Docker.Docker _docker;
        private RestApiClient _client;
        private string _id;

        public MLModel()
        {
            _docker = new Docker.Docker();
            _id = "";
        }
        
        public async Task Init(IMLModelConfig config)
        {
            try
            {
                _client = new RestApiClient("http://127.0.0.1:5000/");
            
                Console.WriteLine("INFO: Checking retina-net service...");
                var status = await _client.GetStatusAsync();
                if (status != null && status.Contains("server is running",StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("INFO: Retina-net is ready.");
                    return;
                }
            
                Console.WriteLine("INFO: Creating container...");
                _id = await _docker.CreateContainer(config.Image);
            
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

        public IPrediction Predict(IPhoto photo)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<(int number, IPrediction prediction)> Predict(IEnumerable<IPhoto> photos)
        {
            throw new System.NotImplementedException();
        }
        
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
    }
}