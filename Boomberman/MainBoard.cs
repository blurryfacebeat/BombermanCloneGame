using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
    
namespace Boomberman
{
    public delegate void deBoom(Bomb b);

    //Статичное состояние клетки на поле, будут менять состояние только при определенных условиях (Пример: кирпич - взорвали и т.д.)
    enum Condition
    {
        empty,
        wall,
        brick,
        bomb,
        fire,
        bonus
    }

    class MainBoard
    {
        //Главная панель
        Panel GamePanel;

        //Карта
        PictureBox[,] mapPic;

        Condition[,] map;

        //Кол-во. клеток по X и Y
        int sizeX = 17, sizeY = 11;

        static Random rand = new Random();

        Bomber bomber;

        Enemy enemy;

        List<Enemy> enemies;

        deClear NeedClear;

        Label score;

        //Главная доска конструктор
        public MainBoard(Panel panel, deClear _clear, Label _score)
        {
            //Присваиваем переменной нашу panel
            GamePanel = panel;

            enemies = new List<Enemy>();

            NeedClear = _clear;

            score = _score;

            //Размер квадрата
            int boxSize;

            if ((GamePanel.Width / sizeX) < (GamePanel.Height / sizeY))
            {
                boxSize = GamePanel.Width / sizeX;
            }
            else
            {
                boxSize = GamePanel.Height / sizeY;
            }

            //Создаем карту
            InitStartMap(boxSize);

            //Создаем персонажа
            InitStartPlayer(boxSize);

            for (int i = 0; i < 5; i++)
            {
                //Создаем противника
                InitStartEnemy(boxSize);
            }

            BonusClass.Prepare();
        }

