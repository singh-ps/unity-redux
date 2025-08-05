using R3;

public interface IState<TState>
{
	BehaviorSubject<TState> StateSubject { get; }
	TState Value { get; set; }
}

public class State<TState> : IState<TState>
{
	private BehaviorSubject<TState> stateSubject;
	public State(TState initialState)
	{
		stateSubject = new BehaviorSubject<TState>(initialState);
	}

	public BehaviorSubject<TState> StateSubject => stateSubject;

	public TState Value {
		get => stateSubject.Value;
		set => stateSubject.OnNext(value);
	}
}