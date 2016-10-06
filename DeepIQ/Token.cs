using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepIQ
{
    /*There are currently only creature tokens that can be created. If I update the DeepIQ tables to
     * have noncreature tokens, then change this to a parent class for Tokens.
     * */
    class Token
    {
        private static int tokenNumber = 1;  //The number of tokens created so far. Used to assign unique tokenIDs.
        private string tokenID;
        private int power;
        private int toughness;
        List<string> keywords;

        public Token(string tokenInfo)
        {
            tokenID = "C" + tokenNumber;
            tokenNumber++;

            String[] PT = tokenInfo.Split('/');     //The token info is just be P/T
            power = Int32.Parse(PT[0]);
            toughness = Int32.Parse(PT[1]);

            keywords = new List<string>();      //Keywords are added later. They're rolled on another table.

        }

        public void adjustPT(int power, int toughness)
        {
            this.power += power;
            this.toughness += toughness;
        }

        //Adds a keyword, checks for uniqueness.
        public void addKeyword(String keyword)
        {
            if (!keywords.Contains(keyword) && !keyword.Equals("Nothing"))
            {
                keywords.Add(keyword);
            }
        }

        public void removeKeyword(String keyword)
        {
            if (keywords.Contains(keyword))
            {
                keywords.Remove(keyword);
            }
        }

        public string getID()
        {
            return tokenID;
        }

        public int getPower()
        {
            return power;
        }

        public int getToughness()
        {
            return toughness;
        }

        public List<string> getKeywords()
        {
            return keywords;
        }

        //Displays the token in a method somewhat reminiscient of an actual card.
        public void display()
        {
            Console.WriteLine("\nToken: " + tokenID);
            foreach (String s in keywords)
            {
                Console.WriteLine(s);
            }

            Console.WriteLine("\t " + power + "/" + toughness + "\n");
        }

        

    }
}
