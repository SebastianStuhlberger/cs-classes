/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

using StateMachine.ExampleClasses;
using Tests.StateMachine.TestingClasses;

namespace Tests.StateMachine;

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
        Assert.True(testMachine.CurrentStateIs<DemoStateB>());
        Assert.False(testMachine.CurrentStateIs<DemoStateA>());
    }
}