        //Создаем карту
        private void InitStartMap(int boxSize)
        {
            mapPic = new PictureBox[sizeX, sizeY];

            map = new Condition[sizeX, sizeY];

            //Очищаем главную панель
            GamePanel.Controls.Clear();

            

            //Проходим по всему массиву и отрисовываем клетки
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (x == 0 || y == 0 || x == sizeX - 1 || y == sizeY - 1)
                    {
                        CreatePlace(new Point(x, y), boxSize, Condition.wall);
                    }
                    else if (x % 2 == 0 && y % 2 == 0)
                    {
                        CreatePlace(new Point(x, y), boxSize, Condition.wall);
                    }
                    //Генерирует число от 0 до 3 если одно из 3 значений выпадает равным 0, то создаем кирпич
                    else if (rand.Next(5) == 1)
                    {
                        CreatePlace(new Point(x, y), boxSize, Condition.brick);
                    }
                    else
                    {
                        CreatePlace(new Point(x, y), boxSize, Condition.empty);
                    }
                    //CreatePlace(new Point(x, y), boxSize);
                }
            }
            //Освобождаем начальное место для передвижения персонажа
            ChangeCondition(new Point(1, 1), Condition.empty);
            ChangeCondition(new Point(2, 1), Condition.empty);
            ChangeCondition(new Point(1, 2), Condition.empty);
        }

        //Отрисовываем клетки
        private void CreatePlace(Point point, int boxSize, Condition condition)
        {
            PictureBox picture = new PictureBox();

            //Узнаем местоположение picture
            picture.Location = new Point(point.X * (boxSize - 1), point.Y * (boxSize - 1));

            //Узнаем размер клетки
            picture.Size = new Size(boxSize, boxSize);

            //Добавляем рамки квадрату
            //picture.BorderStyle = BorderStyle.FixedSingle;

            //Добавляем изображение квадрату
            picture.SizeMode = PictureBoxSizeMode.StretchImage;

            //Присваиваем массиву только что созданную картинку
            mapPic[point.X, point.Y] = picture;

            //Добавляем picture на панель
            GamePanel.Controls.Add(picture);

            //Обновляем состояние
            ChangeCondition(point, condition);

            //Добавляем цвет для picture
            //picture.BackColor = Color.WhiteSmoke;
        }

        //Меняем состояния
        private void ChangeCondition(Point point, Condition newCondition)
        {
            switch (newCondition)
            {
                //Показываем стену
                case Condition.wall:
                    mapPic[point.X, point.Y].Image = Properties.Resources.wall;
                    break;
                //Показываем кирпич
                case Condition.brick:
                    mapPic[point.X, point.Y].Image = Properties.Resources.brick;
                    break;
                //Показываем бомбу
                case Condition.bomb:
                    mapPic[point.X, point.Y].Image = Properties.Resources.bomb;
                    break;
                //Показываем огонь
                case Condition.fire:
                    mapPic[point.X, point.Y].Image = Properties.Resources.fire;
                    break;
                //Показываем бонус
                case Condition.bonus:
                    mapPic[point.X, point.Y].Image = Properties.Resources.bonus;
                    break;
                default:
                    //Показываем землю
                    mapPic[point.X, point.Y].Image = Properties.Resources.ground;
                    break;
            }
            //Меняем состояние по точке, передаем в новое состояние
            map[point.X, point.Y] = newCondition;
        }

        //Создаем персонажа
        private void InitStartPlayer(int boxSize)
        {
            //Координаты появления
            int x = 1, y = 1;

            PictureBox picture = new PictureBox();

            //Выравниваем персонажа по центру клетки
            picture.Location = new Point(x * (boxSize) + 7, y * (boxSize) + 3);

            //Размер персонажа
            picture.Size = new Size(boxSize - 14, boxSize - 6);

            //Присваиваем изображение
            picture.Image = Properties.Resources.bomber;
            picture.BackgroundImage = Properties.Resources.ground;
            picture.BackgroundImageLayout = ImageLayout.Stretch;
            picture.SizeMode = PictureBoxSizeMode.StretchImage;

            //Добавляем на панель
            GamePanel.Controls.Add(picture);

            //Выставляем персонажа на передний план
            picture.BringToFront();

            bomber = new Bomber(picture, mapPic, map, score);
        }

        //Создаем врага
        private void InitStartEnemy(int boxSize)
        {
            //Координаты появления
            int x = 15, y = 9;

            FindEmptyPlace(out x, out y);

            PictureBox picture = new PictureBox();

            //Выравниваем персонажа по центру клетки
            picture.Location = new Point(x * (boxSize) - 8, y * (boxSize) - 7);

            //Размер врага
            picture.Size = new Size(boxSize - 14, boxSize - 5);

            //Присваиваем изображение
            picture.Image = Properties.Resources.enemy;
            picture.BackgroundImage = Properties.Resources.ground;
            picture.BackgroundImageLayout = ImageLayout.Stretch;
            picture.SizeMode = PictureBoxSizeMode.StretchImage;

            //Добавляем на панель
            GamePanel.Controls.Add(picture);

            //Выставляем персонажа на передний план
            picture.BringToFront();

            //Заносим противников в лист
            enemies.Add(new Enemy(picture, mapPic, map, bomber));
        }

        //Ищем свободное место
        private void FindEmptyPlace(out int x, out int y)
        {
            int loop = 0;

            do
            {
                //Боты появляются только в правой части экрана
                x = rand.Next(map.GetLength(0) / 2, map.GetLength(0));
                y = rand.Next(1, map.GetLength(1));
            } while (map[x, y] != Condition.empty && loop++ < 100);
        }

        //Движения персонажа
        public void PlayerMove(Arrows arrow)
        {
            if (bomber == null)
            {
                return;
            }
            bomber.PlayerMove(arrow);
        }

        //Кладем бомбу
        public void PutBomb()
        {
            Point bomberPoint = bomber.MyNowPoint();

            //Нельзя ставить несколько бомб в одном месте
            if (map[bomberPoint.X, bomberPoint.Y] == Condition.bomb)
            {
                return;
            }

            if (bomber.PutBomb(mapPic, Boom))
            {
                ChangeCondition(bomber.MyNowPoint(), Condition.bomb);
            }
        }

        //Взрыв
        private void Boom(Bomb bomb)
        {
            //Распространение огня
            ChangeCondition(bomb.bombPlace, Condition.fire);
            Flame(bomb.bombPlace, Arrows.left);
            Flame(bomb.bombPlace, Arrows.right);
            Flame(bomb.bombPlace, Arrows.up);
            Flame(bomb.bombPlace, Arrows.down);
            //Удаляем бомбу
            bomber.RemoveBomb(bomb);

            FireDestroy();
            //Очищаем через секунду
            NeedClear();
        }

        //Уничтожение огнем
        private void FireDestroy()
        {
            List<Enemy> delEnemies = new List<Enemy>();

            //Перебираем всех противников из коллекции
            foreach (Enemy enemy in enemies)
            {
                Point enemyPoint = enemy.MyNowPoint();
                if (map[enemyPoint.X, enemyPoint.Y] == Condition.fire)
                {
                    delEnemies.Add(enemy);
                }
            }

            //Удаляем спрайты противников с карты
            for (int x = 0; x < delEnemies.Count; x++)
            {
                enemies.Remove(delEnemies[x]);
                GamePanel.Controls.Remove(delEnemies[x].enemy);
                delEnemies[x] = null;
            }
            //Собираем мусор
            GC.Collect();
            //Ждем окончания отработки
            GC.WaitForPendingFinalizers();
        }

        //Пламя
        private void Flame(Point bombPlace, Arrows arrow)
        {
            int sx = 0, sy = 0;

            switch (arrow)
            {
                case Arrows.left:
                    sx = -1;
                    break;
                case Arrows.right:
                    sx = 1;
                    break;
                case Arrows.up:
                    sy = -1;
                    break;
                case Arrows.down:
                    sy = 1;
                    break;
                default:
                    break;
            }

            bool isNotDone = true;
            int x = 0;
            int y = 0;
            do
            {
                x += sx;
                y += sy;
                if (Math.Abs(x) > bomber.lenFire || Math.Abs(y) > bomber.lenFire)
                {
                    break;
                }
                if (isFire(bombPlace, x, y))
                {
                    ChangeCondition(new Point(bombPlace.X + x, bombPlace.Y + y), Condition.fire);
                }
                else
                {
                    isNotDone = false;
                }
            } while (isNotDone);
        }

        //Можно ли
        private bool isFire(Point place, int sx, int sy)
        {
            switch (map[place.X + sx, place.Y + sy])
            {
                case Condition.empty:
                    return true;
                case Condition.wall:
                    return false;
                case Condition.brick:
                    if (rand.Next(-1, 2) > 0)
                    {
                        ChangeCondition(new Point(place.X + sx, place.Y + sy), Condition.bonus);
                    }
                    else
                    {
                        ChangeCondition(new Point(place.X + sx, place.Y + sy), Condition.fire);
                    }
                    return false;
                case Condition.bomb:
                    foreach (Bomb bomb in bomber.bombs)
                    {
                        //Если ставим бомбу на бомбу, то взрываем
                        if (bomb.bombPlace == new Point(place.X + sx, place.Y + sy))
                        {
                            bomb.Reaction();
                        }
                    }
                    return false;
                default:
                    return true;
            }
        }

        //Очистка огня
        public void ClearFire()
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    if (map[x, y] == Condition.fire)
                    {
                        ChangeCondition(new Point(x, y), Condition.empty);
                    }
                }
            }
        }

        //Конец игры
        public bool GameOver()
        {
            Point myPoint = bomber.MyNowPoint();

            //Если мы попали на огонь
            if (map[myPoint.X, myPoint.Y] == Condition.fire)
            {
                return true;
            }

            //Если все противники мертвы
            if (enemies.Count == 0)
            {
                return true;
            }

            //Столкновение с противником
            foreach (Enemy enemy in enemies)
            {
                if (myPoint == enemy.MyNowPoint())
                {
                    return true;
                }
            }
            return false;
        }

        public void SetEnemiesLevel(int level)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.SetLevel(level);
            }
        }
    }
}
