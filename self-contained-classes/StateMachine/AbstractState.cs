/* ========================================================================= */
/* Code by Sebastian Stuhlberger                                             */
/* ========================================================================= */

namespace StateMachine
{

    /// <summary>
    /// Derive from this class to create a new State to insert into a StateMachine.
    /// </summary>
    /// <typeparam name="StateSubGroup">The subgroup for the target StateMachine and all its states</typeparam>
    public abstract class AbstractState<StateSubGroup> where StateSubGroup : AbstractStateSubgroup
    {
        /// <summary>
        /// A reference to the StateMachine that currently manages the instance of the state.
        /// The StateMachine will automatically set this field, when the state is added to it.
        /// </summary>
        public AbstractStateMachine<StateSubGroup> StateMachine { get; set; }

        /// <summary>
        /// This method is called once, when the state is entered.
        /// </summary>
        public abstract void OnEnter();

        /// <summary>
        /// This method is called once, right before the state is exited.
        /// </summary>
        public abstract void OnExit();

        /// <summary>
        /// This method is called once, every time the StateMachine undergoes an update-tick.
        /// </summary>
        public abstract void OnUpdate();
    }

}
