using UnityEngine;

public class ResourceLoaderService
{
    public T LoadResource<T>(string path) where T : Object
    {
        return (T) Resources.Load(path, typeof (T));
    }

    public T[] LoadAllResources<T>(string path) where T : Object
    {
        return Resources.LoadAll<T>(path);
    }
}
