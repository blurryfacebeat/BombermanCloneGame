using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Boomberman
{
    public class Bomb
    {
        Timer timer;

        //Кол-во секунд
        int countSec = 4;

        PictureBox[,] mapPic;

        public Point bombPlace
        {
            get;
            private set;
        }

        deBoom boBoom;

        public Bomb(PictureBox[,] _mapPic, Point _bombPlace, deBoom bb)
        {
            mapPic = _mapPic;
            bombPlace = _bombPlace;
            boBoom = bb;
            CreateTime();
            timer.Enabled = true;
        }

        private void CreateTime()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Tick += timer_Tick;
        }

        void timer_Tick(object sender, System.EventArgs e)
        {
            if(countSec <= 0)
            {
                timer.Enabled = false;
                boBoom(this);
                return;
            }
            WriteTimer(--countSec);
        }

        private void WriteTimer(int num)
        {
            mapPic[bombPlace.X, bombPlace.Y].Image = Properties.Resources.bomb;
            mapPic[bombPlace.X, bombPlace.Y].Refresh();

            //Используем ресурс только в этой части кода
            using (Graphics gr = mapPic[bombPlace.X, bombPlace.Y].CreateGraphics()) 
            {
                PointF point = new PointF(
                    mapPic[bombPlace.X, bombPlace.Y].Size.Width / 4 + 100,
                    mapPic[bombPlace.X, bombPlace.Y].Size.Height / 4 + 5);
                gr.DrawString(
                    num.ToString(),
                    new Font("Arial", 20),
                    Brushes.Red,
                    new PointF());

            }
        }

        //Реакция бомбы
        public void Reaction()
        {
            countSec = 0;
        }
    }
}
