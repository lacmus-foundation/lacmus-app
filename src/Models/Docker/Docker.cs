using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Newtonsoft.Json;
using Serilog;

namespace LacmusApp.Models.Docker
{
    public class Docker : IDisposable
    {
        private readonly DockerClient _client;

        public Docker()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _client = new DockerClientConfiguration(
                        new Uri("npipe://./pipe/docker_engine"))
                    .CreateClient();
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || 
                    RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                _client = new DockerClientConfiguration(
                        new Uri("unix:///var/run/docker.sock"))
                    .CreateClient();
            }
            else
            {
                throw new Exception($"Your system dose not supported: {RuntimeInformation.OSDescription}");
            }
        }
        
        
        
        public async Task Initialize(IDockerImage image, IDockerAccaunt accaunt)
        {
            try
            {
                Log.Information($"Initializing image {image.Name}:{image.Tag}.");
                var progressDictionary = new Dictionary<string, string>();
                var images = await _client.Images.ListImagesAsync(new ImagesListParameters {MatchName = $"{image.Name}:{image.Tag}"});
                if (images.Count > 0)
                {
                    Log.Information($"Such image already exists: {images.First().ID}.");
                    return;
                }
                
                
                var progress = new Progress<JSONMessage>();
                var count = 0;
                var pb = new ProgressBar();
                pb.Report(0.0, $"Downloading {count} of {progressDictionary.Count}" );
                progress.ProgressChanged += (sender, message) =>
                {
                    try
                    {
                        if (progressDictionary.ContainsKey(message.ID))
                        {
                            if (progressDictionary[message.ID] != "Pull complete" && message.Status == "Pull complete")
                            {
                                count++;
                                progressDictionary[message.ID] = message.Status;
                            }
                            pb.Report((double)count / progressDictionary.Count, $"Downloading {count} of {progressDictionary.Count}" );
                        }
                        else if(message.Status.Contains("Pulling fs layer") || message.Status.Contains("Waiting"))
                        {
                            progressDictionary.Add(message.ID, message.Status);
                        }
                    }
                    catch
                    {
                        //ignored
                    }
                };
                
                await _client.Images.CreateImageAsync(
                    new ImagesCreateParameters
                    {
                        FromImage = $"{image.Name}:{image.Tag}"
                    },
                    new AuthConfig
                    {
                        Email = accaunt.Email,
                        Username = accaunt.Username,
                        Password = accaunt.Password
                    },
                    progress
                );
                pb.Dispose();
                Log.Information($"Successfully initialized image {image.Name}:{image.Tag}.");
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to initialize image {image.Name}:{image.Tag}.", e);
            }
        }

        public async Task<string> CreateContainer(IDockerImage image)
        {
            try
            {
                Log.Information($"Creating container {image.Name}:{image.Tag}");
                var containers = await _client.Containers.ListContainersAsync(new ContainersListParameters {All = true});
                foreach (var container in containers)
                {
                    if (container.Image == $"{image.Name}:{image.Tag}")
                    {
                        Log.Information($"Such container already exists: {container.ID}.");
                        return container.ID;
                    }
                }
                
                var images = await _client.Images.ListImagesAsync(new ImagesListParameters {MatchName = $"{image.Name}:{image.Tag}"});
                if (images == null || images.Count == 0)
                {
                    throw new Exception($"No such image {image.Name}:{image.Tag}.");
                }

                if (image.Tag.Contains("gpu"))
                {
                    var stdOut = "";
                    var bash = new BashCommand();
                    stdOut = bash.Execute($"docker create --gpus all -p 5000:5000 {image.Name}:{image.Tag}", out var err);
                    
                    await Task.Delay(800);
                    stdOut = stdOut.Replace(Environment.NewLine, String.Empty);
                    if(string.IsNullOrWhiteSpace(stdOut) || !string.IsNullOrWhiteSpace(err))
                        throw new Exception($"invalid id {err}");
                    
                    Log.Information($"Successfully created container {image.Name}:{image.Tag}.");
                    return stdOut;
                }

                //else
                var containerCreateResponse = await _client.Containers.CreateContainerAsync(
                    new CreateContainerParameters
                    {
                        Image = $"{image.Name}:{image.Tag}",
                        HostConfig = new HostConfig
                        {
                            PortBindings = new Dictionary<string, IList<PortBinding>>
                            {
                                { "5000", new List<PortBinding> { new PortBinding { HostPort = "5000" } } }
                            }
                        },
                        ExposedPorts = new Dictionary<string, EmptyStruct>
                        {
                            { "5000", new EmptyStruct() }
                        }
                    });
                if (string.IsNullOrWhiteSpace(containerCreateResponse.ID))
                {
                    throw new Exception("invalid id");
                }

                Log.Information($"Successfully created container {image.Name}:{image.Tag}.");
                return containerCreateResponse.ID;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to create docker container: {e.Message}");
            }
        }

        public async Task<bool> Run(string id)
        {
            try
            {
                Log.Information($"Running container: {id}.");
                var containers = await _client.Containers.ListContainersAsync(new ContainersListParameters {All = true});
                foreach (var container in containers)
                {
                    if (container.ID == id && container.Status.Contains("Up"))
                    {
                        Log.Information($"Such container already running: {container.ID}.");
                        return true;
                    }
                }
                
                var isSuccess = await _client.Containers.StartContainerAsync(id, new ContainerStartParameters());
                if(!isSuccess)
                    throw new Exception("Running was not successful.");
                
                Log.Information($"Successfully run container {id}.");
                return true;
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to start docker container: {e.Message}");
            }
        }

        public async Task StopAll(string imageName)
        {
            try
            {
                Log.Information($"Stopping all containers {imageName}");
                var containers =
                        await _client.Containers.ListContainersAsync(new ContainersListParameters {All = true});
                foreach (var container in containers)
                {
                    if (container.Image.Contains(imageName) && container.Status.Contains("Up"))
                    {
                        var success =
                                await _client.Containers.StopContainerAsync(container.ID,
                                        new ContainerStopParameters());
                        Log.Information($"Successfully stop container {container.ID} {success.ToString()}.");
                    }
                }
                Log.Information($"All containers {imageName} was stop.");
            }
            catch(Exception e)
            {
                throw new Exception($"Unable to stop docker container: {e.Message}");
            }
        }
        
        public async Task Stop(string id)
        {
            try
            {
                Log.Information($"Stopping container {id}");
                var containers = await _client.Containers.ListContainersAsync(new ContainersListParameters {All = true});
                foreach (var container in containers)
                {
                    if (container.ID == id && container.Status.Contains("Exited"))
                    {
                        Log.Information($"Such container already stopped: {container.ID}.");
                        return;
                    }
                }
                
                var success = await _client.Containers.StopContainerAsync(id, new ContainerStopParameters());
                if(!success)
                    throw new Exception("Stopping was not successful.");
                Log.Information($"Successfully stop container {id}.");
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to stop docker container: {e.Message}");
            }
        }

        public async Task<List<string>> GetTagsFromDockerRegistry(string imageName)
        {
            var baseUrl = "https://registry.hub.docker.com";
            try
            {
                var client = new RestApiClient(baseUrl);
                var result = new List<string>();
                var response = new DockerTagResponse { Next = baseUrl + $"/v2/repositories/{imageName}/tags/"};
            
                while (!string.IsNullOrEmpty(response.Next) && response.Next.Contains(baseUrl))
                {
                    var jsonResp = await client.GetAsync(response.Next.Remove(0, baseUrl.Length));
                    response = JsonConvert.DeserializeObject<DockerTagResponse>(jsonResp);
                    result.AddRange(response.Images.Select(image => image.Tag).ToList());
                }
                return result;
            }
            catch(Exception e)
            {
                throw new Exception($"Unable to retrieve tag(s): {e.Message}", e);
            }
        }

        public async Task Remove(IDockerImage image)
        {
            try
            {
                //stop and remove all containers
                var containers = await _client.Containers.ListContainersAsync(new ContainersListParameters {All = true});
                foreach (var container in containers)
                {
                    if (container.Image == $"{image.Name}:{image.Tag}")
                    {
                        await Stop(container.ID);
                        await _client.Containers.RemoveContainerAsync(container.ID, new ContainerRemoveParameters {Force = true});
                    }
                }
                
                var images = await _client.Images.ListImagesAsync(new ImagesListParameters {MatchName = $"{image.Name}:{image.Tag}"});
                foreach (var img in images)
                {
                    await _client.Images.DeleteImageAsync(img.ID, new ImageDeleteParameters {Force = true});
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to create docker image: {e.Message}");
            }
        }

        public async Task<List<string>> GetInstalledTags(string imageName)
        {
            try
            {
                var tags = new List<string>();
                var images = await _client.Images.ListImagesAsync(new ImagesListParameters {MatchName = imageName});
                foreach (var image in images)
                {
                    tags.AddRange(image.RepoTags);
                }

                return (from tag in tags where tag.Contains($"{imageName}:") select tag.Replace($"{imageName}:", "")).ToList();
            }
            catch(Exception e)
            {
                throw new Exception($"Unable to retrieve installed versions: {e.Message}");
            }
        }
        
        public async Task<List<string>> GetInstalledImages()
        {
            try
            {
                var imgs = new List<string>();
                var images = await _client.Images.ListImagesAsync(new ImagesListParameters());
                foreach (var image in images)
                {
                    imgs.AddRange(image.RepoTags);
                }

                return imgs;
            }
            catch(Exception e)
            {
                throw new Exception($"Unable to retrieve installed versions: {e.Message}");
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
        
        [JsonObject]
        private class DockerTagResponse
        {
            [JsonProperty("count")]
            public int Count { get; set; }
            [JsonProperty("next")]
            public string Next { get; set; }
            [JsonProperty("results")]
            public DockerImageResponse[] Images { get; set; }
        }
        
        [JsonObject]
        private class DockerImageResponse
        {
            [JsonProperty("name")]
            public string Tag { get; set; }
        }
    }
}