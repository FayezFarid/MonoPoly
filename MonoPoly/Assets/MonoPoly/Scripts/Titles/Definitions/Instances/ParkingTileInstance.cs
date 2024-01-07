using System;


public class ParkingTileInstance : TileInstance, IMoneyTrader
{
    private int _money = 0;

    public int Money => _money;

    public void ReduceMoney(int amount)
    {
        _money -= amount;
    }

    public int IncreaseMoney(int amount)
    {
        _money += amount;
        return amount;
    }

    public void PayPlayer(IMoneyTrader player, int amount)
    {
        int NewAmount = player.IncreaseMoney(amount);
        ReduceMoney(NewAmount);
    }
    public bool CanNotAfford(int amount)
    {
        SpicyHarissaLogger.LogError("Calling CanNot Afford or parking tile");
        return true;
    }
    public override void OnLandedUpon()
    {
    }

    public override void UpgradeLand()
    {
    }


}