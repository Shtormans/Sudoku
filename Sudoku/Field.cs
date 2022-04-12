using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku
{
    class Field
    {
        public static readonly Size GridSize = new Size(9, 9);
        public static readonly Size BlockSize = new Size(3, 3);
        public readonly int CellCount = GridSize.Width * GridSize.Height;
        private int _cellLeft = GridSize.Width * GridSize.Height;

        private readonly Cell[,] _field;
        private readonly Block[,] _blocks;
        private readonly Form1 _form;
        private Random _random = new Random();

        public int CellLeft
        {
            get { return _cellLeft; }
        }

        public Field(Form1 form)
        {
            _form = form;
            _field = new Cell[GridSize.Height, GridSize.Width];
            _blocks = new Block[BlockSize.Height, BlockSize.Width];

            CreateBlocks();
            CreateField();
        }

        public bool GetCellIsUsed(int x, int y)
        {
            return _field[y, x].IsUsed;
        }

        public int GetCellNumber(int x, int y)
        {
            return _field[y, x].Number;
        }

        public bool TrySetNumber(int number, int x, int y)
        {
            if (!CheckNumber(number, x, y))
            {
                _field[y, x].SetIncorrectNumber(number);
                return false;
            }

            _field[y, x].SetCorrectNumber(number);

            if (_field[y, x].Number != number)
            {
                _cellLeft--;
            }
            return true;
        }

        public void ClearNumber(int x, int y)
        {
            if (_field[y, x].Number == 0)
            {
                _cellLeft++;
            }

            _field[y, x].ClearNumber();
        }

        private Panel CreatePanel(Point location)
        {
            Panel panel = new Panel();
            panel.Size = new Size(Cell.Size.Width * 3, Cell.Size.Height * 3);
            panel.Location = location;
            panel.BorderStyle = BorderStyle.Fixed3D;
            _form.Controls.Add(panel);

            return panel;
        }

        private void CreateBlocks()
        {
            for (int y = 0; y < BlockSize.Height; y++)
            {
                for (int x = 0; x < BlockSize.Width; x++)
                {
                    Point blockLocation = new Point(x * Block.Size.Width, y * Block.Size.Height);
                    _blocks[y, x] = new Block(blockLocation);

                    _blocks[y, x].Show(_form);
                }
            }
        }

        private void CreateField()
        {
            for (int y = 0; y < GridSize.Height; y++)
            {
                for (int x = 0; x < GridSize.Width; x++)
                {
                    Point cellLocation = new Point(Cell.Size.Width * x % Block.Size.Width, Cell.Size.Height * y % Block.Size.Height);
                    Point globalLocation = new Point(x, y);
                    Cell cell = new Cell(globalLocation, cellLocation, _form);

                    _blocks[y / 3, x / 3].SetCell(cell);

                    _field[y, x] = cell;
                }
            }
        }

        private bool TryCreateNumber(int x, int y)
        {
            int number = _field[y, x].Number;

            for (number = number + 1; number < 10; number++)
            {
                if (TrySetNumber(number, x, y))
                {
                    return true;
                }
            }

            return false;
        }

        private void GenerateNumbersRandomly()
        {
            for (int i = 0; i < CellCount; i++)
            {
                Point point = new Point(i / GridSize.Width, i % GridSize.Width);

                if (!TryCreateNumber(point.X, point.Y))
                {
                    _field[point.Y, point.X].ClearNumber();

                    i -= 2;
                    if (i == -2)
                    {
                        break;
                    }
                }
            }

            for (int y = 0; y < GridSize.Height; y++)
            {
                for (int x = 0; x < GridSize.Width; x++)
                {

                    if (_random.Next(0, 2) == 0)
                    {
                        _field[y, x].ClearNumber();
                        continue;
                    }

                    int number = _field[y, x].Number;
                    _field[y, x].SetStartNumber(number);

                    _cellLeft--;
                }
            }
        }

        private void GenerateNumbersRandomlyOld()
        {
            for (int y = 0; y < GridSize.Height; y++)
            {
                for (int x = 0; x < GridSize.Width; x++)
                {
                    if (_random.Next(0, 2) == 0)
                        continue;

                    SetRandoNumber(x, y);

                    _cellLeft--;
                }
            }
        }

        private void GenerateNumbersManually()
        {
            int[,] numbers = { {0, 4, 0, 6, 9, 0, 8, 0, 2 },
                             {5, 0, 2, 4, 0, 0, 0, 7, 0 },
                             {0, 7, 0, 0, 5, 0, 0, 0, 9 },
                             {0, 0, 0, 5, 0, 2, 3, 1, 0 },
                             {2, 1, 0, 3, 0, 0, 0, 6, 8 },
                             {0, 5, 3, 1, 4, 0, 0, 0, 0 },
                             {0, 0, 0, 0, 0, 4, 7, 9, 0 },
                             {4, 3, 0, 0, 1, 0, 2, 0, 0 },
                             {9, 0, 8, 0, 0, 0, 0, 4, 0 } };

            for (int y = 0; y < GridSize.Height; y++)
            {
                for (int x = 0; x < GridSize.Width; x++)
                {
                    int num = numbers[y, x];

                    if (num == 0)
                        continue;

                    _field[y, x].SetStartNumber(num);

                    _cellLeft--;
                }
            }
        }

        public void GenerateNumbers()
        {
            GenerateNumbersRandomly();
        }

        private void SetRandoNumber(int x, int y)
        {
            int number;

            do
            {
                number = _random.Next(1, 10);
            } while (!CheckNumber(number, x, y));

            _field[y, x].SetStartNumber(number);
        }

        private bool CheckNumber(int number, int x, int y)
        {
            return CheckRow(number, x, y) && CheckColumn(number, y, x) && CheckBlock(number, x, y);
        }

        public bool CheckRow(int number, int current_x, int y)
        {
            for (int x = 0; x < GridSize.Width; x++)
            {
                Cell currentCell = _field[y, x];

                if (x != current_x && number == currentCell.Number)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckColumn(int number, int currentY, int x)
        {
            for (int y = 0; y < GridSize.Height; y++)
            {
                Cell currentCell = _field[y, x];

                if (y != currentY && number == currentCell.Number)
                {
                    return false;
                }
            }

            return true;
        }

        public bool CheckBlock(int number, int x, int y)
        {
            return _blocks[y / 3, x / 3].CheckBlock(number, x, y);
        }
    }

    class Block
    {
        public static readonly Size Size = new Size(Cell.Size.Width * Field.BlockSize.Width, Cell.Size.Height * Field.BlockSize.Height);

        private Panel _block;
        private List<Cell> _cellList = new List<Cell>();

        public Block(Point location)
        {
            _block = new Panel();
            _block.Location = location;
            _block.BorderStyle = BorderStyle.Fixed3D;
            _block.Size = Size;
        }

        public void SetCell(Cell cell)
        {
            _block.Controls.Add(cell.GetCellBody());
            _cellList.Add(cell);
        }

        public void Show(Form form)
        {
            form.Controls.Add(_block);
        }

        public bool CheckBlock(int number, int currentX, int currentY)
        {
            int cellIndex = currentY * currentX + currentX;

            for (int i = 0; i < Field.BlockSize.Width * Field.BlockSize.Height; i++)
            {
                Cell currentCell = _cellList[i];

                if (i != cellIndex && number == currentCell.Number)
                {
                    return false;
                }
            }

            return true;
        }
    }

    class Cell
    {
        public static readonly Size Size = new Size(30, 30);
        public readonly Point GridLocation;
        public readonly Point GlobalLocation;

        private Button _body;
        private int _number = 0;
        private Form1 _form;
        private bool _isUsed = false;

        public bool IsUsed
        {
            get { return _isUsed; }
        }

        public int Number
        {
            get { return _number; }
        }

        public Button GetCellBody()
        {
            return _body;
        }

        public Cell(Point globalLocation, Point location, Form1 form)
        {
            _body = new Button();

            GridLocation = new Point(location.X / Size.Width, location.Y / Size.Height);
            GlobalLocation = globalLocation;
            _form = form;

            _body.Size = Size;
            _body.Location = location;
            _body.FlatStyle = FlatStyle.Flat;
            _body.FlatAppearance.BorderSize = 1;
            _body.BackColor = Color.White;

            _body.KeyDown += Button_KeyDown;
        }

        public void ClearNumber()
        {
            _number = 0;
            _body.BackColor = Color.White;
            _body.Text = "";
        }

        public void SetNumber(int number)
        {
            _number = number;
            _body.Text = _number.ToString();
        }

        public void SetCorrectNumber(int number)
        {
            _body.BackColor = Color.White;

            SetNumber(number);
        }

        public void SetIncorrectNumber(int number)
        {
            _body.BackColor = Color.Red;

            SetNumber(number);
        }

        public void SetStartNumber(int number)
        {
            _body.Enabled = false;
            _body.BackColor = Color.Gray;

            SetNumber(number);

            _isUsed = true;
        }

        public void Button_KeyDown(object sender, KeyEventArgs e)
        {
            int key = 0;

            switch (e.KeyCode)
            {
                case Keys.D1:
                case Keys.NumPad1:
                    key = 1;
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    key = 2;
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    key = 3;
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    key = 4;
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    key = 5;
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    key = 6;
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    key = 7;
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    key = 8;
                    break;
                case Keys.D9:
                case Keys.NumPad9:
                    key = 9;
                    break;
                case Keys.Delete:
                    ClearNumber();
                    return;
                case Keys.Insert:
                    _form.UseBackTraking();
                    return;
                default:
                    return;
            }

            _form.TrySetNumber(key, GlobalLocation.X, GlobalLocation.Y);
        }
    }
}
