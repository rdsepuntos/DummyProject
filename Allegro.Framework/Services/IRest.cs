namespace Allegro.Framework.Service
{
    public interface IRest
    {
        TResponse PostRequest<TResponse>(string url, object parameter) where TResponse : class;
    }
}
