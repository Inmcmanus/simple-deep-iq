using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//This class is simply an object representing an entry in the rollable tables.

namespace DeepIQ
{
    class TableItem
    {
        private string description; //The plaintext description for the user
        private List<string> effectList;   //The list of effects in a format for the program to parse

        public TableItem(string description, string effectLine)
        {
            this.description = description;
            effectList = new List<string>();

            String[] effects = effectLine.Split(':');
            foreach (string s in effects)
            {
                effectList.Add(s);
            }
        }

        public string getDescription()
        {
            return description;
        }

        public List<string> getEffects()
        {
            return effectList;
        }

        public void display()
        {
            Console.WriteLine(description);
        }
    }
}
