using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameOfLife
{
    public enum ReturnCode { NONE, ROLL_AGAIN, STOP, BRIDGE_OUT, RETURN_TO_START, MILLIONAIRE, CAREER, DETOUR };


    public abstract class Tile
    {

        protected List<Tile> tiles;
        private static Random rand;

        public abstract ReturnCode landAction(Player p, List<Player> otherPlayers);

        public virtual ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            return ReturnCode.NONE;
        }

        public virtual void reset()
        {
        }

        public void addNextTile(Tile t)
        {
            if (tiles == null)
            {
                this.tiles = new List<Tile>();
            }
            this.tiles.Add(t);
        }

        public virtual Tile getNextTile(Player p)
        {
            if (tiles != null)
            {
                return tiles[0];
            }
            else
            {
                return null;
            }
        }

        protected static int getRandInt(int min, int max)
        {
            if (rand == null)
            {
                rand = new Random();
            }

            return rand.Next(min, max + 1);
        }
    }

    public class TileStart : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (!p.hasAuto && p.BuyAuto)
            {
                p.hasAuto = true;
                p.cash -= 1000;
            }

            return ReturnCode.NONE;
        }
    }

    public class TileSlowStart : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            return ReturnCode.ROLL_AGAIN;
        }
    }

    public class TileCaptureLion : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 4000;
            return ReturnCode.NONE;
        }

        public override Tile getNextTile(Player p)
        {
            if (p.PickCollege && p.career == Career.NONE)
            {
                if (tiles[0].GetType() == typeof(TileCarRepairs))
                {
                    return tiles[0];
                }
                else
                {
                    return tiles[1];
                }
            }
            else
            {
                if (tiles[0].GetType() == typeof(TileCarRepairs))
                {
                    return tiles[1];
                }
                else
                {
                    return tiles[0];
                }
            }
        }
    }

    public class TilePayday : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += p.salary;
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            p.cash += p.salary;
            return ReturnCode.NONE;
        }
    }

    public class TileCorrespondenceCourse : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 1000;
            return ReturnCode.NONE;
        }
    }

    public class TileBusinessSalary : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.career == Career.NONE)
            {
                p.career = Career.BUSINESS;
                p.salary = 12000;
            }
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            if (p.career == Career.NONE)
            {
                p.career = Career.BUSINESS;
                p.salary = 12000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileContestWinner : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 12000;
            return ReturnCode.NONE;
        }
    }

    public class TileRentApartment : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 1000;
            return ReturnCode.NONE;
        }
    }

    public class TileBirthday : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 1000;
            return ReturnCode.NONE;
        }
    }

    public class TileDiscoverUranium : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 240000;
            return ReturnCode.NONE;
        }
    }

    public class TileStartABusiness : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 30000;
            return ReturnCode.NONE;
        }
    }

    public class TileInheritance : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 300000;
            return ReturnCode.NONE;
        }
    }

    public class TileIfYouWantLifeInsurance : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        private void doAction(Player p)
        {
            if (p.BuyLife && !p.hasLife)
            {
                p.cash -= 10000;
                p.hasLife = true;
            }
        }
    }

    public class TileTornadoStrikes : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            return ReturnCode.RETURN_TO_START;
        }
    }

    public class TileFindArtTreasure : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 120000;
            return ReturnCode.NONE;
        }
    }

    public class TileUncleNeedsHelp : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 2000;
            return ReturnCode.NONE;
        }
    }

    public class TileGetMarried : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.married = true;
            int rand = getRandInt(1, 10);
            int amount = 0;
            if (rand <= 3)
            {
                amount = 2000;
            }
            else if (rand <= 6)
            {
                amount = 1000;
            }

            foreach (Player other in otherPlayers)
            {
                p.cash += amount;
                other.cash -= amount;
            }

            return ReturnCode.ROLL_AGAIN;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            return ReturnCode.STOP;
        }
    }

    public class TileBusinessTrip : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 3000;
            return ReturnCode.NONE;
        }
    }

    public class TileJuryDuty : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.missTurn = true;
            return ReturnCode.NONE;
        }
    }

    public class TileBuyFurniture : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 6000;
            return ReturnCode.NONE;
        }
    }

    public class TileCarAccident : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (!p.hasAuto)
            {
                p.cash -= 12000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileBusinessNeedsToAdvertise : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 60000;
            return ReturnCode.NONE;
        }
    }

    public class TileBuyTwoHorses : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 60000;
            return ReturnCode.NONE;
        }
    }

    public class TileHelpFatherInLaw : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 20000;
            return ReturnCode.NONE;
        }
    }

    public class TileWinTheLottery : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 96000;
            return ReturnCode.NONE;
        }
    }

    public class TileVisitInLaws : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.missTurn = true;
            return ReturnCode.NONE;
        }
    }

    public class TileBuyAHouse : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        private void doAction(Player p)
        {
            p.cash -= 40000;
        }
    }

    public class TileIfYouWantFireInsurance : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        private void doAction(Player p)
        {
            if (p.BuyFire && !p.hasFire)
            {
                p.cash -= 10000;
                p.hasFire = true;
            }
        }
    }

    public class TileAuntLeaves50Cats : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 20000;
            return ReturnCode.NONE;
        }
    }

    public class TileWinOnHorses : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 300000;
            return ReturnCode.NONE;
        }
    }

    public class TileTwinSonsAreBorn : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.sons += 2;
            foreach (Player other in otherPlayers)
            {
                p.cash += 2000;
                other.cash -= 2000;
            }

            return ReturnCode.NONE;
        }
    }

    public class TileIfYouWantStock : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        private void doAction(Player p)
        {
            if (p.BuyStock && !p.hasStock)
            {
                p.cash -= 50000;
                p.hasStock = true;
            }
        }
    }

    public class TileWinMountainRoadRace : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 280000;
            return ReturnCode.NONE;
        }
    }

    public class TileStockPricesDrop22 : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash -= 22000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileJustSpun8 : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            // TODO: Use real roll value
            int rand = getRandInt(1, 10);
            if (rand == 8)
            {
                p.cash += 80000;
            }

            return ReturnCode.NONE;
        }
    }

    public class TileInnovationPaysOff : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 50000;
            return ReturnCode.NONE;
        }
    }

    public class TileBuyOfficeBuilding : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 150000;
            return ReturnCode.NONE;
        }
    }

    public class TileSonIsBorn : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.sons++;
            foreach (Player other in otherPlayers)
            {
                p.cash += 1000;
                other.cash -= 1000;
            }

            return ReturnCode.NONE;
        }
    }

    public class TileSonIsBornFork : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.sons++;
            foreach (Player other in otherPlayers)
            {
                p.cash += 1000;
                other.cash -= 1000;
            }

            return ReturnCode.NONE;
        }

        public override Tile getNextTile(Player p)
        {
            int rand = getRandInt(0, 1);
            return this.tiles[rand];
        }
    }

    public class TileStocksAreUp240 : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash += 240000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileExpandBusiness : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 120000;
            return ReturnCode.NONE;
        }
    }

    public class TileStockPricesDrop : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash -= 16000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileElectronicsStock : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash += 120000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileLifeInsurance70 : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasLife)
            {
                p.cash += 70000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileClimbMtEverest : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 120000;
            return ReturnCode.NONE;
        }
    }

    public class TileDaughterIsBorn : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.daughters++;
            foreach (Player other in otherPlayers)
            {
                p.cash += 1000;
                other.cash -= 1000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileSavePollutedLake : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 240000;
            return ReturnCode.NONE;
        }
    }

    public class TileRecklessDriver : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.hasAuto = false;
            return ReturnCode.NONE;
        }
    }

    public class TileWinTennis : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 100000;
            return ReturnCode.NONE;
        }
    }

    public class TileAdoptASonAndDaughter : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.sons++;
            p.daughters++;
            foreach (Player other in otherPlayers)
            {
                p.cash += 2000;
                other.cash -= 2000;
            }

            return ReturnCode.NONE;
        }

        public override Tile getNextTile(Player p)
        {
            int rand = getRandInt(0, 1);
            return this.tiles[rand];
        }
    }

    public class TileDetour : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            return ReturnCode.DETOUR;
        }
    }

    public class TileStockDividend : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash += 12000;
            }

            return ReturnCode.NONE;
        }
    }

    public class TileBusinessIsBooming : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 24000;
            return ReturnCode.NONE;
        }
    }

    public class TileSpinTheWheel : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            int rand = getRandInt(1, 10);
            p.cash += rand * 1000;
            return ReturnCode.NONE;
        }
    }


    public class TileUncleLeavesSkunkFarm : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 20000;
            return ReturnCode.NONE;
        }
    }

    public class TileGoldMine : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash += 360000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileBurglerStrikes : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 10000;
            return ReturnCode.NONE;
        }
    }

    public class TileJustSpun3 : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            //TODO: Make this reflect the real roll value

            int rand = getRandInt(1, 10);
            if (rand == 3)
            {
                p.cash += 30000;
            }

            return ReturnCode.NONE;
        }
    }

    public class TileTakeWorldwideCruise : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 20000;
            return ReturnCode.NONE;
        }
    }

    public class TileUncleInJail : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 2000;
            return ReturnCode.NONE;
        }
    }

    public class TileBuyNewHome : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 140000;
            return ReturnCode.NONE;
        }
    }

    public class TileWriteBestSeller : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 96000;
            return ReturnCode.NONE;
        }
    }

    public class TilePipeBursts : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 20000;
            return ReturnCode.NONE;
        }
    }

    public class TileBuyAHellicopter : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 100000;
            return ReturnCode.NONE;
        }
    }

    public class TileGoatEatsOrchids : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 3000;
            return ReturnCode.NONE;
        }
    }


    public class TilePlayTheMarket : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        private void doAction(Player p)
        {
            if (p.PlayTheMarket && p.hasStock)
            {
                int rand = getRandInt(1, 10);
                if (rand <= 3)
                {
                    p.cash -= 60000;
                }
                else if (rand >= 7)
                {
                    p.cash += 120000;
                }
            }
        }
    }

    public class TileBuyAYacht : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 80000;
            return ReturnCode.NONE;
        }
    }

    public class TileTaxesDue : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= p.salary / 2;
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= p.salary / 2;
            return ReturnCode.NONE;
        }
    }

    public class TileAtlantis : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 12000;
            return ReturnCode.NONE;
        }
    }

    public class TileFavoriteCharity : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 10000;
            return ReturnCode.NONE;
        }
    }

    public class TileLifeInsurance120 : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasLife)
            {
                p.cash += 120000;
            }
            return ReturnCode.NONE;
        }
    }


    public class TileStocksAreUp : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash += 140000;
            }

            return ReturnCode.NONE;
        }
    }

    public class TileHelpHomeless : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 120000;
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 120000;
            return ReturnCode.NONE;
        }
    }

    public class TileFalseTeeth : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 2000;
            return ReturnCode.NONE;
        }
    }

    public class TileCareless : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.hasFire = false;
            return ReturnCode.NONE;
        }
    }

    public class TileInheritCattleRanch : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 240000;
            return ReturnCode.NONE;
        }
    }

    public class TileYachtRamsIceberg : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 10000;
            return ReturnCode.NONE;
        }
    }

    public class TileBridgeOut : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            return ReturnCode.BRIDGE_OUT;
        }
    }

    public class TileLuxuryCar : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 40000;
            return ReturnCode.NONE;
        }
    }

    public class TileNobelPrize : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 120000;
            return ReturnCode.NONE;
        }
    }

    public class TileGoFishing : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.missTurn = true;
            return ReturnCode.NONE;
        }
    }

    public class TileStrikeOilTollbridge : Tile
    {
        private Player firstOver = null;

        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash += 480000;
            }

            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            if (firstOver == null)
            {
                firstOver = p;
            }
            else
            {
                if (firstOver != p)
                {
                    p.cash -= 24000;
                    firstOver.cash += 24000;
                }
            }

            return ReturnCode.NONE;
        }

        public override void reset()
        {
            this.firstOver = null;
        }
    }

    public class TileRoyalties : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 120000;
            return ReturnCode.NONE;
        }
    }

    public class TilePropertyTaxes : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 50000;
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 50000;
            return ReturnCode.NONE;
        }
    }

    public class TileSellCattleRanch : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 200000;
            return ReturnCode.NONE;
        }
    }

    public class TileTornado : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (!p.hasFire)
            {
                p.cash -= 100000;
            }

            return ReturnCode.NONE;
        }
    }

    public class TileFashion : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 10000;
            return ReturnCode.NONE;
        }
    }


    public class TilePaydayInterest : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            doAction(p);
            return ReturnCode.NONE;
        }

        private void doAction(Player p)
        {
            p.cash += p.salary;
            if (p.cash < 0)
            {
                // Pay interest on loan
                p.cash = (int)((double)p.cash * 1.05);
            }
        }
    }

    public class TileGameShowWinner : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 240000;
            return ReturnCode.NONE;
        }
    }

    public class TileOldDebts : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 3000;
            return ReturnCode.NONE;
        }
    }

    public class TileRobbed : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 70000;
            return ReturnCode.NONE;
        }
    }

    public class TileLifeInsurance : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasLife)
            {
                p.cash += 240000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileStrickOil : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash += 480000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileLuckyDay : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.TryLuckyDay)
            {
                if (getRandInt(1, 10) <= 2)
                {
                    p.cash += 300000;
                }
            }
            else
            {
                p.cash += 20000;
            }

            return ReturnCode.NONE;
        }
    }


    public class TileRevenge : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            Player other = otherPlayers[getRandInt(0, otherPlayers.Count - 1)];
            if (other.cash > 0)
            {
                int amount = other.cash < 200000 ? other.cash : 200000;
                p.cash += amount;
                other.cash -= amount;
            }

            return ReturnCode.NONE;
        }
    }

    public class TileEuropeanTour : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 20000;
            return ReturnCode.NONE;
        }
    }

    public class TileRareCoins : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 40000;
            return ReturnCode.NONE;
        }
    }

    public class TileWorldCruise : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 70000;
            return ReturnCode.NONE;
        }
    }


    public class TileDayOfReckoning : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.cash < 0)
            {
                p.cash = (int)((double)p.cash * 1.25);
            }

            p.cash += (p.sons + p.daughters) * 48000;
            return ReturnCode.ROLL_AGAIN;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            return ReturnCode.STOP;
        }
    }

    public class TileSellStock : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash += 600000;
            }

            return ReturnCode.NONE;
        }
    }

    public class TileLifeInsuranceMatures : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasLife)
            {
                p.cash += 240000;
            }
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasLife)
            {
                p.cash += 240000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TilePhonyDiamond : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 20000;
            return ReturnCode.NONE;
        }
    }


    public class TileMillionaire : Tile
    {
        private bool isFirst = true;

        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (isFirst)
            {
                p.cash += 240000;
            }

            if (p.hasStock)
            {
                p.cash += 120000;
            }
            if (p.hasLife)
            {
                p.cash += 8000;
            }

            isFirst = false;

            return ReturnCode.MILLIONAIRE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            return ReturnCode.STOP;
        }

        public override void reset()
        {
            this.isFirst = true;
        }
    }

    public class TileCarRepairs : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 2000;
            return ReturnCode.NONE;
        }
    }

    public class TileScholarship : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 3000;
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 3000;
            return ReturnCode.NONE;
        }
    }

    public class TileTuition : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 5000;
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 5000;
            return ReturnCode.NONE;
        }
    }

    public class TileStudy : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.missTurn = true;
            return ReturnCode.NONE;
        }
    }

    public class TilePrizePhoto : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 2000;
            return ReturnCode.NONE;
        }
    }

    public class TileGamblingLoss : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 2000;
            return ReturnCode.NONE;
        }
    }

    public class TileCareerDoctor : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.career = Career.DOCTOR;
            p.salary = 50000;
            return ReturnCode.CAREER;
        }
    }

    public class TileCareerJournalist : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.career = Career.JOURNALIST;
            p.salary = 24000;
            return ReturnCode.CAREER;
        }
    }

    public class TileCareerLawyer : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.career = Career.LAWYER;
            p.salary = 50000;
            return ReturnCode.CAREER;
        }
    }

    public class TileCareerTeacher : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.career = Career.TEACHER;
            p.salary = 20000;
            return ReturnCode.CAREER;
        }
    }

    public class TileCareerPhysicist : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.career = Career.PHYSICIST;
            p.salary = 30000;
            return ReturnCode.CAREER;
        }
    }

    public class TileCareerUniversity : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.career = Career.UNIVERSITY;
            p.salary = 16000;
            return ReturnCode.NONE;
        }

        public override ReturnCode passAction(Player p, List<Player> otherPlayers)
        {
            p.career = Career.UNIVERSITY;
            p.salary = 16000;
            return ReturnCode.NONE;
        }
    }

    public class TileFindFamousPainting : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 480000;
            return ReturnCode.NONE;
        }
    }

    public class TileStockPricesDrop36 : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash -= 36000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileBusinessIsBooming100 : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 100000;
            return ReturnCode.NONE;
        }
    }

    public class TileWreckedCar : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (!p.hasAuto)
            {
                p.cash -= 16000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TilePolarExpedition : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash -= 100000;
            return ReturnCode.NONE;
        }
    }

    public class TileLifeInsurance60 : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasLife)
            {
                p.cash += 60000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileCarAccident50 : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (!p.hasAuto)
            {
                p.cash -= 50000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileHouseOnFire : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (!p.hasFire)
            {
                p.cash -= 60000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileStockDividend18 : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            if (p.hasStock)
            {
                p.cash += 18000;
            }
            return ReturnCode.NONE;
        }
    }

    public class TileWinACase : Tile
    {
        public override ReturnCode landAction(Player p, List<Player> otherPlayers)
        {
            p.cash += 240000;
            return ReturnCode.NONE;
        }
    }
}
