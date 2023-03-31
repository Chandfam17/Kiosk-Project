using System.Diagnostics;
using System.Globalization;
using System.Transactions;

//create variables needed for program
decimal totalCost = 0;
string userImput = "";
decimal userMoney = 0;
decimal totalMoney = 0;
decimal change = 0.0m;
decimal changeLeft = 0.0m;
bool checkMoney = true;
string payment = "";
string creditCard = "";
string cardType = "";
bool isCreditCard = false;
string account = "1234567890";
string iWantCash = "";
decimal cashback = 0m;
string[] getMoney = new string[2];
bool flag = true;
string[] strings = new string[7];
string transNum = "";
decimal getChange = 0;


//create new branch of Bank called kiosk
Bank kiosk;
kiosk.currencyValue = new decimal[] { .01m, .05m, .10m, .25m, .50m, 1, 2, 5, 10, 20, 50, 100 }; //set values
kiosk.currencyAmount = new decimal[12]; //create twelve spots for amounts for each values

//use for loop to assign 20 of each value to kiosk
for (int i = 0; i < kiosk.currencyAmount.Length; i++)
{
    kiosk.currencyAmount[i] = 20;
}

Console.WriteLine("\nWelcome valued customer!\n");

//create a do loop that runs until the user does not want to enter any more items
do
{
    //use variable to calculate and store item totals as the user enters them
    totalCost += GetUserImput();

    //ask user if they have another item, then store user answer in variable
    Console.Write("Do you have another item? (y/n): ");
    userImput = Console.ReadLine().Trim().ToLower();

    //create an if statement for user imput in case user entered wrong key
    if (userImput != "y" && userImput != "n")
    {
        Console.Write("Invalid imput. Please enter (y/n):");
        userImput = Console.ReadLine();
    }

} while (userImput == "y" || userImput == "Y"); //while these statements are true, loop will run


Console.WriteLine("Total cost: $" + totalCost); //display total cost

//prompt user to enter preferred payment method, use do loop as user validation
do
{
    Console.Write("\nWould you like to use card or cash? (Enter card/cash): ");
    payment = Console.ReadLine().Trim().ToLower();

} while (payment != "card" && payment != "cash");

//if they choose cash, send info for transaction logging and call cash function
if (payment == "cash")
{
    decimal cost = totalCost;
    decimal totalChange = cashback;

    getChange = CashOption(kiosk.currencyValue, kiosk.currencyAmount, totalCost, cashback);
    transNum = RandomNum();
    totalChange += getChange;
    Logging(transNum, cost, "none", 0, totalChange);
}

