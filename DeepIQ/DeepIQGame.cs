using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* This is the class that runs the "Game". That is, a single iteration of the match against DeepIQ. 
 * It handles all it's own functionality in this class, but otherwise lets the driver class 
 * control it; telling it when to take a turn, make a creature and so on. While it isn't implemented
 * this is intended so that you could run multiple different games of magic. Say, a best of three match.
 */
namespace DeepIQ
{
    class DeepIQGame
    {
        private int level;
        private List<List<TableItem>> resultTables;
        private List<TableItem> tokenTable;
        private List<TableItem> spookyTable;
        private List<Token> creatureList;
        private int turn;

        public DeepIQGame(int level, List<List<TableItem>> resultTables, List<TableItem> tokenTable, List<TableItem> spookyTable)
        {
            this.level = level;
            this.resultTables = resultTables;
            this.tokenTable = tokenTable;
            this.spookyTable = spookyTable;
            creatureList = new List<Token>();
            turn = 1;
        }


        public void resetGame(int level)
        {
            this.level = level;
            creatureList = new List<Token>();
            turn = 1;
            Console.WriteLine("\nStarting a new game...");
            Console.WriteLine("-------------------------\nTurn 0\n-------------------------");
        }


        /*This is a standard turn for DeepIQ
         * "Upkeep" - Displaying turn number and misc upkeep business
         * "Main Phase" - Rolling the die for the turn and adding to the board
         * "End Phase" - Display the board state for the end of the turn.
         * */     
        public void takeTurn()
        {
            //Upkeep
            Console.WriteLine("-------------------------\nTurn " + turn + "\n-------------------------");

            //Main Phase
            rollTable(level-1);

            //End Phase
            displayBoardstate();
            turn++;
        }


        //Shows the current boardstate of DeepIQ
        public void displayBoardstate()
        {
            Console.WriteLine("DeepIQ's Current Boardstate:");
            foreach (Token t in creatureList)
            {
                t.display();
            }
        }


        //A generic dice roller in case other rolls come up. Usually you'll call diceRoller(1, 10, n)
        public int diceRoller(int diceNumber, int diceSize, int modifier)
        {
            Random die = new Random();
            int sum = modifier;
            for (int i = 0; i < diceNumber; i++)
            {
                sum += die.Next(1, diceSize+1);
            }
            return sum;
        }


        //Looks for and removes a creature from the board. This should be called manually by the player
        //if they would use one of their cards on DeepIQ's creatures or kill on during combat.
        public void removeCreature(string creatureID)
        {
            foreach (Token t in creatureList)
            {
                if (t.getID().Equals(creatureID))
                {
                    creatureList.Remove(t);
                    break;
                }
            }
        }

        //Rolls on the keyword table and adds it to a relevant creature.
        private Token rollTokenKeywords(int result, Token c)
        {
            //It's possible to roll higher than the number of entries due to modifiers. This checks for that.
            if (result > tokenTable.Count - 1)
            {
               result = tokenTable.Count - 1;
            }
            if (result < 1)
            {
                result = 1;
            }

            List<string> keywordResult = tokenTable[result].getEffects();

            //Change the P/T if need be
            c.adjustPT(Int32.Parse(keywordResult[0].Split('/')[0]), Int32.Parse(keywordResult[0].Split('/')[1]));
            
            //Extract all the keywords and add them one by one
            string[] keywordList = keywordResult[1].Split(' ');

            foreach (string s in keywordList)
            {
                if (s.Equals("Protection")) //Protection has special rules
                {
                    switch (diceRoller(1, 5, 0))
                    {
                        case 1:
                            c.addKeyword("Protection from White");
                            break;
                        case 2:
                            c.addKeyword("Protection from Blue");
                            break;
                        case 3:
                            c.addKeyword("Protection from Black");
                            break;
                        case 4:
                            c.addKeyword("Protection from Red");
                            break;
                        case 5:
                            c.addKeyword("Protection from Green");
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    c.addKeyword(s);
                }
            }
            
            //This checks if it has any bonus rolling to do.
            if (keywordResult.Count > 2)
            {
                for (int i = 2; i < keywordResult.Count; i++)
                {
                    c = rollTokenKeywords(diceRoller(1, 10, 0), c);
                }
            }


            return c;
            
        }


        /*This is what the actual "playing cards" part of DeepIQ's turn is. Rolls a random effect off the
         * current table and adds something to the board or gives the player instructions. It accepts an integer
         * representing the table to roll on as input and automatically processes the resulting roll.
         * */      
        private void rollTable(int tableNumber)
        {
            int tableIndex = diceRoller(1, 10, 0);

            /*The tables of roll results are treated as a multidimensional array. The 'x' axis is the 'level' (1-6)
             * of DeepIQ at the moment. the 'y' axis is the variable result of a roll. Usually it'll be 1-10.
             */
            TableItem rollResult = resultTables[tableNumber][tableIndex - 1];

            //Print the plaintext effect for the player to see
            Console.Write("DeepIQ rolled a " + tableIndex + " - ");
            rollResult.display();
   
            //Then do the actual effect
            List<string> effectList = rollResult.getEffects();

            foreach(string s in effectList) //There can be more than one effect
            {
                
                string[] effect = s.Split('|');

                //This checks for an OR condition in the effect. The browbeat mechanic.
                //It keeps the selected effect and then continues as normal
                if (effect.Length > 1)
                {
                    
                    string choice = "";
                    while (!choice.Equals("a") && !choice.Equals("b"))
                    {
                        
                        Console.Write("Choose option a or b: ");        
                        choice = Console.ReadLine().ToLower();

                    }              
                    
                    if (choice.Equals("a"))
                    {
                        effect = effect[0].Split(' ');                    
                    }
                    else
                    {
                        effect = effect[1].Split(' ');
                    }
                }
                else
                {
                    effect = s.Split(' ');
                }

                //The first element is what the effect actually does. Subsequent elements denote the details of it.
                //Things like "Destroy a creature", with notes of what it targets.
                switch (effect[0])
                {
                    case "Create":
                        Token c = new Token(effect[1]);
                        //Roll on the token table for bonuses
                        rollTokenKeywords(diceRoller(1, 10, Int32.Parse(effect[2])), c);

                        creatureList.Add(c);
                        
                        break;
                    case "Sacrifice":
                        //Deep IQ can't do anything here. It's up to the player.
                        break;
                    case "Exile":
                        //Deep IQ can't do anything here. It's up to the player.
                        break;
                    case "Advance":
                        if (level < resultTables.Count)
                        {
                            level++;
                            Console.WriteLine("DeepIQ advanced to table " + level);
                        }                    
                        break;
                    case "Roll":
                        rollTable(Int32.Parse(effect[1])-1);
                        break;
                    case "Damage":
                        //Deep IQ can't do anything here. It's up to the player.
                        break;
                    default:
                        break;
                }

                

            }        
        }

    }
}
