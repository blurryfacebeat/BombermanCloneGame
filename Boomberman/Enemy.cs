using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boomberman
{
    class Enemy
    {
        /// <summary>
        /// Уровни сложности:
        ///     1. Easy - выбирает доступную точку и бежит к ней
        ///     2. Medium - выбирает доступную точку и бежит к ней, но, если видит бомбу или огонь - убегает
        ///     3. Hard - бегает от точки к точке, если доступен человек, бежит к нему, если встретил бомбу - убегает
        /// </summary>
        /// 

        //Уровень сложности
        int level = 1;

        public PictureBox enemy
        {
            get;
            private set;
        }

        //Мувы
        Timer timer;

        //Точка для перемещения
        Point destinePlace;

        //Точка нахождения противника
        Point enemyPlace;

        MovingClass moving;

        int step = 3;

        Condition[,] map;

        int[,] fmap;

        //Цифры задйествованные в пути
        int paths;

        //Путь который строим
        Point[] path;

        //Текущий шаг, который есть
        int pathStep;

        static Random rand = new Random();

        Bomber bomber;

        public Enemy(PictureBox picEnemy, PictureBox[,] _mapPic, Condition[,] _map, Bomber _bomber)
        {
            enemy = picEnemy;
            map = _map;
            bomber = _bomber;
            fmap = new int[map.GetLength(0), map.GetLength(1)];
            path = new Point[map.GetLength(0) * map.GetLength(1)];
            moving = new MovingClass(picEnemy, _mapPic, _map, AddBonus);
            enemyPlace = moving.MyNowPoint();
            destinePlace = enemyPlace;
            CreateTimer();
            timer.Enabled = true;
        }

        private void CreateTimer()
        {
            timer = new Timer();
            //В ms
            timer.Interval = 10;
            timer.Tick += timer_Tick;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (enemyPlace == destinePlace)
            {
                GetNewPlace();
            }

            if (path[0].X == 0 && path[0].Y == 0)
            {
                if (!FindPath())
                {
                    return;
                }
            }

            if (pathStep > paths)
            {
                return;
            }

            if (path[pathStep] == enemyPlace)
            {
                pathStep++;
            }
            else
            {
                MoveEnemy(path[pathStep]);
            }
        }



        private void MoveEnemy(Point newPlace)
        {
            int sx = 0, sy = 0;

            if (enemyPlace.X < newPlace.X)
            {
                sx = newPlace.X - enemyPlace.X > step ? step : newPlace.X - enemyPlace.X;
            }
            else
            {
                sx = enemyPlace.X - newPlace.X < step ? newPlace.X - enemyPlace.X : -step;
            }
            if (enemyPlace.Y < newPlace.Y)
            {
                sy = newPlace.Y - enemyPlace.Y > step ? step : newPlace.Y - enemyPlace.Y;
            }
            else
            {
                sy = enemyPlace.Y - newPlace.Y < step ? newPlace.Y - enemyPlace.Y : -step;
            }

            moving.Move(sx, sy);

            enemyPlace = moving.MyNowPoint();

            //При уровне сложности 2, если противник видит огонь или бомбу, то ищет новое местоположение
            if (level >= 2 && (map[newPlace.X, newPlace.Y] == Condition.bomb || map[newPlace.X, newPlace.Y] == Condition.fire))
            {
                GetNewPlace();
            }


        }

        //Поиск пути
        private bool FindPath()
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    fmap[x, y] = 0;
                }
            }
            bool added;
            bool found = false;
            fmap[enemyPlace.X, enemyPlace.Y] = 1;
            int nr = 1;
            //Поиск в ширину по массиву нашего *nr
            do
            {
                added = false;
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    for (int y = 0; y < map.GetLength(1); y++)
                    {
                        //Добавляем и маркеруем пути, пока есть возможность
                        if (fmap[x, y] == nr)
                        {
                            MarkPath(x + 1, y, nr + 1);
                            MarkPath(x - 1, y, nr + 1);
                            MarkPath(x, y - 1, nr + 1);
                            MarkPath(x, y + 1, nr + 1);
                            added = true;
                        }
                    } 
                }
                //Если путь проставлен как найденный
                if (fmap[destinePlace.X, destinePlace.Y] > 0)
                {
                    found = true;
                    break;
                }
                nr++;
            } while (added);
            //Если путь не найден
            if (!found)
            {
                return false;
            }
            int sx = destinePlace.X;
            int sy = destinePlace.Y;
            paths = nr;

            while (nr >= 0)
            {
                path[nr].X = sx;
                path[nr].Y = sy;
                if (isPath(sx + 1, sy, nr))
                {
                    sx++;
                }
                else if (isPath(sx - 1, sy, nr))
                {
                    sx--;
                }
                else if (isPath(sx, sy + 1, nr))
                {
                    sy++;
                }
                else if (isPath(sx, sy - 1, nr))
                {
                    sy--;
                }
                nr--;
            }
            pathStep = 0;
            return true;
        }

        //Пометка пути
        private void MarkPath(int x, int y, int n)
        {
            if (x < 0 || x >= map.GetLength(0))
            {
                return;
            }

            if (y < 0 || y >= map.GetLength(1))
            {
                return;
            }

            if (fmap[x, y] > 0)
            {
                return;
            }

            if (map[x, y] != Condition.empty)
            {
                return;
            }

            fmap[x, y] = n;
        }

        private bool isPath(int x, int y, int n)
        {
            if (x < 0 || x >= map.GetLength(0))
            {
                return false;
            }

            if (y < 0 || y >= map.GetLength(1))
            {
                return false;
            }

            return fmap[x, y] == n;
        }

        //Создает новое место, куда бот хотел бы попасть
        private void GetNewPlace()
        {
            //При уровне сложности 3 противник ищет местоположение игрока
            if (level == 3)
            {
                destinePlace = bomber.MyNowPoint();
                if (FindPath())
                {
                    return;
                }
            }
            int loop = 0;

            do
            {
                destinePlace.X = rand.Next(1, map.GetLength(0) - 1);
                destinePlace.Y = rand.Next(1, map.GetLength(1) - 1);
            } while (!FindPath() && loop++ < 100);

            if (loop >= 100)
            {
                destinePlace = enemyPlace;
            }
        }

        public Point MyNowPoint()
        {
            return moving.MyNowPoint();
        }

        public void SetLevel(int _level)
        {
            level = _level;
        }

        private void AddBonus(Bonuses bonuses)
        {

        }
    }
}
