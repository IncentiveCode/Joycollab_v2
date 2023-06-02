/// <summary>
/// 시스템 상 저장 공간 (Repository) 
/// @author         : HJ Lee
/// @last update    : 2023. 06. 02
/// @version        : 0.1
/// @update
///     v0.1 (2023. 06. 02) : 파일 최초 생성, R class 에서 분리. 
/// </summary>


namespace Joycollab.v2 
{
	public interface iRepositoryController 
    {
        void RegisterObserver(iRepositoryObserver observer, eStorageKey key);
        void UnregisterObserver(iRepositoryObserver observer, eStorageKey key);
        void RequestInfo(iRepositoryObserver observer, eStorageKey key);
        void Notify(iRepositoryObserver observer, eStorageKey key);
        void NotifyAll(eStorageKey key);
    }

    public interface iRepositoryObserver 
    {
        void UpdateInfo(eStorageKey key);
    }
}