using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RescuerLaApp.Models.Docker;
using RescuerLaApp.Models.Photo;
using RescuerLaApp.ViewModels;

namespace RescuerLaApp.Models.ML
{
    public interface IMLModel : IDisposable
    {
        Task Init();
        Task<List<Object>> Predict(PhotoViewModel photoViewModel);
    }
}