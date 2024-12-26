/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

using StateMachine;
using StateMachine.ExampleClasses;

namespace Tests;

public class StateMachineTests
{
    [Fact]
    public void Initialize_WhenCalled_SetsUpDefaultState()
    {
        // Arrange
        EmptyTestingStateMachine testMachine = new();
        testMachine.AddState<DemoStateB>();
        testMachine.AddState<DemoStateA>();

        // Act
        testMachine.Initialize();

        // Assert
        Assert.False(testMachine.CurrentStateIs<DemoStateA>());
        Assert.True(testMachine.CurrentStateIs<DemoStateB>());
    }
}
