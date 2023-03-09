using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PitchSolution
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
        void UpdateFloatingMenu(FloatingMenu menu);
    }
}