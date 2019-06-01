using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boomberman
{
    public delegate void deAddBonus(Bonuses b);

    //Перемещения
    enum Arrows
    {
        left,
        right,
        up,
        down
    }

    class Bomber
    {
        PictureBox bomber;

        int step;

        MovingClass moving;

        public int lenFire
        {
            get;
            private set;
        }

        //Массив с бомбами
        public List<Bomb> bombs
        {
            get;
            private set;
        }

        //Кол-во бомб
        int countBombs;

        Label score;

        public Bomber(PictureBox _bomber, PictureBox[,] _mapPic, Condition[,] _map, Label lbScore)
        {
            bomber = _bomber;
            score = lbScore;
            step = 5;
            countBombs = 3;
            lenFire = 3;
            bombs = new List<Bomb>();
            moving = new MovingClass(_bomber, _mapPic, _map, AddBonus);
            ChandeScore();
        }

        public void PlayerMove (Arrows arrow)
        {
            switch (arrow)
            {
                case Arrows.left:
                    bomber.Image = Properties.Resources.moveleft;
                    moving.Move(-step, 0);
                    break;
                case Arrows.right:
                    bomber.Image = Properties.Resources.moveright;
                    moving.Move(step, 0);
                    break;
                case Arrows.up:
                    bomber.Image = Properties.Resources.bomber;
                    moving.Move(0, -step);
                    break;
                case Arrows.down:
                    bomber.Image = Properties.Resources.bomber;
                    moving.Move(0, step);
                    break;
                default:
                    break;
            }
        }

        public Point MyNowPoint()
        {
            return moving.MyNowPoint();
        }

        public bool PutBomb(PictureBox[,] mapPic, deBoom bb)
        {
            if (bombs.Count >= countBombs)
            {
                return false;
            }

            Bomb bomb = new Bomb(mapPic, MyNowPoint(), bb);
            bombs.Add(bomb);
            return true;
        }

        //Удаление бомбы
        public void RemoveBomb(Bomb bomb)
        {
            bombs.Remove(bomb);
        }

        private void ChandeScore(string alarm = "")
        {
            if (score == null)
            {
                return;
            }

            score.Text = "Скорость: " + step + "  Количество бомб: " + countBombs + "  Сила бомб: " + lenFire + " " + alarm;
        }

        private void AddBonus(Bonuses bonuses)
        {
            switch (bonuses)
            {
                case Bonuses.bomb_plus:
                    countBombs++;
                    //ChandeScore("+1 К БОМБЕ");
                    break;
                case Bonuses.bomb_minus:
                    countBombs = countBombs == 1 ? 1 : countBombs--;
                    //ChandeScore("-1 К БОМБЕ");
                    break;
                case Bonuses.fire_plus:
                    lenFire++;
                    //ChandeScore("+1 К ОГНЮ");
                    break;
                case Bonuses.fire_minus:
                    lenFire = lenFire == 1 ? 1 : lenFire--;
                    //ChandeScore("-1 К ОГНЮ");
                    break;
                case Bonuses.speed_plus:
                    step++;
                    //ChandeScore("+1 К СКОРОСТИ");
                    break;
                case Bonuses.speed_minus:
                    step = step <= 3 ? 3 : step--;
                    //ChandeScore("-1 К СКОРОСТИ");
                    break;
                default:
                    break;
            }
            ChandeScore(bonuses.ToString());
        }
    }
}