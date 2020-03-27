using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using LacmusApp.Models.Docker;
using LacmusApp.Models.Photo;
using LacmusApp.ViewModels;

namespace LacmusApp.Models.ML
{
    public interface IMLModel : IDisposable
    {
        Task Init();
        Task<List<Object>> Predict(PhotoViewModel photoViewModel);
    }
}