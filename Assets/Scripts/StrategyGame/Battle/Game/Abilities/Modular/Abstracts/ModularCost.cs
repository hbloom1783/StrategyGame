namespace StrategyGame.Battle.Game.Abilities
{
    abstract public class ModularCost : ModularAttribute
    {
        public abstract bool canPay { get; }
        public abstract void PayCost();
    }
}
