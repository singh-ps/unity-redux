# Unity Redux

A lightweight Redux implementation for Unity projects, providing predictable state management for your game applications.

## Overview

This Unity Redux library brings the power of Redux state management to Unity, allowing you to manage your game state in a predictable and maintainable way. Built with C# and integrated with R3 (Reactive Extensions) for reactive programming capabilities.

## Features

- **Predictable State Management**: Centralized state management following Redux principles
- **Type-Safe Actions**: Strongly typed actions using C# structs
- **Reactive Programming**: Built-in observables using R3 for state changes
- **Unity Integration**: Seamless integration with Unity's MonoBehaviour lifecycle
- **Immutable State Updates**: Ensures state consistency through reducer functions

## Installation

1. [**Install R3** using NuGetForUnity](https://github.com/Cysharp/R3?tab=readme-ov-file#unity)
2. Open Package Manager window (Window | Package Manager)
3. Click `+` button on the upper-left of a window, and select "Add package from git URL..."
4. Enter the following URL and click `Add` button

```
https://github.com/singh-ps/unity-redux.git?path=/Assets/Redux
```

> **_NOTE:_** To install a concrete version you can specify the version by prepending #v{version} e.g. `#v1.0.0`. For more see [Unity UPM Documentation](https://docs.unity3d.com/Manual/upm-git.html).

## Quick Start

### 1. Define Your State

```csharp
public class GameState
{
    public int Score { get; set; }
    public int Health { get; set; }

    public GameState()
    {
        Score = 0;
        Health = 100;
    }

    public GameState(GameState other)
    {
        Score = other.Score;
        Health = other.Health;
    }
}
```

### 2. Create Actions

```csharp
public readonly struct UpdateScoreAction
{
    public readonly int Score;

    public UpdateScoreAction(int score)
    {
        Score = score;
    }
}

public readonly struct IncreaseHealthAction
{
    public readonly int Amount;

    public IncreaseHealthAction(int amount)
    {
        Amount = amount;
    }
}
```

### 3. Implement Reducers

```csharp
public static class GameReducers
{
    public static GameState UpdateScoreReducer(GameState state, UpdateScoreAction action)
    {
        return new GameState(state)
        {
            Score = action.Score
        };
    }

    public static GameState IncreaseHealthReducer(GameState state, IncreaseHealthAction action)
    {
        return new GameState(state)
        {
            Health = state.Health + action.Amount
        };
    }
}
```

### 4. Create Your Store

```csharp
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
            {typeof(IncreaseHealthAction), new Func<GameState, IncreaseHealthAction, GameState>(GameReducers.IncreaseHealthReducer)}
        };
    }
}
```

### 5. Use in Unity MonoBehaviour

```csharp
public class GameManager : MonoBehaviour
{
    private readonly GameStateStore store = new();

    private void Start()
    {
        // Subscribe to state changes
        store.StateObservable.Subscribe(state =>
        {
            Debug.Log($"Score: {state.Score}, Health: {state.Health}");
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Dispatch actions
            store.Dispatch(new UpdateScoreAction(100));
            store.Dispatch(new IncreaseHealthAction(10));
        }
    }

    private void OnDestroy()
    {
        store.Dispose();
    }
}
```

## Core Components

### Store<TState>

The central store that holds your application state and manages state updates through reducers.

- `CurrentState`: Get the current state value
- `StateObservable`: Observable stream of state changes
- `Dispatch<TAction>()`: Dispatch actions to update state
- `Dispose()`: Clean up resources

### State<TState>

Internal state container that wraps your state with R3's BehaviorSubject for reactive capabilities.

### Actions

Define actions as readonly structs to ensure immutability and type safety.

### Reducers

Pure functions that take the current state and an action, returning a new state.

## Dependencies

- **Unity 2021.3+** (recommended)
- **NuGetForUnity**: For package management
- **R3**: For reactive programming support

## Example

Check out the complete example in [`Assets/Redux/Scripts/Examples/GameStateExample.cs`](Assets/Redux/Scripts/Examples/GameStateExample.cs) for a full implementation demonstrating:

- State definition
- Action creation
- Reducer implementation
- Store setup
- Unity integration

## Best Practices

1. **Keep reducers pure**: No side effects, always return new state objects
2. **Use readonly structs for actions**: Ensures immutability
3. **Subscribe to state changes in Start()**: Avoid memory leaks
4. **Always dispose stores**: Call `Dispose()` in `OnDestroy()`
5. **Keep state immutable**: Create new state objects instead of modifying existing ones

## License

This project is licensed under the terms specified in the [LICENSE](LICENSE) file.

## Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.
