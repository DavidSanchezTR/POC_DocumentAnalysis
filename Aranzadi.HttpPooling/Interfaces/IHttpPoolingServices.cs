// See https://aka.ms/new-console-template for more information

using System;
using System.Threading.Tasks;
using Aranzadi.HttpPooling.Models;

namespace Aranzadi.HttpPooling.Interfaces
{

    public interface IHttpPoolingServices
    {
        Task Start();
        Task Stop();
        Task AddRequest(HttpPoolingRequest url);
    }

}