//if they choose card, the following will execute
else if (payment == "card")
{
    //create int to count so that the program doesn't ask the user if they want cash every time the loop runs
    int i = 0;
    decimal total = totalCost;

    do
    {
        //call the card option to get, identify, and validate the card
        cardType = CardOption(creditCard);
        Console.WriteLine("Card Type: " + cardType);

        //if this is the first time the loop has run, program will ask user if they want cash back. Do loop will ensure user validation
        if (i == 0)
        {
            do
            {
                Console.Write("\nWould you like cash back? (y/n): ");
                iWantCash = Console.ReadLine().Trim().ToLower();

            } while (iWantCash != "n" && iWantCash != "y");
        }

        //ADD INPUT VALIDATION LATER
        //if the user wants cash back, theses statements will execute
        if (i == 0 && iWantCash == "y")
        {
            string cashback1 = "";
            bool check = true;
            do
            {
                Console.Write("\nHow much cash would you like back? $");
                cashback1 = Console.ReadLine();


                for (int j = 0; j < cashback1.Length; j++)
                {
                    if (cashback1[i] != '1' && cashback1[i] != '2' && cashback1[i] != '3' && cashback1[i] != '4' && cashback1[i] != '5' && cashback1[i] != '6' && cashback1[i] != '7' && cashback1[i] != '8' && cashback1[i] != '9' && cashback1[i] != '0')
                    {
                        check = false;
                        break;
                    }
                    else
                    {
                        check = true;
                    }
                }

            } while (check == false);

            cashback = Convert.ToDecimal(cashback1);
        }
        //add one to counter so it doesn't ask cash back statements again
        i++;


        //let user know the program has called our money request function for card
        Console.WriteLine("\nRequesting Funds...");
        getMoney = MoneyRequest(account, totalCost);

        //if the card is declined, the following will execute
        if (getMoney[1] == "declined")
        {
            Console.WriteLine("\nAccount status: {0}", getMoney[1]);

            //ask the user to enter a different payment method
            payment = ChangePayment(payment);

            //if they enter another card, the loop will run again
            if (payment == "y")
            {
                flag = true;
            }
            //if they enter cash, call transaction logging and cash function, then exit loop
            else if (payment == "n")
            {
                decimal totalChange = cashback;
                getChange = CashOption(kiosk.currencyValue, kiosk.currencyAmount, totalCost, cashback);
                transNum = RandomNum();
                totalChange += getChange;
                Logging(transNum, totalCost, "none", 0, totalChange);
                break;
            }

        }
        //if the card is NOT declined, the following will execute
        else
        {
            //convert money accepted to a decimal value
            decimal getMoneyVar = Convert.ToDecimal(getMoney[1]);
            //getMoneyVar = Math.Round(getMoneyVar);

            //if the amount accepted is less than the total cost, the following will execute
            if (getMoneyVar < totalCost)
            {
                //create a decimal store money left and display totals to user
                decimal moneyLeft = totalCost - getMoneyVar;
                Console.WriteLine("\nAmount accepted: $" + getMoney[1]);
                Console.WriteLine("\nYou owe ${0}.", moneyLeft);

                moneyLeft = totalCost - getMoneyVar;
                //create while loop that will run until total cost is paid
                while (moneyLeft > 0)
                {
                    //call ChangePayment() function and ask user for a different payment method
                    payment = ChangePayment(payment);

                    //if they use a different card, the following will execute
                    if (payment == "y")
                    {
                        //call card function
                        cardType = CardOption(creditCard);
                        Console.WriteLine("Card Type: " + cardType);

                        //call money request to get more funds
                        getMoney = MoneyRequest(account, moneyLeft);

                        //if card gets declined, program will display message and loop will reiterate
                        if (getMoney[1] == "declined")
                        {
                            Console.WriteLine("Account declined.");
                        }
                        //if card isn't declined, calculate and display amount accepted and amount still owed
                        else
                        {
                            getMoneyVar = Convert.ToDecimal(getMoney[1]);

                            Console.WriteLine("\nAmount accepted: $" + getMoneyVar);
                            moneyLeft -= getMoneyVar;
                            Console.WriteLine("\nYou owe: $" + moneyLeft);

                        }

                        if (moneyLeft <= 0)
                        {
                            transNum = RandomNum();

                            Logging(transNum, 0, cardType, total, cashback);
                        }

                        //if the user wanted cash back, use greedy algorithm to dispense change
                        if (iWantCash == "y")
                        {
                            //once all money has been paid, the following will execute
                            if (moneyLeft <= 0)
                            {
                                //call function and assign it to bool variable to check if kiosk has enough money
                                checkMoney = CheckRegister2(cashback, kiosk.currencyValue, kiosk.currencyAmount);

                                //if register doesn't have enough money
                                if (checkMoney == false)
                                {
                                    Console.WriteLine("Kiosk does not have enough physical money to supply change. Please speak to the manager.");
                                }
                                //if register has enough money
                                else
                                {
                                    changeLeft = cashback;
                                    Console.WriteLine("Dispensing cash back...\n");
                                    do
                                    {

                                        cashback = GreedyAlgorithm(kiosk.currencyValue, kiosk.currencyAmount, changeLeft);
                                        Console.WriteLine("Dispensing ${0}...", cashback);

                                        changeLeft = changeLeft - cashback;

                                    } while (changeLeft > 0.01m);
                                }
                            }
                        }


                    }


                    //if the user opted to pay with cash, call cash function, then end program
                    else if (payment == "n")
                    {
                        decimal cost = moneyLeft;
                        decimal totalChange = cashback;

                        total -= moneyLeft;
                        transNum = RandomNum();
                        getChange = CashOption(kiosk.currencyValue, kiosk.currencyAmount, moneyLeft, cashback);
                        totalChange += getChange;
                        Logging(transNum, cost, cardType, total, totalChange);

                        break;
                    }
                }
                break;

            }
            //if the card was NOT declined and will accept the total cost, the following will execute
            else
            {
                //display amount and message to user
                Console.WriteLine("\nAmount accepted: $" + getMoney[1]);
                Console.WriteLine("\nThank you for your payment!\n");


                transNum = RandomNum();
                Logging(transNum, 0, cardType, total, cashback);

                //if the user opted for cash back, the following will execute
                if (iWantCash == "y")
                {
                    //check to ensure register has enough money
                    checkMoney = CheckRegister2(cashback, kiosk.currencyValue, kiosk.currencyAmount);

                    //if register doesn't have enough money, display message and end program
                    if (checkMoney == false)
                    {
                        Console.WriteLine("Kiosk does not have enough physical money to supply change. Please speak to the manager.");
                        break;
                    }
                    //if register does have enough money, the following will execute
                    else
                    {
                        changeLeft = cashback;
                        Console.WriteLine("Dispensing cash back...\n");
                        do
                        {
                            cashback = GreedyAlgorithm(kiosk.currencyValue, kiosk.currencyAmount, changeLeft);
                            Console.WriteLine("Dispensing ${0}...", cashback);

                            changeLeft = changeLeft - cashback;

                        } while (changeLeft > 0.01m);
                    }
                }
                break;
            }
        }


    } while (flag == true);

}

