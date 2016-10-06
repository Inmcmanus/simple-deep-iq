using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace DeepIQ
{
    class DeepIQDriver
    {
        private List<List<TableItem>> Tables = new List<List<TableItem>>();
        private List<TableItem> tokenTable = new List<TableItem>();
        private List<TableItem> spookyTable = new List<TableItem>();
        private int startingLevel = 0;

        static void Main(string[] args)
        {
            DeepIQDriver DeepIQ = new DeepIQDriver();

            DeepIQ.initGame();          
            DeepIQ.Game();
        }


        //Setup the initial game parameters. Currently just asks for table number.
        private void initGame()
        {
            Console.WriteLine("Magic the Gathering: DeepIQ" +
                              "\nA program to test decks against a simple opponent.\n");

            while (startingLevel == 0)
            {
                    Console.WriteLine("\nEnter a starting level (1-6):");

                    if (!Int32.TryParse(Console.ReadLine(), out startingLevel) || startingLevel > 6 || startingLevel < 0)   //A check to make sure the user enters valid input within range
                    {
                        startingLevel = 0;
                    }
            }

            Console.WriteLine("DeepIQ will start the game on Table " + startingLevel);

            //Read in the table data. This reads in each table and stores them as an element in the List 'Tables'.
            //The program can read in tables of any size and any number of them.
            StreamReader tableReader = new StreamReader(File.OpenRead(@"DeepIQTables.csv"));

            tableReader.ReadLine(); //Removes the column headers; they're for user readability in the file
            while (!tableReader.EndOfStream)
            {
                string line = tableReader.ReadLine();
                string[] values = line.Split(',');

                int tableNumber = 0;

                /* There are three kinds of tables. The token table, which contains info about what creature tokens
                 * can look like. The Spooky table, which is a list of random "big" effects. Then are are the standard
                 * rollable tables. There are N of there - it could be anything - and DeepIQ moves through these 
                 * throughout the game.
                 * */
                if (values[0].Equals("t"))
                {
                    tokenTable.Add(new TableItem(null, values[2]));
                }
                else if (values[0].Equals("s"))
                {
                    spookyTable.Add(new TableItem(values[1], values[2]));
                }
                else if (Int32.TryParse(values[0], out tableNumber)) 
                {
                    if (tableNumber > Tables.Count) //This checks if it's going to read in a new table and creates it if it will.
                    {
                        Tables.Add(new List<TableItem>());
                    }

                    Tables[Tables.Count - 1].Add(new TableItem(values[2], values[3]));  //Add the next entry to the current table
                }
                
            }
        }


        //The Game Loop
        private void Game()
        {
            DeepIQGame game = new DeepIQGame(startingLevel, Tables, tokenTable, spookyTable);
            help();
            Console.WriteLine("\nStarting a new game...");
            Console.WriteLine("-------------------------\nTurn 0\n-------------------------");

            bool stop = false;
            while (!stop)
            {
                try
                {
                    string[] input = Console.ReadLine().Split(' ');
                    switch (input[0].ToLower())
                    {
                        case "help":
                            help();
                            break;
                        case "next":
                            game.takeTurn();
                            break;
                        case "remove":
                            Console.WriteLine("Removing token " + input[1]);
                            game.removeCreature(input[1].ToUpper());
                            break;
                        case "display":
                            game.displayBoardstate();
                            break;
                        case "reset":
                            game.resetGame(startingLevel);
                            break;
                        case "difficulty":
                            if (Int32.Parse(input[1]) > 0 && Int32.Parse(input[1]) < 7)
                            {
                                startingLevel = Int32.Parse(input[1]);
                            }
                            break;
                        case "exit":
                            Console.WriteLine("Quitting DeepIQ...");
                            stop = true;
                            break;
                        default:
                            Console.WriteLine("Invalid Command. Type 'help' for a list of commands.");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Invalid Command. Type 'help' for a list of commands.");
                }
            }
        }


        private static void help()
        {
            Console.WriteLine("\nCommand list: " +
                              "\nhelp - Displays this menu " +
                              "\nnext - Advances DeepIQ to the next turn." +
                              "\nremove <token_id> - Removes the specified token from the board. E.g. 'remove C14' " +
                              "\ndisplay - Displays DeepIQ's current boardstate." +
                              "\nreset - Restarts the game at your difficulty set from earlier " +
                              "\ndifficulty <new_difficulty> - Changes DeepIQ's starting level. Does not take effect until you call 'reset' " +
                              "\nexit - Quits the program");
        }
    }
}
