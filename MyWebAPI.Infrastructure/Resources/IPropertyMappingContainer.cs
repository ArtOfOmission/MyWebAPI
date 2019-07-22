using MyWebAPI.Core.Interfaces;
using MyWebAPI.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyWebAPI.Infrastructure.Resources
{
    public interface IPropertyMappingContainer
    {
        void Register<T>() where T : IPropertyMapping, new();
        IPropertyMapping Resolve<TSource, TDestination>() where TDestination : IEntity;
        bool ValidateMappingExistsFor<TSource, TDestination>(string fields) where TDestination : IEntity;
    }
}
