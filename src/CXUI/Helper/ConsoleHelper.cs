using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CXUI.Helper
{
    /// <summary>
    /// Help working with console color
    /// </summary>
    public class ConsoleColorChanger : IDisposable
    {
        private ConsoleColor oldColor;

        /// <summary>
        /// Create color changer and set color
        /// </summary>
        /// <param name="newColor"></param>
        public ConsoleColorChanger(ConsoleColor newColor)
        {
            oldColor = Console.ForegroundColor;
            Console.ForegroundColor = newColor;
        }

        /// <summary>
        /// Reset color
        /// </summary>
        public void Dispose()
        {
            Console.ForegroundColor = oldColor;
        }
    }
}
