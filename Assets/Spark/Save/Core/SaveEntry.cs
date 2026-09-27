using Newtonsoft.Json.Linq;

namespace Spark.Save.Core
{
    internal interface ISaveEntry
    {
        JToken CaptureToken();
        void RestoreToken(JToken token);
    }

    internal class SaveEntry<TState> : ISaveEntry
    {
        private readonly ISaveParticipant<TState> _participant;

        public SaveEntry(ISaveParticipant<TState> participant) => _participant = participant;

        public JToken CaptureToken() => JToken.FromObject(_participant.CaptureState());

        public void RestoreToken(JToken token) => _participant.RestoreState(token.ToObject<TState>());
    }
}