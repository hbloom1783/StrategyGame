namespace StrategyGame.Battle.Game.Abilities
{
    public sealed class APCost : ModularCost
    {
        public int apCost = 1;

        public override bool canPay { get { return unit.ap >= apCost; } }

        public override void PayCost()
        {
            unit.ap -= apCost;
        }
    }
}