//END MAIN
//CLASS STARTS

static decimal CashOption(decimal[] value, decimal[] amount, decimal totalCost, decimal cashback)
{

    decimal userMoney = 0;
    decimal totalMoney = 0;
    decimal change = 0.0m;
    decimal changeLeft = 0.0m;
    bool checkMoney = true;


    //create do loop that runs until the user has entered money
    //equal to or over money needed
    do
    {
        //prompt user to enter a bill and store in variable
        Console.Write("\nPlease enter a bill or coin: $");
        userMoney = decimal.Parse(Console.ReadLine());

        //use if statements to ensure user entered correct coin and/or dollar amount
        if (userMoney == .01m || userMoney == .05m || userMoney == .10m || userMoney == .25m || userMoney == .50m || userMoney == 1m || userMoney == 2m || userMoney == 5m || userMoney == 10m || userMoney == 20m || userMoney == 50m || userMoney == 100m)
        {
            //use another variable to add up user's money each time loop reiterates
            totalMoney += userMoney;

            //if the user has not entered enough money to cover cost,
            //program will display a message showing what is still owed
            if (totalMoney < totalCost)
            {
                Console.WriteLine("Money needed: $" + (totalCost - totalMoney));
            }
        }


    } while (totalMoney < totalCost); //end do loop




    change = totalMoney - totalCost;  //calculate change needed and store in variable
    decimal getChange = change;
    changeLeft = change; //assign another variable the value of change

    //display statements showing change needed
    Console.WriteLine("\nThank you for your payment!");
    Console.WriteLine("\nChange needed: $" + change + "\n");

    //assign a variable to the function CheckRegister to ensure kiosk
    //has enough physical money to despense change needed
    checkMoney = CheckRegister2(change, value, amount);

    //if there is not enough money, display message and end program
    if (checkMoney == false)
    {
        Console.WriteLine("Kiosk does not have enough money to supply change. Please speak to the manager.");
    }
    //if there is enough money, the following will execute
    else
    {
        //create while loop to run until change needed is less than or equal to one cent
        while (change >= 0.01m)
        {
            //assign variable to GreedyAlgorithm function
            changeLeft = GreedyAlgorithm(value, amount, change);

            //display dispensed money
            Console.WriteLine("Dispensing ${0}...", changeLeft);

            //subtract money dispensed from change left
            change = change - changeLeft;
        }

        //if the user requested cash back, the following will execute
        if (cashback != 0)
        {
            Console.WriteLine("\nDispensing Cash Back...\n");
            while (cashback >= 0.01m)
            {
                //assign variable to GreedyAlgorithm function
                changeLeft = GreedyAlgorithm(value, amount, cashback);

                //display dispensed money
                Console.WriteLine("Dispensing ${0}...", changeLeft);

                //subtract money dispensed from change left
                cashback = cashback - changeLeft;
            }
        }


        //use for loop to display money left in kiosk
        Console.WriteLine("\n\n\tKiosk Bank");
        Console.WriteLine("\nMoney Value\tAmount Left");
        Console.WriteLine("------------------------");

        for (int j = 0; j < amount.Length; j++)
        {
            Console.WriteLine("{0}\t\t{1}", value[j], amount[j]);
        }


    }

    return getChange;
}


