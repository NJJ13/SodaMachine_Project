using System;
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

        //Constructor (Spawner)
        public SodaMachine()
        {
            _register = new List<Coin>();
            _inventory = new List<Can>();
            FillInventory();
            FillRegister();
            
        }

        //Member Methods (Can Do)

        //A method to fill the sodamachines register with coin objects.
        public void FillRegister()
        {
            while (_register.Count < 10)
            {
                Coin dime = new Dime();
                _register.Add(dime);
            }
            while (_register.Count < 30)
            {
                Coin nickel = new Nickel();
                _register.Add(nickel);
            }
            while (_register.Count < 50)
            {
                Coin quarter = new Quarter();
                _register.Add(quarter);
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
            string validCustomerCoins = UserInterface.CoinSelection(selectedCan, customer.Wallet.Coins);
        }
        //Gets a soda from the inventory based on the name of the soda.
        private Can GetSodaFromInventory(string nameOfSoda)
        {
            foreach (Can can in _inventory)
            {
                if(nameOfSoda == can.Name)
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
            double valueOfPayment = TotalCoinValue(payment);
            if (valueOfPayment < chosenSoda.Price)
            {
                customer.AddCoinsIntoWallet(payment);
                UserInterface.DisplayError("Transaction can not be completed. There was not enough money deposited in order to purchase " + chosenSoda.Name + ".");
            }
            else if (valueOfPayment == chosenSoda.Price)
            {
                customer.AddCanToBackpack(chosenSoda);
                UserInterface.EndMessage(chosenSoda.Name, 0);
            }
            else if (valueOfPayment > chosenSoda.Price)
            {
                double returnValue = (valueOfPayment - chosenSoda.Price);
                List<Coin> returnedChange = GatherChange(returnValue);
                if (returnedChange == null)
                {
                    customer.AddCoinsIntoWallet(payment);
                    UserInterface.DisplayError("Transaction can not be completed. Soda machine does not have enough change to complete the transaction.");
                }
                else
                {
                    customer.AddCanToBackpack(chosenSoda);
                    customer.AddCoinsIntoWallet(returnedChange);
                    UserInterface.EndMessage(chosenSoda.Name, returnValue);
                }
            }
        }
        //Takes in the value of the amount of change needed.
        //Attempts to gather all the required coins from the sodamachine's register to make change.
        //Returns the list of coins as change to despense.
        //If the change cannot be made, return null.
        private List<Coin> GatherChange(double changeValue)
        {
            List<Coin> changeToDispense = new List<Coin>();
            do
            {
                if (changeValue >= .25)
                {
                    if (RegisterHasCoin("Quarter"))
                    {
                        changeToDispense.Add(GetCoinFromRegister("Quarter"));
                        changeValue -= .25;
                    }
                    else if (RegisterHasCoin("Dime"))
                    {
                        changeToDispense.Add(GetCoinFromRegister("Dime"));
                        changeValue -= .10;
                    }
                    else if (RegisterHasCoin("Nickel"))
                    {
                        changeToDispense.Add(GetCoinFromRegister("Nickel"));
                        changeValue -= .05;
                    }
                    else if (RegisterHasCoin("Penny"))
                    {
                        changeToDispense.Add(GetCoinFromRegister("Penny"));
                        changeValue -= .01;
                    }
                }
                else if (changeValue >= .10)
                {
                    if (RegisterHasCoin("Dime"))
                    {
                        changeToDispense.Add(GetCoinFromRegister("Dime"));
                        changeValue -= .10;
                    }
                    else if (RegisterHasCoin("Nickel"))
                    {
                        changeToDispense.Add(GetCoinFromRegister("Nickel"));
                        changeValue -= .05;
                    }
                    else if (RegisterHasCoin("Penny"))
                    {
                        changeToDispense.Add(GetCoinFromRegister("Penny"));
                        changeValue -= .01;
                    }
                }
                else if (changeValue >= .05)
                {
                    if (RegisterHasCoin("Nickel"))
                    {
                        changeToDispense.Add(GetCoinFromRegister("Nickel"));
                        changeValue -= .05;
                    }
                    else if (RegisterHasCoin("Penny"))
                    {
                        changeToDispense.Add(GetCoinFromRegister("Penny"));
                        changeValue -= .01;
                    }
                }
                else if (changeValue >= .01)
                {
                    if (RegisterHasCoin("Penny"))
                    {
                        changeToDispense.Add(GetCoinFromRegister("Penny"));
                        changeValue -= .01;
                    }
                }
            } while (changeValue != 0);

            return changeToDispense;
        }
        //Reusable method to check if the register has a coin of that name.
        //If it does have one, return true.  Else, false.
        private bool RegisterHasCoin(string name)
        {
            foreach (Coin coin in _register)
            {
                if (coin.Name == name)
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
                if (coin.Name == name)
                {
                    return coin;
                }
            }
            return null;
            
            
        }
        //Takes in the total payment amount and the price of can to return the change amount.
        private double DetermineChange(double totalPayment, double canPrice)
        {
            double changeValue = (totalPayment - canPrice);
            return changeValue;
        }
        //Takes in a list of coins to returnt he total value of the coins as a double.
        private double TotalCoinValue(List<Coin> payment)
        {
            double totalValue = 0;
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
        
    }
}
