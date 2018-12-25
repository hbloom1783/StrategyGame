namespace StrategyGame.Game
{
    public class StateMachine : Athanor.StateMachine.StateMachine
    {
        protected override void LeaveCurrentState()
        {
            GameController.instance.input.ClearEvents();
            if (Battle.Map.MapController.instance != null)
                Battle.Map.MapController.instance.events.ClearEvents();
            if (Strategic.Map.MapController.instance != null)
                Strategic.Map.MapController.instance.events.ClearEvents();

            base.LeaveCurrentState();
        }
    }
}
