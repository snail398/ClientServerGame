namespace Common
{
    public abstract class Controller<T>
    {
        protected T Data;

        protected Controller(T data)
        {
            Data = data;
        }
    }
}