using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    class Wallet
    {
        //Member Variables (Has A)
        public List<Coin> Coins;
        //Constructor (Spawner)
        public Wallet()
        {
            Coins = new List<Coin>();
            FillWallet();
        }
        //Member Methods (Can Do)
        //Fills wallet with starting money
        private void FillWallet()
        {
            for (int i = 0; i < 12; i++)
            {
                Coin quarter = new Quarter();
                Coin dime = new Dime();
                Coin nickel = new Nickel();
                Coin penny = new Penny();
                Coins.Add(quarter);
                Coins.Add(dime);
                Coins.Add(nickel);
                Coins.Add(penny);
            }
            for (int i = 0; i < 8; i++)
            {
                Coin penny = new Penny();
                Coins.Add(penny);
            }
            
        }
    }
}