static string CardOption(string creditCard)
{
    //create variables
    bool isCreditCard = false;
    string cardType = "";

    //get card number using function, then validate card using the luhn algorithm function
    //create do loop that will run until card number is successfully retrieved, validated, and identified
    do
    {
        creditCard = GetCard();

        isCreditCard = CheckLuhn(creditCard);

        if (isCreditCard == true)
        {
            cardType = IdentifyCard(creditCard);
        }

    } while (isCreditCard == false || cardType == "null");

    //return card
    return cardType;
}



static string GetCard()
{
    //prompt user to enter card number
    Console.Write("\nPlease enter your card number: ");
    string card = Console.ReadLine();

    //create loop for user validation
    while (card.Length > 16 && card.Length < 12)
    {
        Console.Write("\nEnter a valid card number: ");
        card = Console.ReadLine();
    }

    return card;
}

static string[] MoneyRequest(string accountNum, decimal amount)
{
    Random rnd = new Random();
    //50% CHANCE TRANSACTION PASSES OR FAILS
    bool pass = rnd.Next(100) < 50;
    //50% CHANCE THAT TRANSACTION IS DECLINED
    bool declined = rnd.Next(100) < 50;

    if (pass)
    {
        return new string[] { accountNum, amount.ToString() };
    }
    else
    {
        if (!declined)
        {
            return new string[] { accountNum, (amount / rnd.Next(2, 6)).ToString() };
        }
        else
        {
            return new string[] { accountNum, "declined" };
        }
    }
}

static string ChangePayment(string payment)
{
    do
    {
        Console.Write("\nEnter 'y' if you would like to pay with a different card, or 'n' if you want to pay with cash. ");
        payment = Console.ReadLine().Trim().ToLower();

    } while (payment != "y" && payment != "n");

    return payment;
}

static string IdentifyCard(string card)
{
    //convert string to char array, then convert the chars to an int array
    char[] cardChars = card.ToCharArray();

    int[] intA = Array.ConvertAll(cardChars, c => (int)Char.GetNumericValue(c));

    string cardType = "";

    //check the first array element to identify card vendor
    if (intA[0] == 3)
    {
        cardType = "AmericanExpress";
    }
    else if (intA[0] == 4)
    {
        cardType = "Visa";
    }
    else if (intA[0] == 5)
    {
        cardType = "MasterCard";
    }
    else if (intA[0] == 6)
    {
        cardType = "Discovery";
    }
    else
    {
        Console.WriteLine("This kiosk only accepts American Express, Visa, MasterCard, or Discovery.\n");
        cardType = "null";
    }
    return cardType;
}


static string RandomNum()
{
    Random rnd = new Random();
    string[] number = new string[10];
    string num = "";

    for (int i = 0; i < 10; i++)
    {
        number[i] = rnd.Next(0, 10).ToString();
        num += number[i];
    }

    return num;
}

static void Logging(string transNum, decimal cash, string card, decimal cardAmt, decimal change)
{

    ProcessStartInfo startInfo = new ProcessStartInfo();
    startInfo.FileName = "C:\\Users\\coffe\\source\\repos\\Transaction Logging\\Transaction Logging\\bin\\Debug\\net6.0\\Transaction Logging.exe";
startInfo.Arguments = transNum + " " + cash.ToString() + " " + cardAmt.ToString() + " " + card + " " + change.ToString();
    Process.Start(startInfo);
}

// Returns true if given
// card number is valid
static bool CheckLuhn(String cardNo)
{
    int nDigits = cardNo.Length;

    int nSum = 0;
    bool isSecond = false;
    for (int i = nDigits - 1; i >= 0; i--)
    {

        int d = cardNo[i] - '0';

        if (isSecond == true)
            d = d * 2;

        // We add two digits to handle
        // cases that make two digits
        // after doubling
        nSum += d / 10;
        nSum += d % 10;

        isSecond = !isSecond;
    }
    return (nSum % 10 == 0);
}

//create function that asks for and returns item cost
static decimal GetUserImput()
{
    bool flag = false;
    string item = "";


    do
    {
        Console.Write("\nEnter item cost: $");
        item = Console.ReadLine();

        for (int i = 0; i < item.Length; i++)
        {
            if (item[i] != '1' && item[i] != '2' && item[i] != '3' && item[i] != '4' && item[i] != '5' && item[i] != '6' && item[i] != '7' && item[i] != '8' && item[i] != '9' && item[i] != '0')
            {
                flag = false;
                break;
            }
            else
            {
                flag = true;
            }
        }

    } while (flag == false);

    decimal item1 = Convert.ToDecimal(item);

    return item1;
}


