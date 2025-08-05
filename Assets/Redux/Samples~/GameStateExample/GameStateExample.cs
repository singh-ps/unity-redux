using R3;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Redux.Examples
{
	public class GameState
	{
		public int Score { get; set; }
		public int Health { get; set; }
		public GameState(int score = 0, int health = 3)
		{
			Score = score;
			Health = health;
		}

		public GameState(GameState other)
		{
			if (other == null)
			{
				throw new ArgumentNullException(nameof(other), "Cannot copy from a null GameState.");
			}
			Score = other.Score;
			Health = other.Health;
		}
	}

	public readonly struct UpdateScoreAction
	{
		public int Score { get; }
		public UpdateScoreAction(int score)
		{
			Score = score;
		}
	}

	public readonly struct IncreaseHealthAction
	{
		public int Amount { get; }
		public IncreaseHealthAction(int amount)
		{
			Amount = amount;
		}
	}

	public readonly struct DeathAction
	{
	}

	public static class GameReducers
	{
		public static GameState UpdateScoreReducer(GameState state, UpdateScoreAction action)
		{
			GameState newState = new(state)
			{
				Score = action.Score
			};
			return newState;
		}

		public static GameState IncreaseHealthReducer(GameState state, IncreaseHealthAction action)
		{
			GameState newState = new(state)
			{
				Health = state.Health + action.Amount
			};
			return newState;
		}

		public static GameState DeathReducer(GameState state, DeathAction _)
		{
			GameState newState = new(state)
			{
				Health = 0
			};
			return newState;
		}
	}

	public class GameStateStore : Store<GameState>
	{
		public GameStateStore() : base(new GameState())
		{
		}

		protected override IReadOnlyDictionary<Type, Delegate> BuildReducers()
		{
			return new Dictionary<Type, Delegate>
			{
				{typeof(UpdateScoreAction), new Func<GameState, UpdateScoreAction, GameState>(GameReducers.UpdateScoreReducer)},
				{typeof(IncreaseHealthAction), new Func<GameState, IncreaseHealthAction, GameState>(GameReducers.IncreaseHealthReducer)},
				{typeof(DeathAction), new Func <GameState, DeathAction, GameState > (GameReducers.DeathReducer)},
			};
		}
	}

	public class GameStateExample : MonoBehaviour
	{
		private readonly GameStateStore store = new();

		private void Start()
		{
			store.StateObservable
			.Subscribe(state =>
			{
				Debug.Log($"Score: {state.Score}, Health: {state.Health}");
			});

			store.StateObservable
			.Select(state => state.Health)
			.DistinctUntilChanged()
			.Where(health => health <= 0)
			.Subscribe(_ =>
			{
				Debug.Log("Game Over!");
			});

			store.StateObservable
			.Select(state => state.Score)
			.DistinctUntilChanged()
			.Subscribe(score =>
			{
				Debug.Log($"Score updated: {score}");
			});
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				store.Dispatch(new UpdateScoreAction(10));
			}
			if (Input.GetKeyDown(KeyCode.H))
			{
				store.Dispatch(new IncreaseHealthAction(1));
			}
			if (Input.GetKeyDown(KeyCode.D))
			{
				store.Dispatch(new DeathAction());
			}
		}

		private void OnDestroy()
		{
			store.Dispose();
		}
	}
}
