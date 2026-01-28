using UnityEngine;

namespace MatrixUtils.DependencyInjection
{
    public static class InjectorExtensions
    {
        public static void RequestInjection(this MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour == null) return;
        
            Injector injector = Object.FindAnyObjectByType<Injector>();
            if (injector == null)
            {
                Debug.LogError("[Injector] No Injector found in scene.");
                return;
            }
            injector.Inject(monoBehaviour);
        }
    }
}