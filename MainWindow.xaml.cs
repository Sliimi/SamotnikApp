using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SamotnikApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _ballCounter;
        private BoardSpace[,] _boardArray;
        private Tuple<int, int> _chosenBall;
        private bool _isChosen;
        private Stack<Tuple<int, int, int, int>> _lastMoves;

        public MainWindow()
        {
            InitializeComponent();
            Setup();
            _lastMoves = new Stack<Tuple<int, int, int, int>>();
        }

        private void Setup()
        {
            _ballCounter = 32;
            Counter.Text = _ballCounter.ToString();
            _boardArray = new BoardSpace[7, 7];
            for (var i = 0; i < 7; i++)
            {
                for (var j = 0; j < 7; j++)
                {
                    if (i is < 2 or > 4 && j is < 2 or > 4)
                    {
                        _boardArray[i, j] = new BoardSpace(false);
                    }
                    else
                    {
                        var ellipse = createEllipse();
                        _boardArray[i, j] = new BoardSpace(true, ellipse);
                        Board.Children.Add(ellipse);
                        Grid.SetColumn(ellipse, i);
                        Grid.SetRow(ellipse, j);
                    }
                }
            }

            Board.Children.Remove(_boardArray[3, 3].CurrentBall);
            _boardArray[3, 3].CurrentBall = null;
        }

        private Ellipse createEllipse()
        {
            var ellipse = new Ellipse();
            ellipse.Stroke = Brushes.Crimson;
            ellipse.Fill = Brushes.Crimson;
            ellipse.IsHitTestVisible = false;
            return ellipse;
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < 7; i++)
            {
                for (var j = 0; j < 7; j++)
                {
                    if (_boardArray[i, j].CurrentBall is not null)
                    {
                        Board.Children.Remove(_boardArray[i, j].CurrentBall);
                    }
                }
            }

            Notifications.Text = "";
            Setup();
        }

        private void Move_Ball(object sender, RoutedEventArgs e)
        {
            if (!_isChosen)
            {
                var button = e.Source as Button;
                var i = Grid.GetColumn(button);
                var j = Grid.GetRow(button);
                if (_boardArray[i, j].CurrentBall != null)
                {
                    _chosenBall = new Tuple<int, int>(i, j);
                    _isChosen = true;
                }
            }
            else
            {
                _isChosen = false;
                var button = e.Source as Button;
                var i = Grid.GetColumn(button);
                var j = Grid.GetRow(button);
                var test = IsMoveValid(_chosenBall.Item1, _chosenBall.Item2, i, j);
                if (test)
                {
                    CommitMove(_chosenBall.Item1, _chosenBall.Item2, i, j);
                    _lastMoves.Push(new Tuple<int, int, int, int>(_chosenBall.Item1, _chosenBall.Item2, i, j));
                    _ballCounter--;
                    Counter.Text = _ballCounter.ToString();
                    Notifications.Text = "";
                    CheckWinLoseCondition();
                }
            }
        }

        private bool IsMoveValid(int oldI, int oldJ, int newI, int newJ)
        {
            if (oldI is < 0 or > 6 || oldJ is < 0 or > 6 || newI is < 0 or > 6 || newJ is < 0 or > 6)
                return false;
            if (_boardArray[newI, newJ].CurrentBall != null)
                return false;
            if (!_boardArray[newI, newJ].IsValid)
                return false;

            if (oldI == newI - 2 && oldJ == newJ && _boardArray[newI - 1, newJ].CurrentBall != null)
            {
                return true;
            }

            if (oldI == newI + 2 && oldJ == newJ && _boardArray[newI + 1, newJ].CurrentBall != null)
            {
                return true;
            }

            if (oldI == newI && oldJ == newJ - 2 && _boardArray[newI, newJ - 1].CurrentBall != null)
            {
                return true;
            }

            if (oldI == newI && oldJ == newJ + 2 && _boardArray[newI, newJ + 1].CurrentBall != null)
            {
                return true;
            }

            return false;
        }

        private void CommitMove(int oldI, int oldJ, int newI, int newJ)
        {
            var ellipseToMove = _boardArray[oldI, oldJ].CurrentBall;
            Grid.SetColumn(ellipseToMove, newI);
            Grid.SetRow(ellipseToMove, newJ);
            Ellipse? ellipseToDelete;
            if (oldI == newI - 2)
            {
                ellipseToDelete = _boardArray[newI - 1, newJ].CurrentBall;
                _boardArray[newI - 1, newJ].CurrentBall = null;
            }
            else if (oldI == newI + 2)
            {
                ellipseToDelete = _boardArray[newI + 1, newJ].CurrentBall;
                _boardArray[newI + 1, newJ].CurrentBall = null;
            }
            else if (oldJ == newJ - 2)
            {
                ellipseToDelete = _boardArray[newI, newJ - 1].CurrentBall;
                _boardArray[newI, newJ - 1].CurrentBall = null;
            }
            else
            {
                ellipseToDelete = _boardArray[newI, newJ + 1].CurrentBall;
                _boardArray[newI, newJ + 1].CurrentBall = null;
            }
            _boardArray[oldI, oldJ].CurrentBall = null;
            Board.Children.Remove(ellipseToDelete);
            _boardArray[newI, newJ].CurrentBall = ellipseToMove;
        }

        private void CheckWinLoseCondition()
        {
            if (_ballCounter == 1)
            {
                Notifications.Text = "Wygrana!";
            }
            else
            {
                for (var i = 0; i < 7; i++)
                {
                    for (var j = 0; j < 7; j++)
                    {
                        if (_boardArray[i, j].CurrentBall is null) continue;
                        if (IsMoveValid(i, j, i + 2, j))
                        {
                            return;
                        }

                        if (IsMoveValid(i, j, i - 2, j))
                        {
                            return;
                        }

                        if (IsMoveValid(i, j, i, j + 2))
                        {
                            return;
                        }

                        if (IsMoveValid(i, j, i, j - 2))
                        {
                            return;
                        }
                    }
                }

                Notifications.Text = "Przegrałeś!";
            }
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (_lastMoves.Count != 0)
            {
                var lastMove = _lastMoves.Pop();
                UndoMove(lastMove.Item1, lastMove.Item2, lastMove.Item3, lastMove.Item4);
            }
            else
            {
                Notifications.Text = "Brak ruchu do cofnięcia";
            }
        }

        private void UndoMove(int oldI, int oldJ, int newI, int newJ)
        {
            var ellipseToMove = _boardArray[newI, newJ].CurrentBall;
            Grid.SetColumn(ellipseToMove, oldI);
            Grid.SetRow(ellipseToMove, oldJ);
            var ellipseToAdd = createEllipse();
            Board.Children.Add(ellipseToAdd);
            _ballCounter++;
            Counter.Text = _ballCounter.ToString();
            if (oldI == newI - 2)
            {
                _boardArray[newI - 1, newJ].CurrentBall = ellipseToAdd;
                Grid.SetColumn(ellipseToAdd, newI - 1);
                Grid.SetRow(ellipseToAdd, newJ);
            }
            else if (oldI == newI + 2)
            {
                _boardArray[newI + 1, newJ].CurrentBall = ellipseToAdd;
                Grid.SetColumn(ellipseToAdd, newI + 1);
                Grid.SetRow(ellipseToAdd, newJ);
            }
            else if (oldJ == newJ - 2)
            {
                _boardArray[newI, newJ - 1].CurrentBall = ellipseToAdd;
                Grid.SetColumn(ellipseToAdd, newI);
                Grid.SetRow(ellipseToAdd, newJ - 1);
            }
            else
            {
                _boardArray[newI, newJ + 1].CurrentBall = ellipseToAdd;
                Grid.SetColumn(ellipseToAdd, newI);
                Grid.SetRow(ellipseToAdd, newJ + 1);
            }
            _boardArray[newI, newJ].CurrentBall = null;
            _boardArray[oldI, oldJ].CurrentBall = ellipseToMove;
        }
    }
}