using UnityEngine;
using UnityEngine.UI;

namespace BusSimulator
{
    // Shows the gold "selected" card frame on whichever mode is currently chosen.
    public class ModeCardHighlight : MonoBehaviour
    {
        public Image freeImage;
        public Image careerImage;
        public Sprite freeNormal;
        public Sprite freeSelected;
        public Sprite careerNormal;
        public Sprite careerSelected;

        private void Update()
        {
            bool free = ModeSelectionController.FreeMoode;
            Sprite f = free ? freeSelected : freeNormal;
            Sprite c = free ? careerNormal : careerSelected;
            if (freeImage.sprite != f) freeImage.sprite = f;
            if (careerImage.sprite != c) careerImage.sprite = c;
        }
    }
}
