using R3;

namespace Redux
{
	public class State<TState>
	{
		private readonly BehaviorSubject<TState> stateSubject;
		public State(TState initialState)
		{
			stateSubject = new BehaviorSubject<TState>(initialState);
		}

		public Observable<TState> Observable {
			get => stateSubject;
		}

		public TState Value {
			get => stateSubject.Value;
			set => stateSubject.OnNext(value);
		}

		public void Dispose()
		{
			stateSubject.Dispose();
		}
	}
}
