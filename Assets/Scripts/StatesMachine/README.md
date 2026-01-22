# State Machine System

A comprehensive, reusable state machine system for Unity projects with dedicated `StateMachine` namespace.

## Overview

This state machine system provides a flexible and powerful framework for managing object states in Unity. It includes basic state management, advanced features like automatic transitions with conditions, and Unity integration through MonoBehaviour wrappers.

## Core Components

### 1. `IState`
The basic interface that all states must implement:
- `OnEnter()` - Called when entering the state
- `OnUpdate()` - Called every frame while in the state
- `OnExit()` - Called when exiting the state

### 2. `BaseState`
Abstract base class that provides common functionality:
- Reference to the owning state machine
- State naming system
- Helper methods for transitions
- Inherits from `IState`

### 3. `StateMachine`
Core state machine class that manages states and transitions:
- State registration by type and name
- Manual state transitions
- State querying and validation
- Event system for state changes

### 4. `StateMachineBehaviour`
Unity MonoBehaviour wrapper for easy integration:
- Automatic Update() calls
- Inspector-friendly interface
- Debug mode with console logging
- Unity lifecycle integration

### 5. `AdvancedStateMachine`
Extended state machine with automatic transitions:
- Condition-based transitions
- Global transitions (from any state)
- Automatic transition checking
- Transition rule management

### 6. `StateMachineUtils`
Utility class with common patterns and helpers:
- Condition builders (float, bool, input)
- Logic operators (AND, OR, NOT)
- Timer conditions
- Comparison utilities

## Quick Start

### Basic Usage

```csharp
using StateMachine;

// Create states
public class IdleState : BaseState
{
    public IdleState(StateMachine stateMachine) : base(stateMachine, "Idle") { }
    
    public override void OnEnter() => Debug.Log("Entered Idle");
    public override void OnUpdate() { /* Idle logic */ }
    public override void OnExit() => Debug.Log("Exited Idle");
}

// Setup state machine
var sm = new StateMachine();
sm.RegisterState(new IdleState(sm));
sm.RegisterState(new MovingState(sm));
sm.Start<IdleState>();

// In Update loop
sm.Update();

// Transition
sm.TransitionTo<MovingState>();
```

### Unity Integration

```csharp
using UnityEngine;
using StateMachine;

public class MyCharacter : StateMachineBehaviour
{
    private void Start()
    {
        // Register states
        RegisterState(new IdleState(StateMachine, this));
        RegisterState(new MovingState(StateMachine, this));
        
        // Start with idle
        StartStateMachine<IdleState>();
    }
}
```

### Advanced Usage with Conditions

```csharp
using StateMachine;

var advancedSM = new AdvancedStateMachine();

// Register states
advancedSM.RegisterState(new IdleState(advancedSM));
advancedSM.RegisterState(new MovingState(advancedSM));

// Add transition rules
advancedSM.AddTransition<IdleState, MovingState>(() => Input.GetKey(KeyCode.W));
advancedSM.AddTransition<MovingState, IdleState>(() => !Input.GetKey(KeyCode.W));

// Global transition (from any state)
advancedSM.AddGlobalTransition<DeathState>(() => health <= 0);

// Start and update
advancedSM.Start<IdleState>();
// In Update: advancedSM.Update(); // Will automatically check transitions
```

## Utility Functions

### Condition Builders

```csharp
using StateMachine;

// Float comparisons
var healthLow = StateMachineUtils.FloatCondition(() => health, 30f, ComparisonType.LessThan);

// Input conditions
var jumpPressed = StateMachineUtils.KeyPressCondition(KeyCode.Space);

// Timer conditions
var timer = StateMachineUtils.CreateTimer(3f);

// Combined conditions
var complexCondition = StateMachineUtils.And(
    healthLow,
    StateMachineUtils.Not(() => isInvincible)
);
```

### Timer Usage

```csharp
// Create timer
var attackTimer = StateMachineUtils.CreateTimer(2f);

// Use in transitions
stateMachine.AddTransition<AttackState, IdleState>(attackTimer);

// Manual checking
if (attackTimer.IsComplete())
{
    // Timer finished
}

// Get progress (0 to 1)
float progress = attackTimer.Progress();
```

## Features

### State Registration
- Register by type and name
- Batch registration
- Automatic name generation
- Duplicate handling

### Transitions
- Manual transitions by type, name, or instance
- Condition-based automatic transitions
- Global transitions from any state
- Transition validation
- Event notifications

### Debugging
- Console logging in debug mode
- Inspector state display
- Scene view state labels
- Transition logging

### Utilities
- Common condition patterns
- Logic operators
- Timer system
- Input helpers

## Best Practices

1. **Inherit from BaseState**: Use `BaseState` instead of implementing `IState` directly
2. **Use StateMachineBehaviour**: For Unity objects, use the MonoBehaviour wrapper
3. **Namespace Usage**: Always use `using StateMachine;` to avoid conflicts
4. **State Naming**: Provide meaningful names for debugging
5. **Condition Functions**: Keep condition functions simple and efficient
6. **Resource Cleanup**: Handle cleanup in `OnExit()` methods

## File Structure

```
StatesMachine/
├── IState.cs                  # Base state interface
├── BaseState.cs               # Abstract state base class
├── StateMachine.cs            # Core state machine
├── StateMachineBehaviour.cs   # Unity MonoBehaviour wrapper
├── AdvancedStateMachine.cs    # Extended state machine with conditions
├── StateMachineUtils.cs       # Utility functions and helpers
└── ExampleUsage.cs            # Complete example implementation
```

## Example: AI Character

See `ExampleUsage.cs` for a complete implementation showing:
- AI with Idle, Patrol, and Chase states
- Condition-based transitions
- Unity integration
- Debug visualization
- State-specific behaviors

This system is designed to be:
- **Reusable**: Works across different Unity projects
- **Flexible**: Supports simple to complex state logic
- **Extensible**: Easy to add new features and states
- **Unity-Friendly**: Integrates seamlessly with Unity workflows
- **Debuggable**: Built-in debugging and visualization tools