using UnityEngine;
using System.Collections;
using System.Text;
using MoreMountains.Tools;
using UnityEngine.UI;

namespace MoreMountains.TopDownEngine
{
    [AddComponentMenu("TopDown Engine/GUI/SuckDisplay")]
    public class SuckDisplay : MMProgressBar
    {
        /// the ID of the AmmoDisplay 
		[Tooltip("the ID of the AmmoDisplay ")]
        public int AmmoDisplayID = 0;
        /// the Text object used to display the current ammo numbers
        [Tooltip("the Text object used to display the current ammo numbers")]
        public Text TextDisplay;

        /// <summary>
        /// On init we initialize our string builder
        /// </summary>
        public override void Initialization()
        {
            base.Initialization();

        }

        /// <summary>
        /// Updates the text display with the parameter string
        /// </summary>
        /// <param name="newText">New text.</param>
        public virtual void UpdateTextDisplay(string newText)
        {
            if (TextDisplay != null)
            {
                TextDisplay.text = newText;
            }
        }

        public virtual void UpdateSuckDisplays(float value)
        {
            UpdateBar(value, 0f, 1f);
        }
    }
}

