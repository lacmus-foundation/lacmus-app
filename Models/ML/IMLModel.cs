using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using RescuerLaApp.Models.Docker;
using RescuerLaApp.Models.Photo;

namespace RescuerLaApp.Models.ML
{
    public interface IMLModel : IDisposable
    {
        Task Init();
        Task<List<Object>> Predict(IPhoto photo);
    }
}