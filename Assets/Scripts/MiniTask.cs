using System;
using System.Threading.Tasks;
using UnityEngine;

namespace BOYAREngine.Utils
{
    public static class MiniTask
    {
        public static async void Run(float delaySec, Action action)
        {
            var tm = Time.time + delaySec;
            while (tm > Time.time)
            {
                await Task.Yield();
            }
            try
            {
                action();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }
    }
}

