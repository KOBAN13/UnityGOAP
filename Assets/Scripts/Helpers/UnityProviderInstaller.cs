using System;
using R3;
using UnityEngine;

namespace Helpers
{
    public static class UnityProviderInstaller
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        public static void SetDefaultObserverSystem()
        {
            SetDefaultObservableSystem(static ex => UnityEngine.Debug.LogError(ex));
        }

        private static void SetDefaultObservableSystem(Action<Exception> unhandledExceptionHandler)
        {
            ObservableSystem.RegisterUnhandledExceptionHandler(unhandledExceptionHandler);
            ObservableSystem.DefaultTimeProvider = UnityTimeProvider.Update;
            ObservableSystem.DefaultFrameProvider = UnityFrameProvider.Update;
        }
    }
}