static bool CheckRegister(decimal[] bills, bool check)
{
    check = true;

    if (bills[0] == 0)
    {
        check = false;
    }
    else if (bills[1] == 0)
    {
        check = false;
    }
    else if (bills[2] == 0)
    {
        check = false;
    }
    else if (bills[3] == 0)
    {
        check = false;
    }
    else if (bills[4] == 0)
    {
        check = false;
    }
    else if (bills[5] == 0)
    {
        check = false;
    }
    else if (bills[6] == 0)
    {
        check = false;
    }
    else if (bills[7] == 0)
    {
        check = false;
    }
    else if (bills[8] == 0)
    {
        check = false;
    }
    else if (bills[9] == 0)
    {
        check = false;
    }
    else if (bills[10] == 0)
    {
        check = false;
    }
    else if (bills[11] == 0)
    {
        check = false;
    }
    return check;
}

//create function that makes sure each spot in the kiosk Bank isn't out of money and returns a bool
static bool CheckRegister2(decimal cost, decimal[] bills, decimal[] amount)
{
    //HOW TO FIX: WE ARE DEALING WITH VALUES, NOT BILLS! COPY BILLS NEEDS TO BE 20, NOT ACTUAL CURRENCY VALUES

    //create variables
    bool check = true;
    decimal owed = cost;
    decimal[] copyBills = new decimal[12];
    int pos = 0;

    ////copying bills into fake bills array
    for (int i = 0; i < copyBills.Length; i++)
    {
        copyBills[i] = amount[i];
    }

    ////STILL NEEDS FIXED. GOT TO FIND A WAY TO SUBTRACT MORE THAN A PENNY AT A TIME
    ////PERHAPS USE POS INSTEAD?
    ////USE SYSTEM I USED FOR GREEDY ALGORITHM INSTEAD

    ////subtract fake money from fake amount owed to check if kiosk has enough money
    //for (int j = 0; owed > 0; j++)
    //{
    //    if (owed >= bills[j] && copyBills[j] > 0)
    //    {
    //        copyBills[j]--;
    //        owed -= bills[j];
    //        pos = j;
    //    }
    //    else if (owed > 0 && copyBills[j] == 0)
    //    {
    //        check = false;
    //        break;
    //    }
    //}

    do
    {
        for (int i = 0; i < copyBills.Length; i++)
        {
            if (owed >= bills[i] && copyBills[i] > 0)
            {
                pos = i;
                check = true;
            }
            else if (owed > 0 && copyBills[i] == 0)
            {
                check = false;
                break;
            }
        }
        copyBills[pos]--;
        owed -= bills[pos];

    } while (owed > 0);

    return check;
}



//create function that goes through each spot in the kiosk array and subtracts one from whichever coin/bill the algorithm dispensed and returns an array of decimals
static decimal[] Register(decimal[] bills, int pos)
{
    if (pos == 0)
    {
        bills[0]--;
    }
    else if (pos == 1)
    {
        bills[1]--;
    }
    else if (pos == 2)
    {
        bills[2]--;
    }
    else if (pos == 3)
    {
        bills[3]--;
    }
    else if (pos == 4)
    {
        bills[4]--;
    }
    else if (pos == 5)
    {
        bills[5]--;
    }
    else if (pos == 6)
    {
        bills[6]--;
    }
    else if (pos == 7)
    {
        bills[7]--;
    }
    else if (pos == 8)
    {
        bills[8]--;
    }
    else if (pos == 9)
    {
        bills[9]--;
    }
    else if (pos == 10)
    {
        bills[10]--;
    }
    else if (pos == 11)
    {
        bills[11]--;
    }

    return bills;
}

//Create a greedy algorithm function using a for loop. Inside of the for loop, the program will traverse “Bank”
//until it finds the highest, or closest value to the amount of change needed and returns that amount
static decimal GreedyAlgorithm(decimal[] currency, decimal[] bills, decimal changeNeeded)
{
    decimal returnChange = 0.0m;
    int pos = 0;

    for (int i = 0; i < currency.Length; i++)
    {
        if (currency[i] <= changeNeeded)
        {
            returnChange = currency[i];
            pos = i;
        }
    }

    Register(bills, pos);
    return returnChange;
}

//create user defined datatype called Bank and two arrays to hold and assign money values
struct Bank
{
    public decimal[] currencyValue;
    public decimal[] currencyAmount;
}






