using R3;

namespace Spark.Modal.Core
{
    public readonly struct ModalResult<TResult>
    {
        public bool IsConfirmed { get; }
        public TResult Value { get; }

        private ModalResult(bool confirmed, TResult value)
        {
            IsConfirmed = confirmed;
            Value = value;
        }

        public static ModalResult<TResult> Confirmed(TResult value) => new(true, value);
        public static ModalResult<TResult> Cancelled() => new(false, default);
    }
    
    public readonly struct ModalResult
    {
        public static ModalResult<Unit> Confirmed() => ModalResult<Unit>.Confirmed(Unit.Default);
        public static ModalResult<Unit> Cancelled() => ModalResult<Unit>.Cancelled();
    }
}
