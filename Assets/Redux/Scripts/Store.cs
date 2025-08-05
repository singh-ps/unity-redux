using R3;
using System;
using System.Collections.Generic;

namespace Redux
{
	public interface IStore<TState> : IDisposable
	{
		void Dispatch<TAction>(TAction action) where TAction : struct;
		Observable<TState> StateObservable { get; }
		TState CurrentState { get; }
	}

	public abstract class Store<TState> : IStore<TState>
	{
		private readonly State<TState> state;
		private readonly Dictionary<Type, Delegate> reducers;
		private bool isDispatching;

		public Observable<TState> StateObservable => state.Observable;
		public TState CurrentState => state.Value;

		protected Store(TState initialState)
		{
			if (initialState == null)
			{
				throw new ArgumentNullException(nameof(initialState));
			}
			state = new State<TState>(initialState);

			IReadOnlyDictionary<Type, Delegate> map = BuildReducers();
			if (map == null || map.Count == 0)
			{
				throw new InvalidOperationException("BuildReducers() must return at least one reducer.");
			}
			reducers = new Dictionary<Type, Delegate>(map);
			isDispatching = false;
		}

		protected abstract IReadOnlyDictionary<Type, Delegate> BuildReducers();

		public void Dispatch<TAction>(TAction action) where TAction : struct
		{
			if (isDispatching)
			{
				throw new InvalidOperationException("Cannot dispatch while already dispatching.");
			}

			reducers.TryGetValue(typeof(TAction), out Delegate rDelegate);

			if (rDelegate == null)
			{
				throw new InvalidOperationException($"No reducer found for action type {typeof(TAction).Name}.");
			}

			isDispatching = true;
			try
			{
				Func<TState, TAction, TState> reducer = (Func<TState, TAction, TState>) rDelegate;
				TState newState = reducer(CurrentState, action) ?? throw new InvalidOperationException($"Reducer for {typeof(TAction).Name} returned null state.");
				state.Value = newState;
			}
			catch (InvalidCastException)
			{
				throw new InvalidOperationException($"Reducer for {typeof(TAction).Name} has incorrect signature.");
			}
			catch (Exception ex) when (ex is not InvalidOperationException)
			{
				throw new InvalidOperationException($"Reducer for {typeof(TAction).Name} threw an exception.", ex);
			}
			finally
			{
				isDispatching = false;
			}
		}

		public void Dispose()
		{
			state.Dispose();
		}
	}
}
