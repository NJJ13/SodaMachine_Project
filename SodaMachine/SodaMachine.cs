﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SodaMachine
{
    class SodaMachine
    {
        //Member Variables (Has A)
        private List<Coin> _register;
        private List<Can> _inventory;
        private List<decimal> creditCardPayments;

        //Constructor (Spawner)
        public SodaMachine()
        {
            _register = new List<Coin>();
            _inventory = new List<Can>();
            creditCardPayments = new List<decimal>();
            FillInventory();
            FillRegister();
            
        }

        //Member Methods (Can Do)

        //A method to fill the sodamachines register with coin objects.
        public void FillRegister()
        {
            while (_register.Count < 40)
            {
                Coin dime = new Dime();
                _register.Add(dime);
                Coin nickel = new Nickel();
                _register.Add(nickel);
                Coin quarter = new Quarter();
                _register.Add(quarter);
                Coin penny = new Penny();
                _register.Add(penny);
            }
            while (_register.Count < 70)
            {
                Coin nickel = new Nickel();
                _register.Add(nickel);
                Coin quarter = new Quarter();
                _register.Add(quarter);
                Coin penny = new Penny();
                _register.Add(penny);
            }
            while (_register.Count < 100)
            {
                Coin penny = new Penny();
                _register.Add(penny);
            }
                      
        }
        //A method to fill the sodamachines inventory with soda can objects.
        public void FillInventory()
        {
            while(_inventory.Count < 30)
            {
                Can orangeSoda = new OrangeSoda();
                Can cola = new Cola();
                Can rootbeer = new RootBeer();
                _inventory.Add(orangeSoda);
                _inventory.Add(cola);
                _inventory.Add(rootbeer);
            }
        }
        //Method to be called to start a transaction.
        //Takes in a customer which can be passed freely to which ever method needs it.
        public void BeginTransaction(Customer customer)
        {
            bool willProceed = UserInterface.DisplayWelcomeInstructions(_inventory);
            if (willProceed)
            {
                Transaction(customer);
            }
        }
        
        //This is the main transaction logic think of it like "runGame".  This is where the user will be prompted for the desired soda.
        //grab the desired soda from the inventory.
        //get payment from the user.
        //pass payment to the calculate transaction method to finish up the transaction based on the results.
        private void Transaction(Customer customer)
        {
            string nameofSoda = UserInterface.SodaSelection(_inventory);
            Can selectedCan = GetSodaFromInventory(nameofSoda);
            List<Coin> payment = customer.GatherCoinsFromWallet(selectedCan);
            CalculateTransaction(payment, selectedCan, customer);

        }
        //Gets a soda from the inventory based on the name of the soda.
        private Can GetSodaFromInventory(string nameOfSoda)
        {
            foreach (Can can in _inventory)
            {
                if(nameOfSoda == can.name)
                {
                    return can;
                }
            }
            return null;
        }

        //This is the main method for calculating the result of the transaction.
        //It takes in the payment from the customer, the soda object they selected, and the customer who is purchasing the soda.
        //This is the method that will determine the following:
        //If the payment is greater than the price of the soda, and if the sodamachine has enough change to return: Dispense soda, and change to the customer.
        //If the payment is greater than the cost of the soda, but the machine does not have ample change: Dispense payment back to the customer.
        //If the payment is exact to the cost of the soda:  Dispense soda.
        //If the payment does not meet the cost of the soda: dispense payment back to the customer.
        private void CalculateTransaction(List<Coin> payment, Can chosenSoda, Customer customer)
        {
            decimal valueOfPayment = TotalCoinValue(payment);
            decimal amountOfChange = DetermineChange(valueOfPayment, chosenSoda.Price);
            if (amountOfChange < 0)
            {
                decimal remainingBalance = chosenSoda.Price - valueOfPayment;
                UserInterface.OutputText("There was not enough coins deposited in order to purchase " + chosenSoda.name + ".");
                bool willProceed = UserInterface.ContinuePrompt("Would you like to put the remaining balance of " + (remainingBalance) + " on your credit card ? (y/n)");
                if (willProceed == true)
                {
                    if (customer.Wallet.creditCard.Value > remainingBalance)
                    {
                        DepositCoinsIntoRegister(payment);
                        ChargeCreditCard(remainingBalance, customer.Wallet.creditCard);
                        _inventory.Remove(chosenSoda);
                        customer.AddCanToBackpack(chosenSoda);
                        UserInterface.OutputText("Your credit card was charged " + remainingBalance + ".");
                        UserInterface.EndMessage(chosenSoda.name, 0);
                    } 
                }
                else
                {
                    customer.AddCoinsIntoWallet(payment);
                    UserInterface.DisplayError("Transaction can not be completed. There was not enough money deposited in order to purchase " + chosenSoda.name + ".");
                }
                
            }
            else if (amountOfChange == 0)
            {
                DepositCoinsIntoRegister(payment);
                _inventory.Remove(chosenSoda);
                customer.AddCanToBackpack(chosenSoda);
                UserInterface.EndMessage(chosenSoda.name, 0);
            }
            else if (amountOfChange > 0)
            {
                List<Coin> returnedChange = GatherChange(amountOfChange);
                if (returnedChange == null)
                {
                    customer.AddCoinsIntoWallet(payment);
                    UserInterface.DisplayError("Transaction can not be completed. Soda machine does not have enough change to complete the transaction.");
                }
                else
                {
                    DepositCoinsIntoRegister(payment);
                    _inventory.Remove(chosenSoda);
                    customer.AddCanToBackpack(chosenSoda);
                    customer.AddCoinsIntoWallet(returnedChange);
                    UserInterface.EndMessage(chosenSoda.name, amountOfChange);
                }
            }
        }
        //Takes in the value of the amount of change needed.
        //Attempts to gather all the required coins from the sodamachine's register to make change.
        //Returns the list of coins as change to despense.
        //If the change cannot be made, return null.
        private List<Coin> GatherChange(decimal changeValue)
        {
            List<Coin> changeToDispense = new List<Coin>();
            while (changeValue > 0)
            {
                while (changeValue >= .25m)
                {
                    foreach (Coin coin in _register)
                    {
                        changeToDispense.Add(GetCoinFromRegister("Quarter"));
                        changeValue -= .25m;
                        break;
                    }
                }
                while (changeValue >= .10m)
                {
                    foreach (Coin coin in _register)
                    {
                        changeToDispense.Add(GetCoinFromRegister("Dime"));
                        changeValue -= .10m;
                        break;
                    }
                }
                while (changeValue >= .05m)
                {
                    foreach (Coin coin in _register)
                    {
                        changeToDispense.Add(GetCoinFromRegister("Nickel"));
                        changeValue -= .05m;
                        break;
                    }
                }
                if (changeValue >= .01m)
                {
                    foreach (Coin coin in _register)
                    {
                        changeToDispense.Add(GetCoinFromRegister("Penny"));
                        changeValue -= .01m;
                    }
                }
                if (changeValue == 0)
                {
                    break;
                }
                else if (changeValue != 0 && RegisterHasCoin("Penny") == false)
                {
                    changeToDispense.AddRange(_register);
                    changeToDispense.Clear();
                    return null;
                }
            }
            return changeToDispense;
        }
        //Reusable method to check if the register has a coin of that name.
        //If it does have one, return true.  Else, false.
        private bool RegisterHasCoin(string name)
        {
            foreach (Coin coin in _register)
            {
                if (coin.name == name)
                {
                    return true;
                }
            }
            return false;
        }
        //Reusable method to return a coin from the register.
        //Returns null if no coin can be found of that name.
        private Coin GetCoinFromRegister(string name)
        {
            foreach (Coin coin in _register)
            {
                if (coin.name == name)
                {
                    return coin;
                }
            }
            return null;
            
            
        }
        //Takes in the total payment amount and the price of can to return the change amount.
        private decimal DetermineChange(decimal totalPayment, decimal canPrice)
        {
            decimal changeValue = (totalPayment - canPrice);
            return changeValue;
        }
        //Takes in a list of coins to returnt he total value of the coins as a double.
        private decimal TotalCoinValue(List<Coin> payment)
        {
            decimal totalValue = 0;
            foreach (Coin coin in payment)
            {
                totalValue += coin.Value;
            }
            return totalValue;
        }
        //Puts a list of coins into the soda machines register.
        private void DepositCoinsIntoRegister(List<Coin> coins)
        {
            foreach (Coin coin in coins)
            {
                _register.Add(coin);
            }
        }
        private void ChargeCreditCard(decimal remainingBalance, CreditCard creditCard)
        {
            creditCard.value -= remainingBalance;
            creditCardPayments.Add(remainingBalance);            
        }
        
    }
}
