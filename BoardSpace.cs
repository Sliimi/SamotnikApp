using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace SamotnikApp
{
    public class BoardSpace
    {
        public bool IsValid { get; set; }
        public Ellipse? CurrentBall { get; set; }

        public BoardSpace(bool isValid, Ellipse currentBall)
        {
            IsValid = isValid;
            CurrentBall = currentBall;
        }

        public BoardSpace(bool isValid)
        {
            IsValid = isValid;
            CurrentBall = null;
        }
    }
}
