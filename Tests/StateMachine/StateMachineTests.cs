/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

using Moq;
using StateMachine.ExampleClasses;
using Tests.StateMachine.TestingClasses;

namespace Tests.StateMachine;

public class StateMachineTests
{
    private const string UNINITIALIZED_MESSAGE = "StateMachine was not properly initialized. CurrentState is not available.";
    private const string DUPLICATE_STATE_MESSAGE = "The State \"{0}\" is already stored, duplicate cannot be added.";

    private EmptyTestingStateMachine CreateDefaultStateMachine()
    {
        EmptyTestingStateMachine stateMachine = new();
        stateMachine.AddState<DemoStateA>();
        stateMachine.AddState<DemoStateB>();
        return stateMachine;
    }

    [Fact]
    public void CurrentStateIs_WhenCalledOnCorrectState_ReturnsTrue()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = CreateDefaultStateMachine();
        stateMachine.Initialize();

        // Act
        bool currentStateIsA = stateMachine.CurrentStateIs<DemoStateA>();

        // Assert
        Assert.True(currentStateIsA);
    }

    [Fact]
    public void CurrentStateIs_WhenCalledOnWrongState_ReturnsFalse()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = CreateDefaultStateMachine();
        stateMachine.Initialize();

        // Act
        bool currentStateIsB = stateMachine.CurrentStateIs<DemoStateB>();

        // Assert
        Assert.False(currentStateIsB);
    }

    [Fact]
    public void CurrentStateIs_WhenCalledOnUninitialized_ThrowsError()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = CreateDefaultStateMachine();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            stateMachine.CurrentStateIs<DemoStateB>()
        );
        Assert.Equal(UNINITIALIZED_MESSAGE, exception.Message);
    }

    [Fact]
    public void HasStateStored_WhenCalledForStoredState_ReturnsTrue()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = CreateDefaultStateMachine();

        // Act
        bool hasStoredA = stateMachine.HasStateStored<DemoStateA>();
        bool hasStoredB = stateMachine.HasStateStored<DemoStateB>();

        // Assert
        Assert.True(hasStoredA);
        Assert.True(hasStoredB);
    }

    [Fact]
    public void HasStateStored_WhenCalledForUnstoredState_ReturnsFalse()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = new();

        // Act
        bool hasStoredA = stateMachine.HasStateStored<DemoStateA>();
        bool hasStoredB = stateMachine.HasStateStored<DemoStateB>();

        // Assert
        Assert.False(hasStoredA);
        Assert.False(hasStoredB);
    }

    [Fact]
    public void Initialize_WhenCalled_SetsUpDefaultState()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = new();
        var mockStateB = new Mock<DemoStateB>();
        stateMachine.AddState(mockStateB.Object);
        stateMachine.AddState<DemoStateA>();

        // Act
        stateMachine.Initialize();

        // Assert
        mockStateB.Verify(state => state.OnEnter(), Times.Once());
        Assert.True(stateMachine.CurrentStateIs<DemoStateB>());
        Assert.False(stateMachine.CurrentStateIs<DemoStateA>());
    }

    [Fact]
    public void Initialize_WhenCalledWithoutStoredStates_ThrowsError()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = new();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            stateMachine.Initialize()
        );
        Assert.Equal(
            "StateMachine initialization failed: ConcreteStateMachine does not contain any states.",
            exception.Message
        );
    }

    [Fact]
    public void Update_WhenCalledOnUninitialized_ThrowsError()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = CreateDefaultStateMachine();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            stateMachine.Update()
        );
        Assert.Equal(UNINITIALIZED_MESSAGE, exception.Message);
    }

    [Fact]
    public void Update_WhenCalledAfterProperStateRequest_HandlesStateChange()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = new();
        var mockStateA = new Mock<DemoStateA>();
        var mockStateB = new Mock<DemoStateB>();
        stateMachine.AddState(mockStateA.Object);
        stateMachine.AddState(mockStateB.Object);
        stateMachine.Initialize();
        stateMachine.RequestState<DemoStateB>();

        // Act
        stateMachine.Update();

        // Assert
        mockStateA.Verify(state => state.OnEnter(), Times.Once());
        mockStateA.Verify(state => state.OnUpdate(), Times.Never());
        mockStateA.Verify(state => state.OnExit(), Times.Once());
        mockStateB.Verify(state => state.OnEnter(), Times.Once());
        mockStateB.Verify(state => state.OnUpdate(), Times.Once());
        mockStateB.Verify(state => state.OnExit(), Times.Never());
        Assert.True(stateMachine.CurrentStateIs<DemoStateB>());
        Assert.False(stateMachine.CurrentStateIs<DemoStateA>());
    }

    [Fact]
    public void Update_WhenCalledWithoutStateRequest_UpdatesNormally()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = new();
        var mockStateA = new Mock<DemoStateA>();
        var mockStateB = new Mock<DemoStateB>();
        stateMachine.AddState(mockStateA.Object);
        stateMachine.AddState(mockStateB.Object);
        stateMachine.Initialize();

        // Act
        stateMachine.Update();
        stateMachine.Update();

        // Assert
        mockStateA.Verify(state => state.OnEnter(), Times.Once());
        mockStateA.Verify(state => state.OnUpdate(), Times.Exactly(2));
        mockStateA.Verify(state => state.OnExit(), Times.Never());
        mockStateB.Verify(state => state.OnEnter(), Times.Never());
        mockStateB.Verify(state => state.OnUpdate(), Times.Never());
        mockStateB.Verify(state => state.OnExit(), Times.Never());
        Assert.True(stateMachine.CurrentStateIs<DemoStateA>());
        Assert.False(stateMachine.CurrentStateIs<DemoStateB>());
    }

    [Fact]
    public void Update_WhenCalledWithRedundentStateRequest_UpdatesNormally()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = new();
        var mockStateA = new Mock<DemoStateA>();
        var mockStateB = new Mock<DemoStateB>();
        stateMachine.AddState(mockStateA.Object);
        stateMachine.AddState(mockStateB.Object);
        stateMachine.Initialize();
        stateMachine.RequestState<DemoStateA>();

        // Act
        stateMachine.Update();
        stateMachine.Update();

        // Assert
        mockStateA.Verify(state => state.OnEnter(), Times.Once());
        mockStateA.Verify(state => state.OnUpdate(), Times.Exactly(2));
        mockStateA.Verify(state => state.OnExit(), Times.Never());
        mockStateB.Verify(state => state.OnEnter(), Times.Never());
        mockStateB.Verify(state => state.OnUpdate(), Times.Never());
        mockStateB.Verify(state => state.OnExit(), Times.Never());
        Assert.True(stateMachine.CurrentStateIs<DemoStateA>());
        Assert.False(stateMachine.CurrentStateIs<DemoStateB>());
    }

    [Fact]
    public void AddState_WhenTryingToAddDuplicateState_ThrowsError()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = new();
        stateMachine.AddState<DemoStateA>();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            stateMachine.AddState<DemoStateA>()
        );
        Assert.Equal(
            string.Format(DUPLICATE_STATE_MESSAGE, "DemoStateA"),
            exception.Message
        );
    }

    [Fact]
    public void AddStateOverload_WhenTryingToAddDuplicateState_ThrowsError()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = new();
        var state = new DemoStateA();
        var stateDuplicate = new DemoStateA();
        stateMachine.AddState(state);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            stateMachine.AddState(stateDuplicate)
        );
        Assert.Equal(
            string.Format(DUPLICATE_STATE_MESSAGE, "DemoStateA"),
            exception.Message
        );
    }

    [Fact]
    public void RemoveState_WhenTryingToRemoveStoredState_RemovesState()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = CreateDefaultStateMachine();

        // Act 
        stateMachine.RemoveState<DemoStateA>();

        // Assert
        Assert.False(stateMachine.HasStateStored<DemoStateA>());
        Assert.True(stateMachine.HasStateStored<DemoStateB>());
    }

    [Fact]
    public void RemoveState_WhenTryingToRemoveUnStoredState_ThrowsError()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = new();
        stateMachine.AddState<DemoStateB>();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            stateMachine.RemoveState<DemoStateA>()
        );
        Assert.Equal(
            "The State \"DemoStateA\" was requested for removal, but is not stored in this StateMachine.",
            exception.Message
        );
    }

    [Fact]
    public void RequestState_WhenTryingToRequestUnStoredState_ThrowsError()
    {
        // Arrange
        EmptyTestingStateMachine stateMachine = new();
        stateMachine.AddState<DemoStateB>();

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() =>
            stateMachine.RequestState<DemoStateA>()
        );
        Assert.Equal(
            "A change to State \"DemoStateA\" was requested, but the State is not stored in this StateMachine.",
            exception.Message
        );
    }
}
