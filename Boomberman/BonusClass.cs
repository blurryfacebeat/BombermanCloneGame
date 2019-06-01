using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boomberman
{
    //Список бонусов
    public enum Bonuses
    {
        empty,
        bomb_plus,
        bomb_minus,
        fire_plus,
        fire_minus,
        speed_plus,
        speed_minus
    }

    public class BonusClass
    {
        static Dictionary<Bonuses, int> percent;
        static List<Bonuses> listBonus;
        static Random rand = new Random();
        //Максимум бонусов
        static int countBonuses = 7;

        public static void Prepare()
        {
            PreparePercent();
            PrepareBonuses();
        }

        //Процент выпадания бонусов
        private static void PreparePercent()
        {
            percent = new Dictionary<Bonuses, int>();
            percent.Add(Bonuses.bomb_plus, 50);
            percent.Add(Bonuses.bomb_minus, 20);
            percent.Add(Bonuses.fire_plus, 60);
            percent.Add(Bonuses.fire_minus, 20);
            percent.Add(Bonuses.speed_plus, 40);
            percent.Add(Bonuses.speed_minus, 10);
        }

        private static void PrepareBonuses()
        {
            listBonus = new List<Bonuses>();
            int sum = 0;
            foreach (int item in percent.Values)
            {
                sum += item;
            }
            do
            {
                int numBonus = rand.Next(0, sum);
                int tBonus = 0;
                foreach (Bonuses bonus in percent.Keys)
                {
                    tBonus += percent[bonus];
                    if (numBonus < tBonus)
                    {
                        listBonus.Add(bonus);
                        break;
                    }
                }
            } while (listBonus.Count < countBonuses);
        }

        public static Bonuses GetBonus()
        {
            if (listBonus.Count == 0)
            {
                return Bonuses.empty;
            }
            Bonuses bonus = listBonus[0];
            listBonus.Remove(bonus);
            return bonus;
        }
    }
}
