using UnityEngine;
using UnityEngine.UI;

namespace BusSimulator
{
    // Shows the gold "selected" frame on the level that is currently chosen.
    public class LevelCardHighlight : MonoBehaviour
    {
        public Image[] cards;
        public Sprite normal;
        public Sprite selected;

        private void Update()
        {
            int sel = PlayerPrefs.GetInt("levelno", 1);
            for (int i = 0; i < cards.Length; i++)
            {
                Sprite s = (i + 1 == sel) ? selected : normal;
                if (cards[i].sprite != s) cards[i].sprite = s;
            }
        }
    }
}
