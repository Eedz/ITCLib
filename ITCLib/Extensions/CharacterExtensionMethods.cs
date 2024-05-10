using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public static class CharacterExtensionMethods
    {
        /// <summary>
        /// Returns true if 'ch' is a Hebrew character.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsHebrew(this char ch)
        {
            if ((ch >= '\u0580' && ch <= '\u05ff') || (ch >= '\ufb1d' && ch <= '\ufb4f')) return true;
            return false;

            
        }

        /// <summary>
        /// Returns true if 'ch' is an Arabic character.
        /// </summary>
        /// <param name="ch"></param>
        /// <returns></returns>
        public static bool IsArabic(char ch)
        {
            if (ch >= '\u0627' && ch <= '\u0649') return true;
            return false;
        }
    }
}
