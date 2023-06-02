namespace Joycollab.v2
{
    public interface IModuleController 
    {
        void RegisterObserver(IModuleObserver observer);
        void UnregisterObserver(IModuleObserver observer);
        void Notify<T>(IModuleObserver observer, T data);
        void NotifyAll<T>(T data);
    }

    public interface IModuleObserver
    {
        void UpdateModuleList<T>(T menu);
    }
}