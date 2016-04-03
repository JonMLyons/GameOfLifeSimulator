using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameOfLife
{
    public enum Career { NONE, DOCTOR, JOURNALIST, LAWYER, TEACHER, PHYSICIST, UNIVERSITY, BUSINESS };
    
    public struct Roll
    {
        public int roll;
        public Tile tile;
        public int delta;
    };

    public class Player
    {
        // Immutable
        private int id;

        // Parameters
        private bool tryLuckyDay;
        private bool buyStock;
        private bool buyAuto;
        private bool buyLife;
        private bool buyFire;
        private bool playTheMarket;
        private bool pickCollege;

        // Game Data
        public int salary = 0;
        public Career career = Career.NONE;
        public int cash = 10000;
        public bool married = false;
        public int sons = 0;
        public int daughters = 0;
        public bool hasStock = false;
        public bool hasAuto = false;
        public bool hasLife = false;
        public bool hasFire = false;
        public bool missTurn = false;
        public List<Roll> rollLog;
        public Tile location;


        public Player(int id, Tile start, bool tryLuckyDay, bool buyStock, bool buyAuto, bool buyLife, bool buyFire, bool playTheMarket, bool pickCollege)
        {
            this.id = id;
            this.tryLuckyDay = tryLuckyDay;
            this.buyStock = buyStock;
            this.buyAuto = buyAuto;
            this.buyLife = buyLife;
            this.buyFire = buyFire;
            this.playTheMarket = playTheMarket;
            this.pickCollege = pickCollege;

            this.rollLog = new List<Roll>();
            Reset(start);
        }        

        public void Reset(Tile start)
        {
            this.salary = 0;
            this.career = Career.NONE;
            this.cash = 10000;
            this.married = false;
            this.sons = 0;
            this.daughters = 0;
            this.hasStock = false;
            this.hasAuto = false;
            this.hasLife = false;
            this.hasFire = false;
            this.missTurn = false;
            this.location = start;
            this.rollLog.Clear();
        }

        public int Id { get { return this.id; } }
        public bool TryLuckyDay { get { return this.tryLuckyDay; } }
        public bool BuyStock { get { return this.buyStock; } }
        public bool BuyAuto { get { return this.buyAuto; } }
        public bool BuyLife { get { return this.buyLife; } }
        public bool BuyFire { get { return this.buyFire; } }
        public bool PlayTheMarket { get { return this.playTheMarket; } }
        public bool PickCollege { get { return this.pickCollege; } }
    }
}
