using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameOfLife
{
    

    class Program
    {
        private static Random rand;
        private static Tile bridgeOutSetback;
        private static Tile start;
        private static Tile polarExpedition;
        private static Tile firstPayday;
        private static List<Tile> tiles;


        static void Main(string[] args)
        {
            start = SetupBoard();

            // Define set of players and their strategies
            List<Player> players = new List<Player>();
            players.Add(new Player(0, start, true, true, true, true, true, true, true));
            players.Add(new Player(1, start, true, true, true, true, true, true, true));
            players.Add(new Player(2, start, true, true, true, true, true, true, true));
            players.Add(new Player(3, start, true, true, true, true, true, true, true));
            players.Add(new Player(4, start, true, true, true, true, true, true, true));

            // Run the simulation
            simulateGames(players, false, 100000);

            // Wait to exit
            Console.WriteLine("Press Enter to Exit");
            Console.ReadLine();
        }


        static void simulateGames(List<Player> players, bool randomOrder, int numGames)
        {
            int[] numWins = new int[players.Count];
            for (int i = 0; i < players.Count; i++)
            {
                numWins[i] = 0;
            }
            
            int numSons = 0;
            int numDaughters = 0;
            int minTCash = int.MaxValue;
            int maxTCash = int.MinValue;
            long avgCash = 0;
            int num = 0;            

            DateTime startTime = DateTime.Now;

            for (int i = 0; i < numGames; i++)
            {
                simulateGame(players, randomOrder);

                Player winner = null;
                int maxCash = int.MinValue;

                foreach (Player p in players)
                {
                    if (p.cash > maxCash)
                    {
                        maxCash = p.cash;
                        winner = p;
                    }
                    else if (p.cash == maxCash)
                    {
                        winner = null;
                        break;
                    }
                }

                if (winner != null)
                {
                    numWins[winner.Id]++;
                }

                foreach (Player p in players)
                {
                    numSons += p.sons;
                    numDaughters += p.daughters;
                    num++;
                    if (p.cash < minTCash)
                        minTCash = p.cash;

                    if (p.cash > maxTCash)
                        maxTCash = p.cash;

                    avgCash += p.cash;
                    p.Reset(start);
                }

                resetTiles();
            }

            foreach (Player p in players)
            {
                Console.WriteLine("Player " + p.Id + ": " + ((double)numWins[p.Id]) / numGames);
            }

            Console.WriteLine("Average Sons: " + ((double)numSons) / num);
            Console.WriteLine("Average Daughters: " + ((double)numDaughters) / num);
            Console.WriteLine("Max Cash: " + maxTCash);
            Console.WriteLine("Min Cash: " + minTCash);
            Console.WriteLine("Average Cash: " + (avgCash) / (long)num);
            Console.WriteLine("Time: " + (DateTime.Now.Ticks - startTime.Ticks) / 10000000 + " seconds");
        }


        static void simulateGame(List<Player> players, bool randomOrder)
        {
            List<Player> playerCopy = new List<Player>(players);
            List<Player> randomPlayers = new List<Player>();
            if (randomOrder)
            {
                while (playerCopy.Count > 0)
                {
                    if (rand == null)
                    {
                        rand = new Random();
                    }
                    int r = rand.Next(0, playerCopy.Count);
                    randomPlayers.Add(playerCopy[r]);
                    playerCopy.RemoveAt(r);
                }
            }
            else
            {
                randomPlayers = new List<Player>(players);
            }

            // Buy auto insurance
            foreach(Player p in randomPlayers)
            {
                if (!p.hasAuto && p.BuyAuto)
                {
                    p.hasAuto = true;
                    p.cash -= 1000;
                }
            }
            

            bool allDone = false;
            while (!allDone)
            {
                allDone = true;
                foreach (Player p in randomPlayers)
                {
                    if (p.location == null)
                    {
                        continue;
                    }

                    allDone = false;

                    if (p.missTurn)
                    {
                        p.missTurn = false;
                    }
                    else
                    {
                        simulateRoll(p, randomPlayers);
                    }
                }
            }
        }

        static void simulateRoll(Player p, List<Player> players)
        { 
            List<Player> others = new List<Player>(players);
            others.Remove(p);

            // Spin the wheel
            int spin = spinWheel();           

            // Move based on spin
            for (int i = 1; i <= spin; i++)
            {   
                p.location = p.location.getNextTile(p);
            
                // Check on action when passing tiles
                if (i < spin)
                {
                    if (p.location.passAction(p, others) == ReturnCode.STOP)
                    {
                        break;
                    }
                }
            }


            // Do action
            int cashStart = p.cash;
            Roll r = new Roll();
            r.roll = spin;
            r.tile = p.location;
            switch (p.location.landAction(p, others))
            {
                case ReturnCode.BRIDGE_OUT:
                    p.location = bridgeOutSetback;
                    break;
                case ReturnCode.RETURN_TO_START:
                    p.location = start;
                    break;
                case ReturnCode.ROLL_AGAIN:
                    simulateRoll(p, players);
                    break;
                case ReturnCode.MILLIONAIRE:
                    p.location = null;
                    break;
                case ReturnCode.CAREER:
                    p.location = firstPayday;
                    p.location.landAction(p, others);
                    break;
                case ReturnCode.DETOUR:
                    p.location = polarExpedition;
                    polarExpedition.landAction(p, others);
                    break;
            }

            r.delta = p.cash - cashStart;
            p.rollLog.Add(r);
        }


        static int spinWheel()
        {
            if (rand == null)
            {
                rand = new Random();
            }

            return rand.Next(1, 11);
        }


        static void resetTiles()
        {
            foreach (Tile t in tiles)
            {
                t.reset();
            }
        }

        static Tile SetupBoard()
        {
            tiles = new List<Tile>();

            Tile next = new TileMillionaire();
            tiles.Add(next);
            Tile current = new TilePhonyDiamond();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileLifeInsuranceMatures();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileSellStock();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileDayOfReckoning();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileWorldCruise();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileRareCoins();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileEuropeanTour();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileRevenge();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileLuckyDay();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileStrickOil();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileLifeInsurance();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileRobbed();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileOldDebts();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileGameShowWinner();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePaydayInterest();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileFashion();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileTornado();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileSellCattleRanch();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePropertyTaxes();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileRoyalties();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileStrikeOilTollbridge();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileGoFishing();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileNobelPrize();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileLuxuryCar();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBridgeOut();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileYachtRamsIceberg();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileInheritCattleRanch();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileCareless();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileLuckyDay();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileFalseTeeth();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileHelpHomeless();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileStocksAreUp();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileLifeInsurance120();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePaydayInterest();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileFavoriteCharity();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileRevenge();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileAtlantis();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileTaxesDue();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBuyAYacht();
            current.addNextTile(next);
            tiles.Add(current); 
            bridgeOutSetback = current;
            next = current;

            current = new TilePlayTheMarket();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileGoatEatsOrchids();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBuyAHellicopter();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePipeBursts();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileWriteBestSeller();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBuyNewHome();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileUncleInJail();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileTakeWorldwideCruise();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileJustSpun3();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBurglerStrikes();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileGoldMine();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileUncleLeavesSkunkFarm();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileSpinTheWheel();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileLuckyDay();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePaydayInterest();
            Tile paydayInterest = current;
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBusinessIsBooming();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileStockDividend();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileDetour();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileAdoptASonAndDaughter();
            Tile lastFork = current;
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileWinTennis();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileRecklessDriver();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileRevenge();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePlayTheMarket();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileSavePollutedLake();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileLuckyDay();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileTaxesDue();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileDaughterIsBorn();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileClimbMtEverest();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileLifeInsurance70();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePaydayInterest();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileElectronicsStock();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileStockPricesDrop();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileExpandBusiness();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileStocksAreUp240();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileLuckyDay();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileSonIsBorn();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileRevenge();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBuyOfficeBuilding();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileInnovationPaysOff();
            Tile inventionPaysOff = current;
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileJustSpun8();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileStockPricesDrop22();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileWinMountainRoadRace();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileDaughterIsBorn();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileSpinTheWheel();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileSonIsBornFork();
            Tile mountainFork = current;
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileIfYouWantStock();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileTaxesDue();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileTwinSonsAreBorn();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileRevenge();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileWinOnHorses();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileDaughterIsBorn();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileAuntLeaves50Cats();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileSonIsBorn();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileLuckyDay();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileIfYouWantFireInsurance();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBuyAHouse();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileVisitInLaws();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileWinTheLottery();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileHelpFatherInLaw();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBuyTwoHorses();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBusinessNeedsToAdvertise();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileCarAccident();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBuyFurniture();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileJuryDuty();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBusinessTrip();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileGetMarried();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileUncleNeedsHelp();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileFindArtTreasure();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileTornado();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileIfYouWantLifeInsurance();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileInheritance();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileStartABusiness();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileDiscoverUranium();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBirthday();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TilePayday();
            firstPayday = current;
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileRentApartment();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileContestWinner();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileBusinessSalary();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileCorrespondenceCourse();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileCaptureLion();
            Tile CaptureLion = current;
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileSlowStart();
            current.addNextTile(next);
            tiles.Add(current); 
            next = current;

            current = new TileStart();
            start = current;
            current.addNextTile(next);
            tiles.Add(current);

            // University fork
            current = new TileCareerUniversity();
            current.addNextTile(firstPayday);
            tiles.Add(current);
            next = current;

            current = new TileCareerPhysicist();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileCareerTeacher();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileCareerLawyer();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileCareerJournalist();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileCareerDoctor();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileGamblingLoss();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TilePrizePhoto();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileStudy();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileTuition();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileScholarship();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileCarRepairs();
            current.addNextTile(next);
            tiles.Add(current);
            CaptureLion.addNextTile(current);          


            // non-Mountain fork
            current = new TileWreckedCar();
            current.addNextTile(inventionPaysOff);
            tiles.Add(current);
            next = current;

            current = new TileDaughterIsBorn();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileBusinessIsBooming100();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileStockPricesDrop36();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileFindFamousPainting();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TilePlayTheMarket();
            current.addNextTile(next);
            tiles.Add(current);
            mountainFork.addNextTile(current);



            // Last Fork
            current = new TileWinACase();
            current.addNextTile(paydayInterest);
            tiles.Add(current);
            next = current;

            current = new TileStockDividend18();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileHouseOnFire();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileCarAccident();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TileLifeInsurance60();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TilePayday();
            current.addNextTile(next);
            tiles.Add(current);
            next = current;

            current = new TilePolarExpedition();
            polarExpedition = current;
            current.addNextTile(next);
            tiles.Add(current);
            lastFork.addNextTile(current);

            return start;
        }
    }
}
