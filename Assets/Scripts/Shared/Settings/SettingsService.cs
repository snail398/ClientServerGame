using System;
using System.Collections.Generic;
using Settings;
using UnityEngine;
using Zenject;

namespace Common
{
    public class SettingsService : IInitializable
    {
        private readonly ResourceLoaderService _ResourceLoaderService;
        
        private readonly Dictionary<Type, ISettings> _SettingsMap = new Dictionary<Type, ISettings>();

        public SettingsService(ResourceLoaderService resourceLoaderService)
        {
            _ResourceLoaderService = resourceLoaderService;
            var settings = _ResourceLoaderService.LoadAllResources<ScriptableObject>("Settings");
            foreach (var setting in settings)
            {
                if (setting is ISettings set)
                {
                    set.Init();
                    _SettingsMap.Add(set.GetType(), set);
                }
            }
        }

        public void Initialize()
        {
            
        }

        public T GetSettings<T>()
        {
            if (_SettingsMap.TryGetValue(typeof(T), out var settings))
                return (T)settings;
            return default;
        }
    }
}