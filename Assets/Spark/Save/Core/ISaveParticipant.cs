namespace Spark.Save.Core
{
    public interface ISaveParticipant<TState>
    {
        TState CaptureState();
        void RestoreState(TState state);
    }
}