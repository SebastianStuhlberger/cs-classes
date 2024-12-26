# AbstractStateMachine

A fully abstract state machine that is built around C# types instead of enums or state-objects.
This allows for clean method calls like `stateMachine.ChangeState<NewTargetStateType>();`

### Note

Some functionality found in this version has by now been refactored in the generic StateMachine found in the [self-contained-classes directory](https://github.com/SebastianStuhlberger/cs-classes/tree/main/self-contained-classes). Check the code there for the up-to-date and tested version.
