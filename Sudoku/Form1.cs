using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    public partial class Form1 : Form
    {
        Field field;
        public Form1()
        {
            InitializeComponent();

            field = new Field(this);
            NewGame();
        }

        private void NewGame()
        {
            field.GenerateNumbers();
        }

        public bool TrySetNumber(int number, int x, int y)
        {
            return field.TrySetNumber(number, x, y);
        }

        public bool TrySetNewNumber(ref int number, int x, int y)
        {
            for (number = number + 1; number < 10; number++)
            {
                if (TrySetNumber(number, x, y))
                {
                    return true;
                }
            }

            return false;
        }

        public void UseBackTraking()
        {
            Point[] cellPoint = new Point[field.CellLeft];
            int[] moves = new int[field.CellLeft];
            int index = 0;

            for (int y = 0; y < Field.GridSize.Height; y++)
            {
                for (int x = 0; x < Field.GridSize.Width; x++)
                {
                    if (!field.GetCellIsUsed(x, y))
                    {
                        cellPoint[index] = new Point(x, y);
                        index++;
                    }
                }
            }

            for (int i = 0; i < cellPoint.Length; i++)
            {
                Point point = cellPoint[i];
                if (!TrySetNewNumber(ref moves[i], point.X, point.Y))
                {
                    field.ClearNumber(point.X, point.Y);
                    moves[i] = 0;
                    i-=2;
                    if (i == -2)
                    {
                        break;
                    }
                }
                this.Update();
                Thread.Sleep(50);
            }
        }
    }
}
