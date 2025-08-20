using System.Linq;
using UnityEngine;

namespace mm
{
    public class RandomizeString : MonoBehaviour
    {
        private static string[] FANTASY_NAMES = new string[50] {"Tharion", "Eryndor", "Arintha", "Kaelin", "Eldrid",
                      "Draven", "Ryker", "Torin", "Lirien", "Galadrielle",
                      "Valoria", "Zephyr", "Sable", "Celestia", "Nyx",
                      "Calantha", "Sorin", "Isadora", "Auriel", "Thalia",
                      "Gwyneth", "Lorien", "Alaric", "Rowan", "Eira",
                      "Darian", "Seraphina", "Evander", "Lysandra", "Halcyon",
                      "Thorne", "Rhiannon", "Kairos", "Sabriel", "Cygnus",
                      "Caius", "Elara", "Orion", "Lyra", "Serenity",
                      "Daedalus", "Lyris", "Vesper", "Aeloria", "Cassius",
                      "Xanthe", "Thetis", "Zarek", "Niamh", "Elwyn" };

        private const string POSSIBLE_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string GetRandomFantasyName()
        {
            return FANTASY_NAMES[Random.Range(0, FANTASY_NAMES.Length)];
        }

        public static string GetRandomString(int length)
        {
            System.Random rand = new System.Random();
            
            return new string(Enumerable.Repeat(POSSIBLE_CHARS, length)
                .Select(s => s[rand.Next(s.Length)]).ToArray());
        }
    }
}