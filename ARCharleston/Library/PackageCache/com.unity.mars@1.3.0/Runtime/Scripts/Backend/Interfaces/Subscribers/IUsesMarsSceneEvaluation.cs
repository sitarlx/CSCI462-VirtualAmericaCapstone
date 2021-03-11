using System;

namespace Unity.MARS.Query
{
    /// <summary>
    /// A class that implements IUsesMarsSceneEvaluation gains the ability to request evaluation of the query scene
    /// </summary>
    public interface IUsesMarsSceneEvaluation{ }

    delegate MarsSceneEvaluationRequestResponse RequestSceneEvaluationDelegate(Action onEvaluationComplete = null);

    static class IUsesMarsSceneEvaluationMethods
    {
        public static RequestSceneEvaluationDelegate RequestSceneEvaluation { get; internal set; }
        public static Action<MarsSceneEvaluationMode> SetEvaluationMode { get; internal set; }
        
        public static Func<float> GetEvaluationInterval { get; internal set; }
        public static Action<float> SetEvaluationInterval { get; internal set; }
    }

    public static class IUsesMarsSceneEvaluationExtensionMethods
    {
        /// <summary>
        /// Request that the results of all active queries be recalculated.
        /// </summary>
        /// <param name="onEvaluationComplete">
        /// A callback executed when the evaluation triggered by the request has completed
        /// </param>
        /// <returns>An enum describing the system response to the request</returns>
        public static MarsSceneEvaluationRequestResponse RequestSceneEvaluation(this IUsesMarsSceneEvaluation caller,
            Action onEvaluationComplete = null)
        {
            return IUsesMarsSceneEvaluationMethods.RequestSceneEvaluation(onEvaluationComplete);
        }

        /// <summary>
        /// Set the scheduling mode for evaluating the MARS scene.
        /// Changing the mode to EvaluateOnInterval will queue an evaluation.
        /// </summary>
        /// <param name="mode">The mode to set</param>
        public static void SetEvaluationMode(this IUsesMarsSceneEvaluation caller, MarsSceneEvaluationMode mode)
        {
            IUsesMarsSceneEvaluationMethods.SetEvaluationMode(mode);
        }
        
        /// <summary>
        /// Get the scene evaluation interval in seconds
        /// </summary>
        public static float GetEvaluationInterval(this IUsesMarsSceneEvaluation caller)
        {
            return IUsesMarsSceneEvaluationMethods.GetEvaluationInterval();
        }
        
        /// <summary>
        /// Set the scene evaluation interval.
        /// Will still set the interval even if not using Interval mode, but not change the mode.
        /// </summary>
        /// <param name="interval">The evaluation interval in seconds</param>
        public static void SetEvaluationInterval(this IUsesMarsSceneEvaluation caller, float interval)
        {
            IUsesMarsSceneEvaluationMethods.SetEvaluationInterval(interval);
        }
    }
}
