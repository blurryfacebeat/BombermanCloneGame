using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boomberman
{


    class MovingClass
    {
        PictureBox bomber;

        PictureBox[,] mapPic;

        Condition[,] map;

        deAddBonus addBonus;

        public MovingClass(PictureBox item, PictureBox[,] _mapPic, Condition[,] _map, deAddBonus methodBonus)
        {
            bomber = item;
            mapPic = _mapPic;
            map = _map;
            addBonus = methodBonus;
        }

        public void Move(int sx, int sy)
        {
            //Проверка на пустую клетку
            if (isEmpty(ref sx, ref sy))
            {
                bomber.Location = new Point(bomber.Location.X + sx, bomber.Location.Y + sy);
                Point myPlace = MyNowPoint();
                if (map[myPlace.X, myPlace.Y] == Condition.bonus)
                {
                    addBonus(BonusClass.GetBonus());
                    map[myPlace.X, myPlace.Y] = Condition.empty;
                    mapPic[myPlace.X, myPlace.Y].Image = Properties.Resources.ground;
                }
            }
        }

        private bool isEmpty(ref int sx, ref int sy)
        {
            Point playerPoint = MyNowPoint();

            //Ищем правую часть персонажа
            int bomberRight = bomber.Location.X + bomber.Size.Width;
            //Ищем левую часть персонажа
            int bomberLeft = bomber.Location.X;
            //Ищем нижнюю часть персонажа
            int bomberDown = bomber.Location.Y + bomber.Size.Height;
            //Ищем верхнюю часть персонажа
            int bomberUp = bomber.Location.Y;

            //Стены для взаимодействия
            //Получаем левую сторону правого квадрата
            int rightWallLeft = mapPic[playerPoint.X + 1, playerPoint.Y].Location.X;
            //Получаем правую сторону левого квадрата
            int leftWallRight = mapPic[playerPoint.X - 1, playerPoint.Y].Location.X + mapPic[playerPoint.X - 1, playerPoint.Y].Size.Width;
            //Получаем верхнюю сторону нижнего квадрата
            int downWallUp = mapPic[playerPoint.X, playerPoint.Y + 1].Location.Y;
            //Получаем нижнюю сторону верхнего квадрата
            int upWallDown = mapPic[playerPoint.X, playerPoint.Y - 1].Location.Y + mapPic[playerPoint.X, playerPoint.Y - 1].Size.Height;

            //Чекаем чтобы не бегать сквозь углы
            //Правый верхний угол нижняя часть
            int rightUpWallDown = mapPic[playerPoint.X + 1, playerPoint.Y - 1].Location.Y + mapPic[playerPoint.X + 1, playerPoint.Y - 1].Size.Height;
            //Правый нижний угол верхняя часть
            int rightDownWallUp = mapPic[playerPoint.X + 1, playerPoint.Y + 1].Location.Y;
            //Левый верхний угол нижняя часть
            int leftUpWallDown = mapPic[playerPoint.X - 1, playerPoint.Y - 1].Location.Y + mapPic[playerPoint.X - 1, playerPoint.Y - 1].Size.Height;
            //Левый нижний угол верхняя часть
            int leftDownWallUp = mapPic[playerPoint.X - 1, playerPoint.Y + 1].Location.Y;

            //Чекаем чтобы не бегать сквозь углы
            //Правый верхний угол левая часть
            int rightUpWallLeft = mapPic[playerPoint.X + 1, playerPoint.Y - 1].Location.X;
            //Левый верхний угол правая часть
            int leftUpWallRight = mapPic[playerPoint.X - 1, playerPoint.Y - 1].Location.X + mapPic[playerPoint.X - 1, playerPoint.Y - 1].Size.Width;
            //Правый нижний угол левая часть
            int rightDownWallLeft = mapPic[playerPoint.X + 1, playerPoint.Y + 1].Location.X;
            //Левый нижний угол правая часть
            int leftDownWallRight = mapPic[playerPoint.X - 1, playerPoint.Y + 1].Location.X + mapPic[playerPoint.X - 1, playerPoint.Y + 1].Size.Width;

            //Смещение, если не влезаем, чтобы не перепрыгивать с углов
            int offset = 5;

            if (sx > 0 && (map[playerPoint.X + 1, playerPoint.Y] == Condition.empty || map[playerPoint.X + 1, playerPoint.Y] == Condition.fire || map[playerPoint.X + 1, playerPoint.Y] == Condition.bonus))
            {
                if (bomberUp < rightUpWallDown)
                {
                    if (rightUpWallDown - bomberUp > offset)
                    {
                        sy = offset;
                    }
                    else
                    {
                        sy = rightUpWallDown - bomberUp;
                    }
                }

                if (bomberDown > rightDownWallUp)
                {
                    if (rightDownWallUp - bomberDown < -offset)
                    {
                        sy = -offset;
                    }
                    else
                    {
                        sy = rightDownWallUp - bomberDown;
                    }
                }

                return true;
            }

            if (sx < 0 && (map[playerPoint.X - 1, playerPoint.Y] == Condition.empty || map[playerPoint.X - 1, playerPoint.Y] == Condition.fire || map[playerPoint.X - 1, playerPoint.Y] == Condition.bonus))
            {
                if (bomberUp < leftUpWallDown)
                {
                    if (leftUpWallDown - bomberUp > offset)
                    {
                        sy = offset;
                    }
                    else
                    {
                        sy = leftUpWallDown - bomberUp;
                    }
                }

                if (bomberDown > leftDownWallUp)
                {
                    if (leftDownWallUp - bomberDown < -offset)
                    {
                        sy = -offset;
                    }
                    else
                    {
                        sy = leftDownWallUp - bomberDown;
                    }
                }

                return true;
            }

            if (sy > 0 && (map[playerPoint.X, playerPoint.Y + 1] == Condition.empty || map[playerPoint.X, playerPoint.Y + 1] == Condition.fire || map[playerPoint.X, playerPoint.Y + 1] == Condition.bonus))
            {
                if (bomberRight > rightDownWallLeft)
                {
                    if (rightDownWallLeft - bomberRight < -offset)
                    {
                        sx = -offset;
                    }
                    else
                    {
                        sx = rightDownWallLeft - bomberRight;
                    }
                }

                if (bomberLeft < leftDownWallRight)
                {
                    if (leftDownWallRight - bomberLeft > offset)
                    {
                        sx = offset;
                    }
                    else
                    {
                        sx = leftDownWallRight - bomberLeft;
                    }
                }

                return true;
            }

            if (sy < 0 && (map[playerPoint.X, playerPoint.Y - 1] == Condition.empty || map[playerPoint.X, playerPoint.Y - 1] == Condition.fire || map[playerPoint.X, playerPoint.Y - 1] == Condition.bonus))
            {
                if (bomberRight > rightUpWallLeft)
                {
                    if (rightUpWallLeft - bomberRight < -offset)
                    {
                        sx = -offset;
                    }
                    else
                    {
                        sx = rightUpWallLeft - bomberRight;
                    }
                }

                if (bomberLeft < leftUpWallRight)
                {
                    if (leftUpWallRight - bomberLeft > offset)
                    {
                        sx = offset;
                    }
                    else
                    {
                        sx = leftUpWallRight - bomberLeft;
                    }
                }

                return true;
            }



            if (sx > 0 && bomberRight + sx > rightWallLeft)
            {
                sx = rightWallLeft - bomberRight;
            }

            if (sx < 0 && bomberLeft + sx < leftWallRight)
            {
                sx = leftWallRight - bomberLeft;
            }

            if (sy > 0 && bomberDown + sy > downWallUp)
            {
                sy = downWallUp - bomberDown;
            }

            if (sy < 0 && bomberUp + sy < upWallDown)
            {
                sy = upWallDown - bomberUp;
            }

            return true;
        }

        //Определяем текущее местоположение игрока
        public Point MyNowPoint()
        {

            //Находим центр
            Point point = new Point();
            {
                point.X = bomber.Location.X + bomber.Size.Width / 2;
                point.Y = bomber.Location.Y + bomber.Size.Height / 2;
            }

            //
            for (int x = 0; x < mapPic.GetLength(0); x++)
            {
                for (int y = 0; y < mapPic.GetLength(1); y++)
                {
                    if ((mapPic[x, y].Location.X < point.X) && (mapPic[x, y].Location.Y < point.Y) && (mapPic[x, y].Location.X + mapPic[x, y].Size.Width > point.X) && (mapPic[x, y].Location.Y + mapPic[x, y].Size.Height > point.Y))
                    {
                        return new Point(x, y);
                    }
                }
            }

            return point;
        }
    }
}
