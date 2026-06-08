using OsuPatcher.Runtime.Play;
using System.Threading;

namespace OsuPatcher.Runtime.Patches
{
    internal class PerformanceCalculationPatch
    {
        private static Performance _performance;
        private static int _ppQueued = 0;

        public static void SetPerformance(Performance performance)
        {
            _performance = performance;
        }

        public static void QueueCalculation(object score, float accuracy, int legacyScore, int maxCombo, int playMode)
        {
            if (_performance == null)
                return;

            if (Interlocked.Exchange(ref _ppQueued, 1) == 0)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        _performance.UpdatePerformance(score, accuracy, legacyScore, maxCombo, playMode);
                    }
                    finally
                    {
                        Interlocked.Exchange(ref _ppQueued, 0);
                    }
                });
            }
        }
    }
}
